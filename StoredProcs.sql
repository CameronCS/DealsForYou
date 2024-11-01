DELIMITER //

CREATE PROCEDURE AcceptOffer(
    IN p_car_id INT, 
    IN p_user_id INT, 
    IN p_offer_id INT, 
    IN p_admin_id INT,
    OUT p_transaction_id INT
)
BEGIN
    DECLARE v_total INT;
    DECLARE v_balance_id INT;

    -- Start a transaction
    START TRANSACTION;

    -- Get the offer total amount
    SELECT total INTO v_total
    FROM offer
    WHERE id = p_offer_id;

    -- Insert into balance table
    INSERT INTO balance (total, current)
    VALUES (v_total, 0);

    -- Get the newly inserted balance ID
    SET v_balance_id = LAST_INSERT_ID();

    -- Insert into transaction table
    INSERT INTO transaction (user_id, admin_id, car_id, balance_id, status)
    VALUES (p_user_id, p_admin_id, p_car_id, v_balance_id, 'Accepted');

    -- Set p_transaction_id to the last inserted ID in the transaction table
    SET p_transaction_id = LAST_INSERT_ID();

    -- Update the car to make it unavailable
    UPDATE car
    SET available = FALSE
    WHERE id = p_car_id;

    -- Optional: Archive the accepted offer instead of deleting it
    INSERT INTO archived_offer (user_id, car_id, price, offer, months, interest, monthly, total)
    SELECT user_id, car_id, price, offer, months, interest, monthly, total
    FROM offer
    WHERE id = p_offer_id;

    -- Delete the accepted offer from the offer table
    DELETE FROM offer WHERE id = p_offer_id;

    -- Delete any other offers associated with the same car ID
    DELETE FROM offer WHERE car_id = p_car_id AND id <> p_offer_id;

    -- Commit the transaction
    COMMIT;
END //

DELIMITER ;

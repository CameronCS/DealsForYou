CREATE VIEW available_cars AS SELECT * FROM car WHERE available = TRUE;

-- -------------------------------------------------------------------------------

CREATE VIEW OfferPreviewView AS
SELECT 
    o.id AS ID, 
    o.user_id AS UserId, 
    o.car_id AS CarId, 
    o.total AS Total, 
    o.offer AS Offer, 
    c.year AS Year, 
    c.make AS Make, 
    c.model AS Model, 
    c.image AS image_data, 
    c.image_name, 
    c.file_type AS FileType, 
    u.firstname, 
    u.lastname, 
    u.username 
FROM 
    offer o 
JOIN 
    car c ON o.car_id = c.id 
JOIN 
    user u ON o.user_id = u.id 
ORDER BY 
    o.car_id, 
    o.total ASC;

CREATE VIEW FullOfferView AS
SELECT 
    u.id AS UserId, 
    u.username, 
    u.firstname, 
    u.lastname, 
    u.email, 
    u.cell, 
    o.id AS OfferId, 
    o.price, 
    o.offer, 
    o.months, 
    o.interest, 
    o.monthly, 
    o.total, 
    c.id AS CarId, 
    c.make, 
    c.model, 
    c.year, 
    c.vin, 
    c.license, 
    c.price AS CarPrice, 
    c.image AS ImageData, 
    c.image_name AS ImageName, 
    c.file_type AS FileType 
FROM 
    offer o 
JOIN 
    user u ON o.user_id = u.id 
JOIN 
    car c ON o.car_id = c.id;
    
-- ---------------------------------------------------------------------------------    
    
CREATE VIEW InvoiceDetails AS
SELECT 
    u.id AS UserId, 
    u.username, 
    u.firstname, 
    u.lastname, 
    u.email, 
    u.cell,
    ao.id AS OfferId, 
    ao.price, 
    ao.offer, 
    ao.months, 
    ao.interest, 
    ao.monthly, 
    ao.total,
    c.id AS CarId, 
    c.make, 
    c.model, 
    c.year, 
    c.vin, 
    c.license, 
    c.price AS CarPrice, 
    c.image AS ImageData, 
    c.image_name AS ImageName, 
    c.file_type AS FileType,
    t.admin_id AS AdminId
FROM 
    transaction t 
JOIN 
    archived_offer ao ON t.id = ao.id 
JOIN 
    user u ON ao.user_id = u.id 
JOIN 
    car c ON ao.car_id = c.id;

-- -----------------------------------------------------------------------

CREATE VIEW CarInvoiceView AS
SELECT 
    c.make,
    c.model,
    c.year,
    i.inv_name,
    i.file,
    i.file_type
FROM 
    car c
JOIN 
    transaction t ON c.id = t.car_id
JOIN 
    invoice i ON t.id = i.trans_id;

-- ---------------------------------------------------------------------------
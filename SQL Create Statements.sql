CREATE DATABASE part3asp;
USE part3asp;

CREATE TABLE User (
    id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password VARCHAR(50) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(50) NOT NULL,
    Cell VARCHAR(50) NOT NULL,
    isAdmin BOOLEAN NOT NULL
);

CREATE TABLE car (
    id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    make VARCHAR(50) NOT NULL,
    model VARCHAR(50) NOT NULL,
    year INT NOT NULL,
    vin VARCHAR(50) NOT NULL,
    license VARCHAR(50) NOT NULL,
    price INT NOT NULL,
    image LONGBLOB NOT NULL,
    image_name VARCHAR(50) NOT NULL,
    file_type VARCHAR(50) NOT NULL,
    available BOOLEAN NOT NULL
);

CREATE TABLE balance (
    id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    total INT NOT NULL,
    current INT NOT NULL
);

CREATE TABLE transaction (
    id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    user_id INT NOT NULL,
    admin_id INT NOT NULL,
    car_id INT NOT NULL,
    balance_id INT NOT NULL,
    status VARCHAR(50) NOT NULL,
    FOREIGN KEY (user_id) REFERENCES user(id),
    FOREIGN KEY (admin_id) REFERENCES user(id),
    FOREIGN KEY (car_id) REFERENCES car(id),
    FOREIGN KEY (balance_id) REFERENCES balance(id),
    CHECK (user_id <> admin_id)
);

CREATE TABLE invoice (
    id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    trans_id INT NOT NULL,
    inv_name VARCHAR(50) NOT NULL,
    file LONGBLOB NOT NULL,
    file_type VARCHAR(50),
    FOREIGN KEY (trans_id) REFERENCES transaction(id) -- Reference the transaction
);

CREATE TABLE offer (
    id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    user_id INT NOT NULL,
    car_id INT NOT NULL,
    price INT NOT NULL,
    offer INT NOT NULL,
    months INT NOT NULL,
    interest INT NOT NULL,
    monthly INT NOT NULL,
    total INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES user(id),
    FOREIGN KEY (car_id) REFERENCES car(id)
);

-- Optionally, create an archive or history table for offers
CREATE TABLE archived_offer (
    id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
    user_id INT NOT NULL,
    car_id INT NOT NULL,
    price INT NOT NULL,
    offer INT NOT NULL,
    months INT NOT NULL,
    interest INT NOT NULL,
    monthly INT NOT NULL,
    total INT NOT NULL,
    deleted_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP, -- Keep track of when the offer was deleted
    FOREIGN KEY (user_id) REFERENCES user(id),
    FOREIGN KEY (car_id) REFERENCES car(id)
);

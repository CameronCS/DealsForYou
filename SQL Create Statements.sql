create database part3asp;
use part3asp;

create table User(
	id int auto_increment not null primary key,
    username varchar(50) not null,
    password varchar(50) not null,
    FirstName varchar(50) not null,
    LastName varchar(50) not null,
    Email varchar(50) not null,
    Cell varchar(50) not null,
    isAdmin boolean not null
);

create table car(
	id int auto_increment not null primary key,
	make varchar(50) not null,
    model varchar(50) not null,
    year int not null,
    vin varchar(50) not null,
    license varchar(50) not null,
    price int not null,
    image longblob not null,
    image_name varchar(50) not null,
    file_type varchar(50) not null,
    available boolean not null
);

create table balance (
	id int auto_increment not null primary key,
    total int not null,
    current int not null
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

create table invoice(
	id int auto_increment not null primary key,
    trans_id int not null,
    inv_name varchar(50) not null,
    file longblob not null,
    file_type varchar(50)
);

create table offer(
	id int auto_increment not null primary key,
    user_id int not null,
    car_id int not null,
    price int not null,
    offer int not null,
    months int not null,
    interest int not null,
    monthly int not null,
    total int not null,
    foreign key (user_id) references user(id),
    foreign key (car_id) references car(id)
);

CREATE VIEW available_cars AS SELECT * FROM car WHERE available = TRUE;

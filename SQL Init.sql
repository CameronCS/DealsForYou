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
    file_type varchar(50) not null
);

insert into User(username, password, firstname, lastname, email, cell, isAdmin) values("CamCS", "Password", "Cameron", "Stocks", "camcstocks@gmail.com", "0833031908", false);
insert into User(username, password, firstname, lastname, email, cell, isAdmin) values("JDoe", "Password", "John", "Doe", "jdoe@gmail.com", "0632795321", false);

insert into User(username, password, firstname, lastname, email, cell, isAdmin) values("Admin1", "Password", "Peter", "Smith", "psmith@gmail.com", "0632795321", true);

select * from user;

select * from car;

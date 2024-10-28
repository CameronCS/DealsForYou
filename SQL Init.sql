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

insert into User(username, password, firstname, lastname, email, cell, isAdmin) values("CamCS", "Password", "Cameron", "Stocks", "camcstocks@gmail.com", "0833031908", false);
insert into User(username, password, firstname, lastname, email, cell, isAdmin) values("JDoe", "Password", "John", "Doe", "jdoe@gmail.com", "0632795321", false);

insert into User(username, password, firstname, lastname, email, cell, isAdmin) values("Admin1", "Password", "Peter", "Smith", "psmith@gmail.com", "0632795321", true);

select * from user;

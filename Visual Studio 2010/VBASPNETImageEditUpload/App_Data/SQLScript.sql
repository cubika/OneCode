/***************************************************************
This is the SQL script to help to create a database called 
"db_Persons".
In this database there's only one table called "tb_personInfo" 
including three columns: 

Id (automatically increasing from 1 by 1)
PersonName(a string meaning the person's name)
PersonImage (an image, collection of bytes to show 
a photo of someone)
***************************************************************/

--Create a database after auto checking
use master
if exists (select [name] from sysdatabases where [name]
='db_Persons')
drop database db_Persons
create database db_Persons
go

--Open the database and create a table called "tb_personInfo"
use db_Persons
go
create table tb_personInfo
(
	Id int primary key identity(1,1),
	PersonName varchar(20)not null,
	PersonImage image,
	PersonImageType varchar(20)
)


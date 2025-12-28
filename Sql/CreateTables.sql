create database DynamicFormulasDB

use DynamicFormulasDB

create table tmp_data(
id int primary key identity(1,1),
a float not null,
b float not null,
c float not null,
d float not null,
e float not null,
f float not null,
g float not null,
h float not null
)

create table tmp_targil(
id int primary key identity(1,1),
targil varchar(max) not null,
tnai varchar(max) null
)

create table tmp_results(
id int primary key identity(1,1),
data_id int foreign key references tmp_data(id) not null,
targil_id int foreign key references tmp_targil(id) not null,
method varchar(20) not null,
result float,
execution_time float not null
)
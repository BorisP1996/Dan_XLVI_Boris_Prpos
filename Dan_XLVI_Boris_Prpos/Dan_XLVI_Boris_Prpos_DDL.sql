--Creating database only if database is not created yet
IF DB_ID('Zadatak_43') IS NULL
CREATE DATABASE Zadatak_43
GO
USE Zadatak_43;

if exists (SELECT name FROM sys.sysobjects WHERE name = 'tblManagers')
drop table tblManagers;

if exists (SELECT name FROM sys.sysobjects WHERE name = 'tblLevels')
drop table tblLevels;

if exists (SELECT name FROM sys.sysobjects WHERE name = 'tblSectors')
drop table tblSectors;

if exists (SELECT name FROM sys.sysobjects WHERE name = 'tblReports')
drop table tblReports;

if exists (SELECT name FROM sys.sysobjects WHERE name = 'tblEmployes')
drop table tblEmployes;


create table tblEmployes (
EmployeID int identity(1,1) primary key,
FirstName nvarchar (50) not null ,
Surname nvarchar (50) not null,
DateOfBirth datetime ,
JMBG varchar(13) unique not null, check(LEN(JMBG) = 13 and ISNUMERIC(JMBG) = 1),
Account nvarchar(20) not null,
Email nvarchar(40) not null,
Salary int not null,
Position nvarchar(30) not null,
Username nvarchar(50) not null,
Pasword nvarchar(50) not null,
)

create table tblSectors(
SectorID int identity(1,1) primary key,
SectorName nvarchar(15)
)

create table tblLevels(
LevelID int identity (1,1) primary key,
LevelType nvarchar (15)
)

create table tblManagers(
ManagerID int identity (1,1) primary key,
EmployeID int,
SectorID int,
LevelID int
)

create table tblReports(
ReportID int identity (1,1) primary key,
EmployeID int,
EmployeName nvarchar(30),
EmployeSurname nvarchar(30),
ReportDate datetime,
Project nvarchar (30),
Position nvarchar (30),
Hourst int
)


Alter Table tblManagers
Add foreign key (EmployeID) references tblEmployes(EmployeID);

Alter Table tblManagers
Add foreign key (SectorID) references tblSectors(SectorID);

Alter Table tblManagers
Add foreign key (LevelID) references tblLevels(LevelID);

Alter Table tblReports
Add foreign key (EmployeID) references tblEmployes(EmployeID);

Insert into tblSectors values ('HR'),('R&D'),('Financial');

Insert into tblLevels values ('Modify'),('ReadOnly');

Insert into tblEmployes values ('Boris','Prpos','1996-10-25','2510996160001','222333444','prposboris@hotmail.com',1200,'Position1','Username1','Pasword1'),
 ('Marko','Markovic','1994-11-23','2311994160001','222333000','markovic@hotmail.com',1400,'Position2','Username2','Pasword2'),
 ('Jovan','Jovic','1995-05-21','2105995160001','222333555','mail3@hotmail.com',1600,'Position3','Username3','Pasword3'),
 ('Maja','Majic','1999-12-25','2512999160001','222333999','mail4@hotmail.com',1100,'Position4','Username4','Pasword4'),
 ('Ana','Anic','1991-09-28','2809991160001','222333888','mail5@hotmail.com',1700,'Position5','Username5','Pasword5'),
('Milos','Kos','1992-02-25','2502992160001','222333111','mail6@hotmail.com',1900,'Position6','Username6','Pasword6')

Insert into tblManagers values (1,1,1),(2,2,2),(3,3,1)

Insert into tblReports values (5,'Ana','Anic','2020-07-02','Project1','Position1',8),(6,'Milos','Kos','2020-07-01','Project2','Position2',8),(3,'Maja','Majic','2020-07-02','Project3','Position3',8)
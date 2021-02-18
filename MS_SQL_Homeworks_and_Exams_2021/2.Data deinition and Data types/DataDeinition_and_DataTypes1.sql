-- Lesson 1 -> Data Definition and Data Types --

-- Problem 1. Create Database
CREATE DATABASE Minions

USE Minions

-- Problem 2. Create Tables
CREATE TABLE Minions(
	Id INT PRIMARY KEY NOT NULL,
	[Name] NVARCHAR(20) NOT NULL,
	Age TINYINT
	)

CREATE TABLE Towns(
	Id INT PRIMARY KEY NOT NULL,
	[Name] NVARCHAR(30) NOT NULL
	)

-- Problem 3. Alter Minions Table
ALTER TABLE Minions
ADD TownId INT NOT NULL

ALTER TABLE Minions
ADD FOREIGN KEY (TownId) REFERENCES Towns(Id)

-- or --
--ALTER TABLE Minions
--ADD TownId INT FOREIGN KEY REFERENCES Towns(Id)

-- Problem 4. Insert Records in Both Tables -> (Insert the Id manually (don’t use identity). Check the result into Judge
INSERT INTO Towns (Id, [Name])
VALUES
	(1, 'Sofia'),
	(2, 'Plovdiv'),
	(3, 'Varna')

INSERT INTO Minions (Id, [Name], Age, TownId)
VALUES
	(1, 'Kevin', 22, 1),
	(2, 'Bob', 15, 3),
	(3, 'Steward', NULL, 2)
-- ok 1/1 -- 

-- Problem 5. Truncate Table Minions
TRUNCATE TABLE Minions

-- Problem 6. Drop All Tables
DROP TABLE Minions
DROP TABLE Towns

-- Problem 7. Create Table People -> Check the result into Judge. NOTE: Allow Picture size is up to 2 MB
CREATE TABLE People(
	Id BIGINT PRIMARY KEY IDENTITY NOT NULL,
	[Name] NVARCHAR(200) NOT NULL,
	Picture VARBINARY(MAX),
	-- more correct -> Picture VARBINARY(MAX) CHECK(DATALENGTH(Picture) <= 2048 * 1024),
	Height DECIMAL(3,2),
	[Weight] DECIMAL(5,2),
	Gender CHAR(1) NOT NULL,
	Birthdate DATE NOT NULL,
	Biography NVARCHAR(MAX)
	)

INSERT INTO People ([Name], Picture, Height, [Weight], Gender, Birthdate, Biography)
VALUES
	('Ivo1', NULL, 1.81, 85.11, 'm', '2001-01-01', NULL),
	('Ivo2', NULL, 1.82, 85.12, 'm', '2001-11-02', NULL),
	('Iva3', NULL, 1.83, 85.13, 'f', '2001-01-25', NULL),
	('Iva4', NULL, 1.84, 85.14, 'f', '2001-12-10', NULL),
	('Ivo5', NULL, 1.85, 85.15, 'm', '2001-10-15', NULL)
	-- or -> ('Ivo5', NULL, 1.85, 85.15, 'm', '10-15-2001', NULL) here dateformat is MM.DD.YYYY
	-- ok 1/1 --

-- Problem 8. Create Table Users -> Check the result into Judge. NOTE: Allow Picture size is up to 900 kB
CREATE TABLE Users(
	Id BIGINT PRIMARY KEY IDENTITY NOT NULL,
	Username VARCHAR(30) UNIQUE NOT NULL,
	[Password] VARCHAR(26) NOT NULL,
	ProfilePicture VARBINARY(MAX) CHECK(DATALENGTH(ProfilePicture) <= 900 * 1024),
	LastLoginTime DATETIME2 NOT NULL,
	IsDeleted BIT NOT NULL
	)

INSERT INTO Users (Username, [Password], ProfilePicture, LastLoginTime, IsDeleted)
VALUES
	('Ivo1', 'Password1', NULL, GETDATE(), 0),
	('Ivo2', 'Password2', NULL, GETDATE(), 1),
	('Ivo3', 'Password3', NULL, GETDATE(), 0),
	('Ivo4', 'Password4', NULL, GETDATE(), 1),
	('Ivo5', 'Password5', NULL, GETDATE(), 0)
	-- or -> LastLoginTime --
	-- ('Ivo5', 'Password5', NULL, '2001-10-15', 0)
	-- ('Ivo5', 'Password5', NULL, '10-15-2001', 0)

	
-- Problem 9. Change Primary Key, remove PK and set composite PK (Id and Username)
ALTER TABLE Users
DROP CONSTRAINT PK_Id_Username

ALTER TABLE Users
ADD CONSTRAINT PK_Composite_Id_Username PRIMARY KEY (Id, Username)

-- Problem 10. Add Check Constraint -> Password fields in Users to be least 5 symbols long.
ALTER TABLE Users
ADD CONSTRAINT CHK_Users_Password_MinLength CHECK (LEN([Password]) >= 5)

INSERT INTO Users (Username, [Password], ProfilePicture, LastLoginTime, IsDeleted)
VALUES
	-- ('NoName', 'Pass', NULL, GETDATE(), 0) -- not inserted
	('NoName', 'Passw', NULL, GETDATE(), 0) -- inserted


-- Problem 11. Set Default Value of a Field
ALTER TABLE Users
ADD CONSTRAINT CK_Users_Change_LastLoginTime_To_CurrentTime DEFAULT GETDATE() FOR LastLoginTime

INSERT INTO Users (Username, [Password], ProfilePicture, IsDeleted)
VALUES
	 ('NoTime', 'Passw', NULL, 0) -- inserted


-- Problem 12. Set Unique Field -> add unique constraint to the Username field to ensure that the values there are at least 3 symbols long.
ALTER TABLE Users
DROP CONSTRAINT PK_Composite_Id_Username

ALTER TABLE Users
ADD CONSTRAINT PK_Users_AddPK_Id PRIMARY KEY (Id)

ALTER TABLE Users
ADD CONSTRAINT CHK_Users_UsernameLenght CHECK (LEN(Username) >= 3)


-- Problem 13. Movies Database -> Check the result into Judge. Using SQL queries create Movies database with the following tables. Populate each table with exactly 5 records.

CREATE DATABASE Movies

USE Movies

-- Directors (Id, DirectorName, Notes)
CREATE TABLE Directors(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	DirectorName NVARCHAR(50) NOT NULL,
	Notes NVARCHAR(MAX)
	)

INSERT INTO Directors (DirectorName)
VALUES
	('Ivo1'),
	('Ivo2'),
	('Ivo3'),
	('Ivo4'),
	('Ivo5')

-- Genres (Id, GenreName, Notes)
CREATE TABLE Genres(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	GenreName NVARCHAR(50) UNIQUE NOT NULL,
	Notes VARCHAR(MAX)
	)

INSERT INTO Genres(GenreName, Notes)
VALUES
	('Horror', NULL),
	('Comedy', NULL),
	('Triller', NULL),
	('Drama', NULL),
	('Western', 'Interesting genre')
	
-- Categories (Id, CategoryName, Notes)
CREATE TABLE Categories(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	CategoryName NVARCHAR(50) UNIQUE NOT NULL,
	Notes NVARCHAR(MAX)
	)

INSERT INTO Categories(CategoryName, Notes)
VALUES
	('Serial', NULL),
	('Sports movie', NULL),
	('Animal movie', NULL),
	('Science movie', NULL),
	('Documental movie', NULL)

SELECT * FROM Categories

-- Movies (Id, Title, DirectorId, CopyrightYear, Length, GenreId, CategoryId, Rating, Notes)
CREATE TABLE Movies(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	Title NVARCHAR(50) UNIQUE NOT NULL,
	DirectorId INT FOREIGN KEY REFERENCES Directors(Id),
	CopyrightYear DATE NOT NULL,
	[Length] TIME,
	GenreId INT FOREIGN KEY REFERENCES Genres(Id),
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
	Rating INT,
	Notes NVARCHAR(MAX)
	) 

INSERT INTO Movies(Title, DirectorId, CopyrightYear, [Length], GenreId, CategoryId, Rating, Notes)
VALUES
	('Movie1', 1, '2001-01-01', NULL, 1, 1, 100, NULL),
	('Movie2', 2, '2001-01-01', NULL, 2, 2, NULL, 'Good one'),
	('Movie3', 3, '2001-01-01', NULL, 3, 3, NULL, 'Good one'),
	('Movie4', 4, '2001-01-01', NULL, 4, 4, NULL, NULL),
	('Movie5', 5, '2001-01-01', NULL, 5, 5, NULL, 'Good one')
	-- ok 1/1 --

-- Problem 14. Car Rental Database -> Check the result into Judge. Create CarRental database with the following entities:

CREATE DATABASE CarRental

USE CarRental

-- Categories (Id, CategoryName, DailyRate, WeeklyRate, MonthlyRate, WeekendRate)

-- Cars (Id, PlateNumber, Manufacturer, Model, CarYear, CategoryId, Doors, Picture, Condition, Available)

-- Employees (Id, FirstName, LastName, Title, Notes)

-- Customers (Id, DriverLicenceNumber, FullName, Address, City, ZIPCode, Notes)

-- RentalOrders (Id, EmployeeId, CustomerId, CarId, TankLevel, KilometrageStart, KilometrageEnd, TotalKilometrage, StartDate, EndDate, TotalDays, RateApplied, TaxRate, OrderStatus, Notes)

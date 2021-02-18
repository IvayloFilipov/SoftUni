-- Problem 14. Car Rental Database -> Check the result into Judge. Create CarRental database with the following entities:

CREATE DATABASE CarRental

USE CarRental

-- Categories (Id, CategoryName, DailyRate, WeeklyRate, MonthlyRate, WeekendRate)
CREATE TABLE Categories(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	CategoryName NVARCHAR(50) NOT NULL, 
	DailyRate DECIMAL(6,2) NOT NULL,
	WeeklyRate DECIMAL(6,2) NOT NULL,
	MonthlyRate DECIMAL(6,2) NOT NULL,
	WeekendRate DECIMAL(6,2) NOT NULL
	)

INSERT INTO Categories (CategoryName, DailyRate, WeeklyRate, MonthlyRate, WeekendRate)
VALUES
	('Car', 10.10, 50.10, 150.10, 15.10),
	('Bus', 20.10, 60.10, 250.10, 25.10),
	('SUV', 15.10, 55.10, 155.10, 20.10)

-- Cars (Id, PlateNumber, Manufacturer, Model, CarYear, CategoryId, Doors, Picture, Condition, Available)
CREATE TABLE Cars(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	PlateNumber NVARCHAR(10) UNIQUE NOT NULL,
	Manufacturer NVARCHAR(30) NOT NULL,
	Model NVARCHAR(30) NOT NULL,
	CarYear DATE NOT NULL,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
	Doors TINYINT NOT NULL,
	Picture VARBINARY(MAX) CHECK(DATALENGTH(Picture) <= 2048 * 1024),
	Condition NVARCHAR(100) NOT NULL,
	Available NVARCHAR(50)
	)

INSERT INTO Cars(PlateNumber, Manufacturer, Model, CarYear, CategoryId, Doors, Picture, Condition, Available)
VALUES
	('CA 1111 AA', 'Ford', 'Ka', '12-25-2010', 1, 4, NULL, 'Good', 'Yes'),
	('CA 1111 BB', 'Ford', 'Transite', '12-25-2010', 2, 3, NULL, 'Good', 'Not till next weekend'),
	('CA 1111 CC', 'Ford', 'Kugar', '12-25-2010', 3, 4, NULL, 'Good', 'Yes')

-- Employees (Id, FirstName, LastName, Title, Notes)
CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	Title NVARCHAR(5) NOT NULL,
	Notes NVARCHAR(MAX)
	)

INSERT INTO Employees(FirstName, LastName, Title, Notes)
VALUES
	('Ivo', 'Petrov', 'CTO', NULL),
	('Ivan', 'Petrov', 'CFO', NULL),
	('Ivaylo', 'Petrov', 'Sales', NULL)

-- Customers (Id, DriverLicenceNumber, FullName, Address, City, ZIPCode, Notes)
CREATE TABLE Customers(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	DriverLicenceNumber NVARCHAR(50) NOT NULL,
	FullName NVARCHAR(100) NOT NULL,
	[Address] NVARCHAR(200),
	City NVARCHAR(50),
	ZIPCode TINYINT, -- must be INT -> 1000 or 1200 ....
	Notes NVARCHAR(MAX)
	)


INSERT INTO Customers(DriverLicenceNumber, FullName, [Address], City, ZIPCode, Notes)
VALUES
	('A123A', 'Ivo Petrov', NULL, 'Sofia', NULL, 'Regular customer'),
	('A123B', 'Ivan Petrov', NULL, 'Plovdiv', NULL, 'Not regular customer'),
	('A123C', 'Iva Petrova', NULL, 'Varna', NULL, 'New customer')

-- RentalOrders (Id, EmployeeId, CustomerId, CarId, TankLevel, KilometrageStart, KilometrageEnd, TotalKilometrage, StartDate, EndDate, TotalDays, RateApplied, TaxRate, OrderStatus, Notes)
CREATE TABLE RentalOrders(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id),
	CustomerId INT FOREIGN KEY REFERENCES Customers(Id),
	CarId INT FOREIGN KEY REFERENCES Cars(Id),
	TankLevel INT NOT NULL,
	KilometrageStart INT NOT NULL,
	KilometrageEnd INT NOT NULL,
	TotalKilometrage INT NOT NULL,
	StartDate DATE NOT NULL,
	EndDate DATE NOT NULL,
	TotalDays INT NOT NULL,
	RateApplied DECIMAL(7,2),
	TaxRate DECIMAL(4,2),
	OrderStatus NVARCHAR(30) NOT NULL,
	Notes NVARCHAR(MAX)
	)


INSERT INTO RentalOrders(EmployeeId, CustomerId, CarId, TankLevel, KilometrageStart, KilometrageEnd, TotalKilometrage, StartDate, EndDate, TotalDays, RateApplied, TaxRate, OrderStatus, Notes)
VALUES
	(1, 1, 3, 50, 10100, 10200, 100, '12-25-2010', '12-26-2010', 1, 15.10, 20.00, 'Occupied', NULL),
	(2, 3, 1, 50, 10100, 10200, 100, '12-25-2010', '12-27-2010', 2, 10.10, 20.00, 'Occupied', NULL),	
	(3, 2, 2, 50, 10100, 10200, 100, '12-25-2010', '12-26-2010', 1, 20.10, 20.00, 'Occupied', NULL)
	-- ok 1/1 --

-- NOTE:-------------------------------------------------

-- CREATE TABLE Payments(
--	AmountCharged DECIMAL(6, 2) NOT NULL,
--	TaxRate DECIMAL(6, 2) NOT NULL,
--	TaxAmount AS (AmountCharged * (TaxRate/100)),
--	PaymentTotal AS (AmountCharged * (1 + TaxRate/100))
--	)
 ---------------------------------------------------------

-- Problem 15. Hotel Database -> Check the result into Judge.
CREATE DATABASE Hotel

USE Hotel

--Employees (Id, FirstName, LastName, Title, Notes)
CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	Title NVARCHAR(30) NOT NULL,
	Notes NVARCHAR(MAX)
	)

INSERT INTO Employees(FirstName, LastName, Title, Notes)
VALUES
	('Ivo', 'Petrov', 'CTO', NULL),
	('Ivan', 'Petrov', 'CFO', NULL),
	('Iva', 'Petrova', 'GM', NULL)

--Customers (AccountNumber, FirstName, LastName, PhoneNumber, EmergencyName, EmergencyNumber, Notes)
CREATE TABLE Customers(
	AccountNumber INT PRIMARY KEY NOT NULL,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	PhoneNumber CHAR(15),
	EmergencyName NVARCHAR(90) NOT NULL,
	EmergencyNumber CHAR(15) NOT NULL,
	Notes NVARCHAR(MAX)
	)

INSERT INTO Customers(AccountNumber, FirstName, LastName, PhoneNumber, EmergencyName, EmergencyNumber, Notes)
VALUES
	(1001, 'Petko', 'Petkov', 'Phone11', 'Ganka', 'Emegrency11', NULL),
	(1002, 'Gosho', 'Petkov', 'Phone22', 'Penka', 'Emegrency22', NULL),
	(1003, 'Ivan', 'Petkov', 'Phone33', 'Stoyanka', 'Emegrency33', NULL)

--RoomStatus (RoomStatus, Notes)
CREATE TABLE RoomStatus(
	RoomStatus NVARCHAR(15) NOT NULL,
	Notes NVARCHAR(MAX)
)

INSERT INTO RoomStatus(RoomStatus, Notes)
VALUES
	('Free', NULL),
	('Not Free', NULL),
	('Cleaning', NULL)

--RoomTypes (RoomType, Notes)
CREATE TABLE RoomTypes(
	RoomType NVARCHAR(20) NOT NULL,
	Notes NVARCHAR(MAX)
	)

INSERT INTO RoomTypes(RoomType, Notes)
VALUES
	('One bed room', NULL),
	('Two beds room', NULL),
	('Three beds room', NULL)
	
--BedTypes (BedType, Notes)
CREATE TABLE BedTypes(
	BedType NVARCHAR(20) NOT NULL,
	Notes NVARCHAR(MAX)
	)

INSERT INTO BedTypes(BedType, Notes)
VALUES
	('Single', NULL),
	('Kid', NULL),
	('Family', NULL)

--Rooms (RoomNumber, RoomType, BedType, Rate, RoomStatus, Notes)
CREATE TABLE Rooms(
	RoomNumber INT PRIMARY KEY NOT NULL,
	RoomType NVARCHAR(20) UNIQUE NOT NULL,
	BedType NVARCHAR(20) NOT NULL,
	Rate DECIMAL(10,2) NOT NULL,
	RoomStatus NVARCHAR(15) NOT NULL,
	Notes NVARCHAR(MAX)
	)

INSERT INTO Rooms(RoomNumber, RoomType, BedType, Rate, RoomStatus, Notes)
VALUES
	(101, 'One bed room', 'Family', 10.10, 'Free', NULL),
	(102, 'Two beds room', 'Single', 10.10, 'Not Free', NULL),
	(103, 'Three beds room', 'Kids', 10.10, 'Cleaning', NULL)


--Payments (Id, EmployeeId, PaymentDate, AccountNumber, FirstDateOccupied, LastDateOccupied, TotalDays, AmountCharged, TaxRate, TaxAmount, PaymentTotal, Notes)
CREATE TABLE Payments(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id),
	PaymentDate DATE,
	AccountNumber INT FOREIGN KEY REFERENCES Customers(AccountNumber),
	FirstDateOccupied DATE,
	LastDateOccupied DATE,
	TotalDays INT,
	AmountCharged DECIMAL(15,2),
	TaxRate DECIMAL(15,2) NOT NULL,
	TaxAmount DECIMAL(15,2),
	--TaxAmount AS (AmountCharged * (TaxRate/100)),
	PaymentTotal DECIMAL(15,2),
	--PaymentTotal AS (AmountCharged * (1 + TaxRate/100)),
	Notes NVARCHAR(MAX)
	)

INSERT INTO Payments(EmployeeId, PaymentDate, AccountNumber, FirstDateOccupied, LastDateOccupied, TotalDays, AmountCharged, TaxRate, TaxAmount, PaymentTotal, Notes)
VALUES
	(1, '10-20-2010', 1001, '10-25-2010', '10-26-2010', 1, 10.10, 20.00, 2.02, 12.12, NULL),
	(2, '10-20-2010', 1003, '10-25-2010', '10-25-2010', 0, NULL, 20.00, NULL, NULL, NULL),
	(3, '10-20-2010', 1003, '10-25-2010', '10-25-2010', 0, NULL, 20.00, NULL, NULL, NULL)


--Occupancies (Id, EmployeeId, DateOccupied, AccountNumber, RoomNumber, RateApplied, PhoneCharge, Notes)
CREATE TABLE Occupancies(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id),
	DateOccupied DATE NOT NULL,
	AccountNumber INT FOREIGN KEY REFERENCES Customers(AccountNumber),
	RoomNumber INT FOREIGN KEY REFERENCES Rooms(RoomNumber),
	RateApplied INT,
	PhoneCharge DECIMAL(15,2),
	Notes NVARCHAR(MAX)
	)

INSERT INTO Occupancies(EmployeeId, DateOccupied, AccountNumber, RoomNumber, RateApplied, PhoneCharge, Notes)
VALUES
	(1, '10-25-2010', 1001, 101, 6, NULL, 'random note'),
	(2, '10-25-2010', 1002, 101, 5, NULL, 'random note'),
	(3, '10-25-2010', 1003, 101, 4, NULL, 'random note')
	-- ok 1/1 --

-- Problem 16. Create SoftUni Database
CREATE DATABASE SoftUni

USE SoftUni

--Towns (Id, Name)
CREATE TABLE Towns(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] NVARCHAR(90) NOT NULL
	)
	
INSERT INTO Towns([Name])
VALUES
	('Sofia'),
	('Plovdiv'),
	('Varna'),
	('Burgas')

--Addresses (Id, AddressText, TownId)
CREATE TABLE Addresses(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	AddressText NVARCHAR(100) NOT NULL,
	TownId INT FOREIGN KEY REFERENCES Towns(Id)
	)

INSERT INTO Addresses(AddressText, TownId)
VALUES
	('Sf, ul"One" №5', 1),
	('Pv, ul"One" №5', 2),
	('Vn, ul"One" №5', 3),
	('Bs, ul"One" №5', 4)


--Departments (Id, Name)
CREATE TABLE Departments(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] NVARCHAR(50) UNIQUE NOT NULL
	)

INSERT INTO Departments([Name])
VALUES
	('Engineering'),
	('Sales'),
	('Marketing'),
	('Software Development'),
	('Quality Assurance')

--Employees (Id, FirstName, MiddleName, LastName, JobTitle, DepartmentId, HireDate, Salary, AddressId)
CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName NVARCHAR(50) NOT NULL,
	MiddleName NVARCHAR(50),
	LastName NVARCHAR(50) NOT NULL,
	JobTitle NVARCHAR(30) NOT NULL,
	DepartmentId INT FOREIGN KEY REFERENCES Departments(Id),
	HireDate DATE NOT NULL,
	Salary DECIMAL(10,2) NOT NULL,
	AddressId INT FOREIGN KEY REFERENCES Addresses(Id)
	)

INSERT INTO Employees(FirstName, MiddleName, LastName, JobTitle, DepartmentId, HireDate, Salary, AddressId)
VALUES
	('Ivan', 'Ivanov', 'Ivanov', '.NET Developer', 4, '02/01/2013', 3500.00, 1),
	('Petar', 'Petrov', 'Petrov', 'Senior Engineer', 1, '03/02/2004', 4000.00, 2),
	('Maria', 'Petrova', 'Ivanova', 'Intern', 5, '08/28/2016', 525.25, 3),
	('Georgi', 'Terziev', 'Ivanov', 'CEO', 2, '12/09/2007', 3000.00, 4),
	('Peter', 'Pan', 'Pan', 'Intern', 3, '08/28/2016', 599.88, 1)


--Problem 17. Backup Database -> 
Десен бутон върху Базата -> избирам Tasks -> Back up -> в оторения прозорец сетвам Backuo type (full), после Back up to: set-vam Disc (or URL -> somewhere on cloud).

--Problem 18. Basic Insert -> USE THE Problem 16. Create SoftUni Database ABOVE -> DONE

--Problem 19.	Basic Select All Fields -> Check the result into Judge.
SELECT * FROM Towns 
SELECT * FROM Departments 
SELECT * FROM Employees 
	-- ok 1/1 --

--Problem 20. Basic Select All Fields and Order Them -> Check the result into Judge. Modify queries from previous problem by sorting:
SELECT * FROM Towns 
ORDER BY [Name] ASC

SELECT * FROM Departments 
ORDER BY [Name] ASC

SELECT * FROM Employees
ORDER BY Salary DESC
	-- ok 1/1 --


--Problem 21. Basic Select Some Fields -> Check the result into Judge.
SELECT [Name] FROM Towns 
ORDER BY [Name] ASC

SELECT [Name] FROM Departments 
ORDER BY [Name] ASC

SELECT FirstName, LastName, JobTitle, Salary FROM Employees
ORDER BY Salary DESC
	-- ok 1/1 --

--Problem 22. Increase Employees Salary -> Check the result into Judge. Increase the salary of all employees by 10%. 
UPDATE Employees
SET Salary *= 1.1

SELECT Salary FROM Employees
	-- ok 1/1 --

--Problem 23. Decrease Tax Rate -> Check the result into Judge. Use Hotel database and decrease tax rate by 3% to all payments. 
USE Hotel

UPDATE Payments
SET TaxRate = TaxRate - (TaxRate * 0.03)

SELECT TaxRate FROM Payments
	-- ok 1/1 --


--Problem 24. Delete All Records -> Check the result into Judge. Use Hotel database and delete all records from the Occupancies table. 
DELETE FROM Occupancies

SELECT * FROM Occupancies
	-- ok 1/1 --
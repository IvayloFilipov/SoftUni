-- Exercises: Table Relations --
CREATE DATABASE TableRelationsDemo

USE TableRelationsDemo

--Problem 1.One-To-One Relationship -> Check result into Judge
CREATE TABLE Passports(
	PassportID INT PRIMARY KEY IDENTITY(101, 1) NOT NULL,
	PassportNumber NVARCHAR(30) NOT NULL -- <-- or CHAR(8) if it is with fixed length and not contains unicode symbols
	)

INSERT INTO Passports (PassportNumber)
VALUES
	('N34FG21B'),
	('K65LO4R7'),
	('ZE657QP2')

CREATE TABLE Persons(
	PersonID INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName NVARCHAR(50) NOT NULL,
	Salary DECIMAL(7,2),
	PassportID INT FOREIGN KEY REFERENCES Passports(PassportID)
	)

--ALTER TABLE Persons
--ADD CONSTRAINT UNQ_PassportID UNIQUE (PassportID)

INSERT INTO Persons (FirstName, Salary, PassportID)
VALUES
	('Roberto', 43300, 102),
	('Tom', 56100, 103),
	('Yana', 60200, 101)

 -- ok 100/100 -- 

------------------second atempt to Problem 1. One-To-One Relationship ------------------------------------
CREATE TABLE Passports(
	PassportID INT PRIMARY KEY IDENTITY(101,1) NOT NULL,
	PassportNumber CHAR(8) NOT NULL
	)

CREATE TABLE Persons(
	PersonID INT NOT NULL,
	FirstName VARCHAR(50) NOT NULL,
	Salary DECIMAL(7,2) NOT NULL,
	PassportID INT FOREIGN KEY REFERENCES Passports(PassportID) UNIQUE NOT NULL
	)

ALTER TABLE Persons
ADD CONSTRAINT PK_Persons_PersonID PRIMARY KEY (PersonID) 

SELECT * FROM Persons
SELECT * FROM Passports

INSERT INTO Passports (PassportNumber)
VALUES
	('N34FG21B'),
	('K65LO4R7'),
	('ZE657QP2')

EXEC sp_rename 'Persons.PersonID', 'OldPersonID', 'COLUMN'

ALTER TABLE Persons
DROP CONSTRAINT [PK_Persons_PersonID]

ALTER TABLE Persons
DROP COLUMN OldPersonID

ALTER TABLE Persons
ADD PersonID INT PRIMARY KEY IDENTITY NOT NULL

INSERT INTO Persons (FirstName, Salary, PassportID)
VALUES
	('Roberto', 43300, 102),
	('Tom', 56100, 103),
	('Yana', 60200, 101)

--ALTER TABLE Persons
--DROP CONSTRAINT PK_Persons_PersonID

--ALTER TABLE Persons
--ADD PRIMARY KEY (PersonID)

--ALTER TABLE Persons
--DROP CONSTRAINT [PK__Persons__AA2FFB85120DF47E]


 -- compile time errror ------

------------------END of second atempt to Problem 1. One-To-One Relationship -----------------------------

-- last attempt to problem 1. One-to-One Relationship --------------

CREATE TABLE Passports(
	PassportID INT PRIMARY KEY IDENTITY(101,1) NOT NULL,
	PassportNumber CHAR(8) NOT NULL
	)

INSERT INTO Passports (PassportNumber)
VALUES
	('N34FG21B'),
	('K65LO4R7'),
	('ZE657QP2')

CREATE TABLE Persons(
	--PersonID INT IDENTITY NOT NULL, -- 66/100 --
	PersonID INT IDENTITY PRIMARY KEY NOT NULL, -- 100/100 --
	FirstName NVARCHAR(50) NOT NULL,
	Salary DECIMAL(7,2) NOT NULL,
	PassportID INT FOREIGN KEY REFERENCES Passports(PassportID) UNIQUE NOT NULL
	)

--ALTER TABLE Persons
--ADD CONSTRAINT PK_Persons_PersonID PRIMARY KEY (PersonID)

INSERT INTO Persons (FirstName, Salary, PassportID)
VALUES
	('Roberto', 43300, 102),
	('Tom', 56100, 103),
	('Yana', 60200, 101)


    ------------------END of last atempt to Problem 1. One-To-One Relationship -----------------
------------------------------------------------------------------------------------------------------


--Problem 2.One-To-Many Relationship  -> Check result into Judge
CREATE TABLE Manufacturers(
	ManufacturerID INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] NVARCHAR(30) NOT NULL,
	EstablishedOn DATE NOT NULL
	)

CREATE TABLE Models(
	ModelID INT PRIMARY KEY IDENTITY(101,1) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	ManufacturerID INT FOREIGN KEY REFERENCES Manufacturers(ManufacturerID) NOT NULL
	)

INSERT INTO Manufacturers ([Name], EstablishedOn)
VALUES
	('BMW', '1916/07/03'),
	('Tesla', '2003/01/01'),
	('Lada', '1966/05/01')
	
INSERT INTO Models ([Name], ManufacturerID)
VALUES
	('X1', 1),
	('i6', 1),
	('Model S', 2),
	('Model X', 2),
	('Model 3', 2),
	('Nova', 3)

	-- here below I made INNER JOIN on both tales (Manufacturer and Models) --

--SELECT ma.[Name], mo.[Name], ma.EstablishedOn
--FROM Models AS mo
--INNER JOIN Manufacturers AS ma
--ON mo.ManufacturerID = ma.ManufacturerID

------------------------------------------------------------------------------

--Problem 3.Many-To-Many Relationship  -> Check result into Judge
CREATE TABLE Students(
	StudentID INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] NVARCHAR(50) NOT NULL
	)

CREATE TABLE Exams(
	ExamID INT PRIMARY KEY IDENTITY(101,1) NOT NULL,
	[Name] NVARCHAR(100) NOT NULL
	)

	-- mapping table many-to-many - FULL sintax ----------
--CREATE TABLE StudentsExams(
--	StudentID INT NOT NULL,
--	ExamID INT NOT NULL,
--	CONSTRAINT PK_Composite_StudentsExams PRIMARY KEY(StudentID, ExamID),
--	CONSTRAINT FK_StudentsExams_Students FOREIGN KEY(StudentID) REFERENCES Students(StudentID),
--	CONSTRAINT FK_StudentsExams_Exams FOREIGN KEY(ExamID) REFERENCES Exams(ExamID)
--	)
--	drop table StudentsExams

	-- or mapping table many-to-many - SHORT sintax -----------
CREATE TABLE StudentsExams(
	StudentID INT FOREIGN KEY REFERENCES Students(StudentID) NOT NULL,
	ExamID INT FOREIGN KEY REFERENCES Exams(ExamID) NOT NULL,
	CONSTRAINT PK_Composite_StudentsExams PRIMARY KEY(StudentID, ExamID)
	)

	-- or mapping table many-to-many - SHORTER sintax -------------
--CREATE TABLE StudentsExams(
--	StudentID INT FOREIGN KEY REFERENCES Students(StudentID) NOT NULL,
--	ExamID INT FOREIGN KEY REFERENCES Exams(ExamID) NOT NULL,
--	PRIMARY KEY(StudentID, ExamID)
--	)

INSERT INTO Students ([Name])
VALUES
	('Mila'),
	('Toni'),
	('Ron')

INSERT INTO Exams ([Name])
VALUES
	('SpringMVC'),
	('Neo4j'),
	('Oracle 11g')

INSERT INTO StudentsExams (StudentID, ExamID)
VALUES
	(1, 101),
	(1, 102),
	(2, 101),
	(3, 103),
	(2, 102),
	(2, 103)

	-- 100/100 --

	-- here below I made JOIN on both tables (Students, Exams) ---
--SELECT s.[Name], e.[Name] 
--FROM StudentsExams AS se
--JOIN Students AS s ON se.StudentID = s.StudentID
--JOIN Exams AS e ON se.ExamID = e.ExamID

-----------------------------------------------------------------------------

--Problem 4.Self-Referencing  -> Check result into Judge
CREATE TABLE Teachers(
	TeacherID INT PRIMARY KEY IDENTITY(101,1) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	ManagerID INT FOREIGN KEY REFERENCES Teachers(TeacherID)
	)

		-- longer sintax --     OK    -- 100/100 -- 
--CREATE TABLE Teachers(
--	TeacherID INT PRIMARY KEY IDENTITY(101,1) NOT NULL,
--	[Name] NVARCHAR(50) NOT NULL,
--	ManagerID INT,		-- must accept NULL
--	CONSTRAINT FK_Teachers_ManagerIDTeacherID FOREIGN KEY (ManagerID) REFERENCES Teachers(TeacherID)
--	)

INSERT INTO Teachers ([Name], ManagerID)
VALUES
	('John', NULL),
	('Maya', 106),
	('Silvia', 106),
	('Ted', 105),
	('Mark', 101),
	('Greta', 101)

	-- 100/100 -- 


--Problem 5.Online Store Database  -> Check result into Judge
CREATE DATABASE OnlineStore

USE OnlineStore

CREATE TABLE Cities(
	CityID INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(50) NOT NULL
	)

CREATE TABLE Customers(
	CustomerID INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(50) NOT NULL,
	Birthday DATE,
	CityID INT FOREIGN KEY REFERENCES Cities(CityID) NOT NULL
	)

CREATE TABLE Orders(
	OrderID INT PRIMARY KEY IDENTITY NOT NULL,
	CustomerID INT FOREIGN KEY REFERENCES  Items(ItemID) NOT NULL
	)

CREATE TABLE ItemTypes(
	ItemTypeID INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(50) NOT NULL
	)

CREATE TABLE Items(
	ItemID INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(50) NOT NULL,
	ItemTypeID INT FOREIGN KEY REFERENCES ItemTypes(ItemTypeID) NOT NULL
	)

CREATE TABLE OrderItems(
	OrderID INT FOREIGN KEY REFERENCES Orders(OrderID) NOT NULL,
	ItemID INT FOREIGN KEY REFERENCES Items(ItemID) NOT NULL,
	CONSTRAINT PK_OrderItems_OrdersItems PRIMARY KEY (OrderId, ItemID)
	)

	-- ok  100/ 100 -- 


--Problem 6.University Database  -> Check result into Judge
CREATE DATABASE University 

USE University

CREATE TABLE Subjects(
	SubjectID INT PRIMARY KEY IDENTITY NOT NULL,
	SubjectName VARCHAR(50) NOT NULL
	)

CREATE TABLE Majors(
	MajorID INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(50) NOT NULL
	)

CREATE TABLE Students(
	StudentID INT PRIMARY KEY IDENTITY NOT NULL,
	StudentNumber VARCHAR(20) NOT NULL,
	StudentName NVARCHAR(100) NOT NULL,
	MajorID INT FOREIGN KEY REFERENCES Majors(MajorID) NOT NULL
	)

CREATE TABLE Payments(
	PaymentID INT PRIMARY KEY IDENTITY NOT NULL,
	PaymentDate DATE NOT NULL,
	PaymentAmount DECIMAL(7,2),
	StudentID INT FOREIGN KEY REFERENCES Students(StudentID) NOT NULL
	)

CREATE TABLE Agenda(
	StudentID INT FOREIGN KEY REFERENCES Students(StudentID) NOT NULL,
	SubjectID INT FOREIGN KEY REFERENCES Subjects(SubjectID) NOT NULL,
	CONSTRAINT PK_Agenda_StudentsSubjects PRIMARY KEY (StudentID, SubjectID)
	)

	-- ok  100/100  --


--Problem 7.SoftUni Design 
USE SoftUni

--Problem 8.Geography Design 


--Problem 9. *Peaks in Rila  -> Check result into Judge
USE [Geography]

SELECT m.MountainRange, p.PeakName, p.Elevation
FROM Mountains AS m
JOIN Peaks AS p ON m.Id = p.MountainId
WHERE m.Id = 17
-- or --> WHERE m.MountainRange = 'Rila'
ORDER BY p.Elevation DESC

 -- !!!	-- or better to start SELECT from Peaks -- should NOT relly on id's --
SELECT m.MountainRange, p.PeakName, p.Elevation
FROM Peaks AS p
JOIN Mountains AS m ON p.MountainId = m.Id 
WHERE m.MountainRange = 'Rila'
ORDER BY p.Elevation DESC

	-- can use logical operators after JOIN -> AND, OR, NOT (AND NOT) --
	-- JOIN Mountains AS m ON p.MountainId = m.Id  -> AND, OR (depend's on the problem) --

	-- ok 100/100 --
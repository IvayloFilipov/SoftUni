-- MS SQL Exam Ц 11 Aug 2020 --
CREATE DATABASE [Bakery]

USE Bakery

	-- Section 1. DDL --
-- 1. Database design --
CREATE TABLE Countries(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] NVARCHAR(50) UNIQUE NOT NULL
	)

CREATE TABLE Customers(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName NVARCHAR(25) NOT NULL,
	LastName NVARCHAR(25) NOT NULL,
	Gender CHAR(1) CHECK(Gender IN ('M', 'F')) NOT NULL,
	Age INT NOT NULL,
	PhoneNumber CHAR(10) NOT NULL,
	CountryId INT FOREIGN KEY REFERENCES Countries(Id) NOT NULL
	)

CREATE TABLE Products(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] NVARCHAR(25) UNIQUE NOT NULL,
	[Description] NVARCHAR(250) NOT NULL,
	Recipe NVARCHAR(max) NOT NULL,
	Price MONEY CHECK(Price >= 0) NOT NULL
	)

CREATE TABLE Feedbacks(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Description] NVARCHAR(255),
	Rate DECIMAL(4,2) CHECK(Rate BETWEEN 0 AND 10) NOT NULL,
	ProductId INT FOREIGN KEY REFERENCES Products(Id) NOT NULL,
	CustomerId INT FOREIGN KEY REFERENCES Customers(Id) NOT NULL
	)

CREATE TABLE Distributors(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] NVARCHAR(25) NOT NULL,
	AddressText NVARCHAR(30) NOT NULL,
	Summary NVARCHAR(200) NOT NULL,
	CountryId INT FOREIGN KEY REFERENCES Customers(Id) NOT NULL
	)

CREATE TABLE Ingredients(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] NVARCHAR(30) NOT NULL,
	[Description] NVARCHAR(200) NOT NULL,
	OriginCountryId INT FOREIGN KEY REFERENCES Countries(Id) NOT NULL,
	DistributorId INT FOREIGN KEY REFERENCES Distributors(Id) NOT NULL
	)

CREATE TABLE ProductsIngredients(
	ProductId INT FOREIGN KEY REFERENCES Products(Id) NOT NULL,
	IngredientId INT FOREIGN KEY REFERENCES Ingredients(Id) NOT NULL
	PRIMARY KEY(ProductId, IngredientId)
	)
	-- ok 30/30 -- 

	-- Section 2. DML --
-- 2.	Insert --
INSERT INTO Distributors([Name], AddressText, Summary, CountryId)
VALUES
('Deloitte & Touche', '6 Arch St #9757',	'Customizable neutral traveling', 2),
('Congress Title', '58 Hancock St', 'Customer loyalty', 13),
('Kitchen People', '3 E 31st St #77', 'Triple-buffered stable delivery', 1),
('General Color Co Inc', '6185 Bohn St #72',	'Focus group', 2),
('Beck Corporation', '21 E Ave	Qualityfocused 4th', 'generation hardware', 23)

INSERT INTO Customers(FirstName, LastName, Gender, Age, PhoneNumber, CountryId)
VALUES
('Francoise', 'Rautenstrauch', 'M', 15, '0195698399', 5),
('Kendra', 'Loud', 'F', 22, '0063631526', 11),
('Lourdes', 'Bauswell', 'M', 50, '0139037043', 8),
('Hannah', 'Edmison', 'F', 18, '0043343686', 1),
('Tom', 'Loeza', 'M', 31, '0144876096', 23),
('Queenie', 'Kramarczyk', 'F', 30, '0064215793', 29),
('Hiu', 'Portaro', 'M', 25, '0068277755', 16),
('Josefa', 'Opitz', 'F', 43, '0197887645', 17)
	-- 1/2 --

-- 3.	Update --
UPDATE Ingredients
SET DistributorId = 35
WHERE [Name] IN ('Bay Leaf', 'Paprika', 'Poppy')

UPDATE Ingredients
SET OriginCountryId = 14
WHERE OriginCountryId = 8
	-- ok  4/4 --

-- 4.	Delete --
--Delete all Feedbacks which relate to Customer with Id 14 or to Product with Id 5
select * from Feedbacks

DELETE FROM Feedbacks
WHERE CustomerId = 14 

DELETE FROM Feedbacks
WHERE ProductId = 5
	-- ок 4/4 --

	-- Section 3. Querying --
-- 5.	Products by Price --
SELECT p.[Name], 
	   p.Price, 
	   p.[Description] 
	FROM Products AS p
ORDER BY p.Price DESC, 
		 p.[Name] ASC
	-- ok 3/3 --

-- 6.	Negative Feedback --
--Select all feedbacks alongside with the customers which gave them. 
--Filter only feedbacks which have rate below 5.0. 
--Order results by ProductId (descending) then by Rate (ascending).
SELECT f.ProductId, f.Rate, f.[Description], f.CustomerId, c.Age, c.Gender
	FROM Feedbacks AS f
JOIN Customers AS c ON f.CustomerId = c.Id
WHERE f.Rate < 5.0
ORDER BY f.ProductId DESC, 
		 f.Rate ASC 
	-- ok 5/5 --

-- 7.	Customers without Feedback --
--Select all customers without feedbacks. Order them by customer id (ascending).
SELECT CONCAT(c.FirstName, ' ', c.LastName) AS [CustomerName], 
	   c.PhoneNumber AS PhoneNumber,
	   c.Gender AS Gender
	FROM Customers AS c
LEFT JOIN  Feedbacks AS f ON f.CustomerId = c.Id
WHERE  [Description] IS NULL OR [Description] = ''
ORDER BY f.CustomerId
	-- 3/6 --
-- or --
SELECT CONCAT(c.FirstName, ' ', c.LastName),
       c.PhoneNumber,
       c.Gender
    FROM Customers AS c
    WHERE c.Id NOT IN
          (SELECT f.CustomerId
               FROM Feedbacks AS f)
    ORDER BY c.Id
	-- ok 6/6 --

-- 8.	Customers by Criteria --
SELECT c.FirstName, c.Age, c.PhoneNumber
	FROM Customers AS c
JOIN Countries AS cnt ON c.CountryId = cnt.Id
WHERE FirstName LIKE '%an%' 
	AND Age >= 21 
	OR c.PhoneNumber = (SELECT PhoneNumber FROM Customers WHERE SUBSTRING(PhoneNumber, 9, 2) = 38)
	AND cnt.Name NOT LIKE 'Greece'
ORDER BY c.FirstName, c.Age DESC

-- 9.	Middle Range Distributors --
--Select all distributors which distribute ingredients used in the making process of all products having average rate between 5 and 8 (inclusive). 
SELECT d.[Name] AS [DistributorName], 
	   i.[Name] AS [IngredientName], 
	   p.[Name] AS [ProductName], 
	   AVG(f.Rate) AS [AvarageRate] 
	FROM Distributors AS d
JOIN Ingredients AS i ON d.Id = i.DistributorId
JOIN ProductsIngredients AS prin ON i.Id = prin.IngredientId
JOIN Products AS p ON prin.ProductId = p.Id
JOIN Feedbacks AS f ON p.Id = f.ProductId
GROUP BY d.[Name], i.[Name], p.[Name]
HAVING AVG(f.Rate) BETWEEN 5 AND 8
ORDER BY d.[Name], i.[Name], p.[Name]

	-- 7/7 --

SELECT k.DistributorName, 
	   k.IngredientName, 
	   k.ProductName, 
	   k.AvarageRate
	FROM
(SELECT d.[Name] AS DistributorName,
		i.[Name] AS IngredientName,
		p.[Name] AS ProductName,
		AVG(f.Rate) AS [AvarageRate]
	FROM ProductsIngredients AS pri
LEFT JOIN Ingredients AS i ON pri.IngredientId = i.Id
JOIN Distributors AS d ON i.DistributorId = d.Id
JOIN Products AS p ON pri.ProductId = p.Id
JOIN Feedbacks AS f ON p.Id = f.ProductId
--WHERE f.Rate BETWEEN 5 AND 8
GROUP BY d.[Name], i.[Name], p.[Name]
HAVING AVG(f.Rate) BETWEEN 5 AND 8) AS k
ORDER BY k.DistributorName, 
		 k.IngredientName, 
		 k.ProductName
	-- 7/7 --

-- 10.	Country Representative --
--Select all countries with their most active distributor (the one with the greatest number of ingredients). 
--If there are several distributors with most ingredients delivered, list them all.
--CountryName	DisributorName

SELECT k.CountryName, k.DisributorName 
	FROM 
(SELECT c.[Name] AS CountryName, 
	   d.[Name] AS DisributorName,
	   DENSE_RANK() OVER (PARTITION BY c.[Name] ORDER BY COUNT(i.Id) DESC) AS [Rank]
	FROM Countries AS c
JOIN  Distributors AS d ON c.Id = d.CountryId
LEFT JOIN Ingredients AS i ON d.Id = i.DistributorId
GROUP BY c.[Name], d.[Name]) AS k
WHERE k.[Rank] = 1
ORDER BY k.CountryName, k.DisributorName
	-- ok 12/12 --

	-- Section 4. Programmability --
-- 11.	Customers with Countries --
GO
CREATE VIEW v_UserWithCountries
AS
	SELECT CONCAT(c.FirstName, ' ', c.LastName) AS [CustomerName ],
			c.Age AS [Age],
			c.Gender AS [Gender],
			cnt.[Name] AS [CountryName]
		FROM Customers AS c
	JOIN Countries AS cnt ON c.CountryId = cnt.Id
	-- ok 10/10 --
GO

SELECT TOP (5) *
  FROM v_UserWithCountries
 ORDER BY Age

-- 12.	Delete Products -- NOT FINISHED --
GO
CREATE TABLE DeletedRecordsFromProducts(
	Id INT PRIMARY KEY NOT NULL,
	Name NVARCHAR(25) NOT NULL,
	Description NVARCHAR(250) NOT NULL,
	Recipe NVARCHAR(Max) NOT NULL,
	Price MONEY NOT NULL
	)
GO
CREATE TRIGGER tr_DeleteRelations ON Products FOR DELETE
AS
BEGIN
	DECLARE @ProductId INT = (SELECT Id FROM deleted);
	DECLARE @ProductName NVARCHAR(25) = (SELECT [Name] FROM deleted);
	DECLARE @ProductDescription NVARCHAR(250) = (SELECT [Description] FROM deleted);
	DECLARE @ProductRecipe NVARCHAR(Max) = (SELECT Recipe FROM deleted);
	DECLARE @ProductPrice MONEY = (SELECT Price FROM deleted);

	INSERT INTO DeletedRecordsFromProducts(Id, Name, Description, Recipe, Price)
	VALUES
		(@ProductId, @ProductName, @ProductDescription, @ProductRecipe, @ProductPrice)
END

DELETE FROM Products WHERE Id = 7
-- NOT FINISHED --
          -- Exercises: Built-in Functions --
-- Part I – Queries for SoftUni Database --
USE  SoftUni

--Problem 1.Find Names of All Employees by First Name -> Check the result into Judge
SELECT e.FirstName, e.LastName 
FROM Employees AS e
WHERE e.FirstName LIKE 'SA%'
--WHERE LEFT(FirstName, 2) = 'Sa'
	-- ok 1/1 --

--Problem 2.Find Names of All employees by Last Name -> Check the result into Judge
SELECT e.FirstName, e.LastName 
FROM Employees AS e
WHERE e.LastName LIKE '%ei%'
	-- ok 1/1 --

--Problem 3.Find First Names of All Employees -> Check the result into Judge

--SELECT e.FirstName
--FROM Employees AS e
--WHERE DATEPART(YEAR, HireDate) BETWEEN 1995 AND 2005
--AND e.DepartmentID IN (3, 10)
	-- ok 1/1 --

SELECT e.FirstName
FROM Employees AS e
WHERE e.DepartmentID IN (3, 10) AND
DATEPART(YEAR, HireDate) BETWEEN 1995 AND 2005
--YEAR(HireDate) BETWEEN 1995 AND 2005
	-- ok 1/1 --	

--Problem 4.Find All Employees Except Engineers -> Check the result into Judge
SELECT e.FirstName, e.LastName 
FROM Employees AS e
WHERE e.JobTitle NOT LIKE '%engineer%'
	-- ok 1/1 --

--Problem 5.Find Towns with Name Length -> Check the result into Judge
SELECT t.[Name] FROM Towns AS t
WHERE LEN(t.[Name]) IN (5, 6)
ORDER BY t.[Name] ASC
-- or ->
SELECT [Name] FROM Towns 
WHERE LEN([Name]) IN (5, 6)
--WHERE LEN([Name]) BETWEEN 5 AND 6
ORDER BY [Name] ASC
	-- ok 1/1 --

--Problem 6.Find Towns Starting With -> Check the result into Judge
SELECT * FROM Towns
WHERE [Name] LIKE 'M%' OR [Name] LIKE 'K%' OR [Name] LIKE 'B%' OR [Name] LIKE 'E%'
ORDER BY [Name] ASC

-- or shorter sintax --
SELECT * FROM Towns
WHERE LEFT([Name], 1) IN ('M', 'K', 'B', 'E')
--WHERE SUBSTRING([Name], 1, 1) IN ('M', 'K', 'B', 'E')
ORDER BY [Name] ASC

SELECT * FROM Towns
WHERE [Name] LIKE '[MKBE]%'
ORDER BY [Name] ASC
	-- ok 1/1 --

--Problem 7.Find Towns Not Starting With -> Check the result into Judge
SELECT * FROM Towns
WHERE [Name] NOT LIKE 'R%' AND [Name] NOT LIKE 'B%' AND [Name] NOT LIKE 'D%'
ORDER BY [Name] ASC

-- or shorter sintax --
SELECT * FROM Towns
WHERE [Name] NOT LIKE '[RBD]%'
ORDER BY [Name] ASC

-- or use [^rbd]%  --
SELECT * FROM Towns
WHERE [Name] LIKE '[^RBD]%'
ORDER BY [Name] ASC
	-- ok 1/1 --

--Problem 8.Create View Employees Hired After 2000 Year -> Check the result into Judge
GO
CREATE VIEW [V_EmployeesHiredAfter2000] AS
(SELECT FirstName, LastName FROM Employees
WHERE DATEPART(YEAR, HireDate) > 2000)
GO
	-- ok 1/1 --

--Problem 9.Length of Last Name -> Check the result into Judge
SELECT FirstName, LastName 
FROM Employees
WHERE LEN(LastName) = 5

	-- ok 1/1 --

--Problem 10.Rank Employees by Salary  -> Check the result into Judge
SELECT EmployeeID, FirstName, LastName, Salary,
DENSE_RANK() OVER(PARTITION BY Salary ORDER BY EmployeeID) AS [Rank]
-- or --DENSE_RANK() OVER(PARTITION BY Salary ORDER BY FirstName DESC) AS [Rank]
-- or --DENSE_RANK() OVER(PARTITION BY Salary ORDER BY LastName DESC) AS [Rank]
FROM Employees
WHERE Salary BETWEEN 10000 AND 50000
ORDER BY Salary DESC
	-- ok 1/1 --

--Problem 11.Find All Employees with Rank 2 *  -> Check the result into Judge
SELECT * FROM
	(
	SELECT EmployeeID, FirstName, LastName, Salary,
	DENSE_RANK() OVER(PARTITION BY Salary ORDER BY EmployeeID) AS [Rank]
	FROM Employees
	WHERE Salary BETWEEN 10000 AND 50000
	) AS tempResult
WHERE tempResult.[Rank] = 2
ORDER BY tempResult.Salary DESC
	-- ok 1/1 --

-- or use WITH...AS --------
WITH MyEmploees AS
	(SELECT EmployeeID, FirstName, LastName, Salary,
		DENSE_RANK() OVER(PARTITION BY Salary ORDER BY EmployeeID) AS [Rank]
	FROM Employees)

SELECT * FROM MyEmploees
WHERE Salary BETWEEN 10000 AND 50000
	  AND [Rank] = 2
ORDER BY Salary DESC
-----------------------------

	---- Part II – Queries for Geography Database  ------
USE [Geography]

--Problem 12.Countries Holding ‘A’ 3 or More Times  -> Check the result into Judge
SELECT CountryName AS [CountryName], IsoCode AS [Iso Code]
FROM Countries
WHERE CountryName LIKE '%a%a%a%'
--WHERE LEN(CountryName) - LEN(REPLACE(CountryName, 'a', '')) >= 3
ORDER BY IsoCode ASC
	-- ok 1/1 --

--Problem 13.Mix of Peak and River Names  -> Check the result into Judge

	-- this is multiple SELECT from to tables that  they do not have anything in common --
	-- whithout using JOIN operator --
SELECT p.PeakName, r.RiverName, LOWER(CONCAT(p.PeakName, SUBSTRING(r.RiverName, 2, LEN(r.RiverName)))) AS Mix
FROM Peaks AS p, Rivers AS r
WHERE RIGHT(p.PeakName, 1) = LEFT(r.RiverName, 1)
ORDER BY Mix ASC
	-- OK -> 1/1 --> the resul is the same as thurd variant

--- second variant -------
SELECT p.PeakName, r.RiverName, CONCAT(p.PeakName, SUBSTRING(r.RiverName,2,LEN(r.RiverName))) AS Mix
FROM Peaks AS p
JOIN Rivers AS r ON p.PeakName = r.RiverName
WHERE p.PeakName LIKE '_%' = r.RiverName LIKE '%_' -- <- INCORRECT sintax ( = )
ORDER BY Mix ASC
	-- NO -> 0/1 --

--- third variat ----
SELECT p.PeakName, r.RiverName, CONCAT(LOWER(p.PeakName), SUBSTRING((LOWER(r.RiverName)), 2, LEN(r.RiverName))) AS Mix
FROM Peaks AS p
JOIN Rivers AS r ON RIGHT(p.PeakName, 1) = LEFT(r.RiverName, 1)
ORDER BY Mix ASC
	-- YES -> 1/1 -- :)


	----- Part III – Queries for Diablo Database ----
USE [Diablo]

--Problem 14.Games from 2011 and 2012 year  -> Check the result into Judge
SELECT TOP(50) [Name], FORMAT([Start], 'yyyy-MM-dd') AS [Start] 
FROM Games
WHERE DATEPART(YEAR, [Start]) IN (2011, 2012)
ORDER BY [Start], [Name]
	-- ok 1/1 --

--Problem 15.User Email Providers  -> Check the result into Judge
SELECT Username, SUBSTRING(Email, CHARINDEX('@', Email) + 1, LEN(Email)) AS [Email Provider] FROM Users
ORDER BY [Email Provider] ASC,
		 Username ASC
	-- ok 1/1 --

--Problem 16.Get Users with IPAdress Like Pattern   -> Check the result into Judge
SELECT Username, IpAddress FROM Users
WHERE IpAddress LIKE '[1-9][0-9][0-9][.][1]%[0-9][.]%[0-9][.][0-9][0-9][0-9]'
ORDER BY Username
	-- ok 1/1 --

	-- or --
SELECT Username, IpAddress FROM Users
WHERE IpAddress LIKE '[1-9][0-9][0-9][.][1]%[.]%[.][0-9][0-9][0-9]' -- <- ÎÊ !!! --
--WHERE IpAddress LIKE '___.1_%._%.___'    -- <- OK !!! -- 
ORDER BY Username
	-- ok 1/1 --


--Problem 17.Show All Games with Duration and Part of the Day   -> Check the result into Judge
SELECT [Name] AS Game,
(CASE
	WHEN DATEPART(HOUR, [Start]) BETWEEN 0 AND 11 THEN 'Morning'
	WHEN DATEPART(HOUR, [Start]) BETWEEN 12 AND 17 THEN 'Afternoon'
	ELSE 'Evening'
END) AS [Part of the Day], 
(CASE
	WHEN Duration <= 3 THEN 'Extra Short'
	WHEN Duration BETWEEN 4 AND 6 THEN 'Short'
	WHEN Duration > 6 THEN 'Long'
	ELSE 'Extra Long'
END) AS [Duration]
FROM Games
ORDER BY [Name],
	 [Duration],
	 [Part of the day]
	-- ok 1/1 --


	---- Part IV – Date Functions Queries ----
USE Orders

--Problem 18.Orders Table  -> Check the result into Judge
SELECT ProductName, 
	   OrderDate, 
	   DATEADD(DAY, 3, OrderDate) AS [Pay Due],
	   DATEADD(MONTH, 1, OrderDate) AS [Deliver Due]
FROM Orders
	-- ok 1/1 --

--Problem 19.People Table
CREATE TABLE People(
	Id INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) UNIQUE NOT NULL,
	Birthdate DATETIME NOT NULL
	)

INSERT INTO People ([Name], Birthdate)
VALUES
	('Victor', '2000-12-07 00:00:00.000'),
	('Steven', '1992-09-10 00:00:00.000'),
	('Stephen', '1910-09-19 00:00:00.000'),
	('John', '2010-01-06 00:00:00.000')


SELECT [Name], 
		DATEDIFF(YEAR, Birthdate, GETDATE()) AS [Age in Years],
		DATEDIFF(MONTH, Birthdate, GETDATE()) AS [Age in Months],
		DATEDIFF(DAY, Birthdate, GETDATE()) AS [Age in Days],
		DATEDIFF(MINUTE, Birthdate, GETDATE()) AS [Age in Minutes]
FROM People

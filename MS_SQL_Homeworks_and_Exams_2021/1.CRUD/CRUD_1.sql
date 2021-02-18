-- Part I – Queries for SoftUni Database --
-- NOTE: Order of queries is not random. It is DEFINED IN STRICT position one after another.
-- check for example: https://www.sisense.com/blog/sql-query-order-of-operations/ 

USE SoftUni

--2.Find All Information About Departments -> Check result into Judge
SELECT * FROM Departments
	-- ok 1/1 --

--3.Find all Department Names -> Check result into Judge
SELECT [Name] FROM Departments
	-- ok 1/1 --

--4.Find Salary of Each Employee -> Check result into Judge
SELECT FirstName, LastName, Salary FROM Employees
	-- ok 1/1 --

--5.Find Full Name of Each Employee -> Check result into Judge
SELECT [FirstName], [MiddleName], [LastName] FROM Employees
	-- ok 1/1 --

--6.Find Email Address of Each Employee -> Check result into Judge
SELECT (FirstName + '.' + LastName + '@softuni.bg') AS [Full Email Address] 
	FROM Employees
	-- or using build in function -> CONCAT(string1, string2, ...., string_n) --
SELECT CONCAT(FirstName, '.', LastName, '@', 'softuni.bg') 
	FROM Employees
	-- ok 1/1 --

--7.Find All Different Employee’s Salaries -> Check result into Judge
SELECT DISTINCT Salary FROM Employees
	-- ok 1/1 --

--8.Find all Information About Employees -> Check result into Judge
SELECT * FROM Employees
	WHERE JobTitle = 'Sales Representative'
	-- ok 1/1 --

--9.Find Names of All Employees by Salary in Range -> Check result into Judge
SELECT FirstName, LastName, JobTitle 
	FROM Employees
	WHERE Salary >= 20000 AND Salary <= 30000
	-- or better use BETWEEN operator --
SELECT FirstName, LastName, JobTitle 
	FROM Employees
	WHERE Salary BETWEEN 20000 AND 30000
	-- ok 1/1 --

--10.Find Names of All Employees -> Check result into Judge
SELECT CONCAT(FirstName, ' ', MiddleName, ' ', LastName) AS [Full Name] 
	FROM Employees
	WHERE Salary = 25000 OR Salary = 14000 OR Salary = 12500 OR Salary = 23600

	-- or better use IN operator --
--SELECT CONCAT(FirstName, ' ', MiddleName, ' ', LastName) AS [Full Name]
SELECT CONCAT(FirstName, ' ',  ISNULL(MiddleName, ''), ' ', LastName) AS [Full Name] 
	FROM Employees
	WHERE Salary IN (25000, 14000, 12500, 23600)
		-- ok 1/1 --

		-- NOTE: NULL + ' ' = ''; -> null plus space is equal to empty string
		-- or
		-- CONCAT(FirstName, ' ', (MiddleName + ' '), LastName) AS [Full Name]
		-- NOTE: MiddleName(if NULL) + ' ' = '';

--11.Find All Employees Without Manager -> Check result into Judge
SELECT FirstName, LastName 
	FROM Employees
	WHERE ManagerID IS NULL
	--NOTE: can use IS NOT NULL;
		-- ok 1/1 --

--12.Find All Employees with Salary More Than 50000	 -> Check result into Judge
SELECT FirstName, LastName, Salary FROM Employees
	WHERE Salary > 50000
	ORDER BY Salary DESC
		-- ok 1/1 --

--13.Find 5 Best Paid Employees -> Check result into Judge
SELECT TOP(5) FirstName, LastName FROM Employees
	ORDER BY Salary DESC
	-- NOTE: take only 'n'-count rows after TOP(n) --
SELECT FirstName, LastName FROM Employees
	ORDER BY Salary DESC
	OFFSET 5 ROWS
	FETCH NEXT 10 ROWS ONLY
		-- ok 1/1 --

--14.Find All Employees Except Marketing -> Check result into Judge
SELECT FirstName, LastName FROM Employees
	WHERE DepartmentID != 4
		-- ok 1/1 --

--15.Sort Employees Table -> Check result into Judge
SELECT * FROM Employees
	ORDER BY 
		Salary DESC,
		FirstName ASC,
		LastName DESC,
		MiddleName ASC
		-- ok 1/1 --

--16.Create View Employees with Salaries -> Check result into Judge
GO
CREATE VIEW v_EmployeesSalaries AS
	(SELECT FirstName, LastName, Salary FROM Employees)
GO
		-- ok 1/1 --
--SELECT * FROM v_EmployeesSalaries


--17.Create View Employees with Job Titles -> Check result into Judge
/*
CREATE VIEW V_EmployeeNameJobTitle AS
	(SELECT 
		CONCAT(FirstName, ' ', (MiddleName + ' '), LastName) AS [Full Name], 
			   JobTitle 
	FROM Employees)
*/
	-- in Judge -> 0/1 --


--NOTE: use ISNULL(expression, value) operator -> returns a specified value if the expression is NULL --
GO
CREATE VIEW V_EmployeeNameJobTitle AS
	(SELECT 
		CONCAT(FirstName, ' ', ISNULL(MiddleName, ''), ' ', LastName) AS [Full Name], 
			   JobTitle 
	FROM Employees)
GO
	-- ok 1/1 --
--SELECT [Full Name] FROM V_EmployeeNameJobTitle

--18.Distinct Job Titles -> Check result into Judge
SELECT DISTINCT JobTitle FROM Employees
	-- ok 1/1 --

--19.Find First 10 Started Projects	 -> Check result into Judge
SELECT TOP(10) * FROM Projects
	ORDER BY 
		StartDate ASC,
		[Name] ASC
	-- ok 1/1 --

--20.Last 7 Hired Employees -> Check result into Judge
SELECT TOP (7) FirstName, LastName, HireDate FROM Employees
	ORDER BY HireDate DESC
	-- ok 1/1 --

--21.Increase Salaries -> Check result into Judge
GO
CREATE VIEW v_IncreaseSalaryInSomeDepartments AS
SELECT * FROM Employees 
WHERE DepartmentID IN (1, 2, 4, 11)
GO

DROP VIEW v_IncreaseSalaryInSomeDepartments

----------------------------------
-- Submite in Judge both queries --> UPDATE and SELECT
UPDATE Employees
--SET Salary += Salary * 0.12
SET Salary *= 1.12
WHERE DepartmentID IN (1, 2, 4, 11)

SELECT Salary FROM Employees
-----------------------------------
-- ok 1/1 --

-- Back up database --
--> right click on the desired Database/Tasks/Back up.../(in the open window chose: Backup type->full, Backup to->disk)/OK

-- Restore SoftUni DB --
--> right click on Database/Restore Database/select SoftUni from drop menu/ok/ok


---------------- END of the Problem 21. -------------------------------------

--Additionaly Solved Problem: --
SELECT SUM(Salary) AS [SumSalary] 
	FROM
		(SELECT TOP(5) Salary FROM Employees
		ORDER BY Salary DESC) 
			AS [TopFiveSalary]


-- Part II – Queries for Geography Database --
USE [Geography]

--22.All Mountain Peaks -> Check result into Judge
SELECT PeakName FROM Peaks 
	ORDER BY PeakName ASC
	-- ok 1/1 --

--23.Biggest Countries by Population -> Check result into Judge
SELECT TOP(30) CountryName, [Population] FROM Countries
	WHERE ContinentCode = 'EU'
	ORDER BY 
		[Population] DESC,
		CountryName ASC
	-- ok 1/1 --

--24. *Countries and Currency (Euro / Not Euro)	 -> Check result into Judge
SELECT CountryName, CountryCode, 
	(CASE
		WHEN CurrencyCode = 'EUR' THEN 'Euro'
		ELSE 'Not Euro'
	END) AS [Currency]
FROM Countries
ORDER BY CountryName ASC
	-- ok 1/1 --


----- Part III – Queries for Diablo Database ---
USE Diablo

--25.All Diablo Characters -> Check result into Judge
SELECT [Name] FROM Characters
	ORDER BY [Name] ASC
	-- ok 1/1 --
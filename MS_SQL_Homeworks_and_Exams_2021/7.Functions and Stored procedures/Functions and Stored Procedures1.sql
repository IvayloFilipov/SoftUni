-- Exercises: Functions and Stored Procedures --

	-- Queries for SoftUni Database --
USE SoftUni
GO

-- Problem 1. Employees with Salary Above 35000 -> check result into Judge 
CREATE PROC usp_GetEmployeesSalaryAbove35000
AS
BEGIN
	SELECT FirstName, LastName 
		FROM Employees
	WHERE Salary > 35000
END
	-- ok 100/100 --
EXEC usp_GetEmployeesSalaryAbove35000

GO 
-- Problem 2. Employees with Salary Above Number -> check result into Judge 
CREATE PROC usp_GetEmployeesSalaryAboveNumber(@MinSalary DECIMAL(18,4))
AS
BEGIN
	SELECT FirstName, LastName 
		FROM Employees
	WHERE Salary >= @MinSalary
END
	-- ok 100/100 --
EXEC usp_GetEmployeesSalaryAboveNumber 48100

GO
-- Problem 3. Town Names Starting With -> check result into Judge 
CREATE PROC usp_GetTownsStartingWith(@townName VARCHAR(50))
AS
BEGIN
	DECLARE @stringSearch VARCHAR(50)
	SET @stringSearch = @townName + '%'

	SELECT [Name] AS [Town] 
		FROM Towns
	WHERE [Name] LIKE @stringSearch
END
	-- ok 100/100 --

GO
-- or short sintax ---------
CREATE PROC usp_GetTownsStartingWith(@townName VARCHAR(50))
AS
BEGIN
	SELECT [Name] AS [Town] 
		FROM Towns
	WHERE [Name] LIKE @townName + '%'
END

GO
-- or use LEFT() built-in function ---------
CREATE PROC usp_GetTownsStartingWith(@townName VARCHAR(50))
AS
BEGIN
	DECLARE @stringCount INT = LEN(@townName)
	SELECT [Name] AS [Town] 
		FROM Towns
	WHERE LEFT([Name], @stringCount) = @townName
END
	-- ok 100/100 --
EXEC usp_GetTownsStartingWith 'b'

GO
-- Problem 4. Employees from Town -> check result into Judge 
CREATE PROC usp_GetEmployeesFromTown(@townName VARCHAR(50))
AS
BEGIN
	SELECT e.FirstName, e.LastName 
		FROM Employees AS e
	JOIN Addresses AS a ON e.AddressID = a.AddressID
	JOIN Towns AS t ON a.TownID = t.TownID
	WHERE t.[Name] = @townName
END
	-- ok 100/100 --
EXEC usp_GetEmployeesFromTown 'Sofia'

GO
-- Problem 5. Salary Level Function -> check result into Judge 
CREATE FUNCTION ufn_GetSalaryLevel(@salary DECIMAL(18,4))
RETURNS VARCHAR(10)
AS
BEGIN
	DECLARE @salaryLevel VARCHAR(10);

	IF(@salary < 30000)
		BEGIN
			SET @salaryLevel = 'Low';
		END
	ELSE IF(@salary BETWEEN 30000 AND 50000)
		BEGIN
			SET @salaryLevel = 'Average';
		END
	ELSE IF(@salary > 50000)
		BEGIN
			SET @salaryLevel = 'High';
		END

	RETURN @salaryLevel;
END
	-- ok 100/100 --
GO
SELECT FirstName, LastName, Salary, 
	   [dbo].[ufn_GetSalaryLevel](Salary) AS [Salary Level] 
	FROM Employees

GO
-- Problem 6. Employees by Salary Level -> check result into Judge 
CREATE PROC usp_EmployeesBySalaryLevel(@salaryLevel VARCHAR(10))
AS
BEGIN
	SELECT FirstName AS [FIrst Name],
		   LastName AS [Last Name]
	FROM Employees
	WHERE @salaryLevel = [dbo].[ufn_GetSalaryLevel](Salary)
END
	-- ok 100/100 --
GO
EXEC usp_EmployeesBySalaryLevel 'high'

GO
-- Problem 7. Define Function -> check result into Judge 
CREATE FUNCTION ufn_IsWordComprised(@setOfLetters NVARCHAR(max), @word NVARCHAR(50))
RETURNS BIT
AS
BEGIN
	DECLARE @res BIT = 1;
	DECLARE @counter INT = 1

	WHILE(@counter <= LEN(@word))
		BEGIN
			DECLARE @currLetter CHAR -- = SUBSTRING(@word, @counter, 1);
			SET @currLetter = SUBSTRING(@word, @counter, 1);

			DECLARE @charInSet INT = CHARINDEX(@currLetter, @setOfLetters);

			IF(@charInSet = 0)
				BEGIN
					SET @res = 0
					RETURN @res;
					--RETURN 0;
				END
			ELSE
				BEGIN
					SET @counter += 1;
				END
		END

	RETURN @res;
	--RETURN 1;
END
	-- ok 100/100 --
GO
SELECT dbo.ufn_IsWordComprised('oistmiahf', 'Sofia') AS [Result] --> 1
SELECT dbo.ufn_IsWordComprised('oistmiahf', 'halves') AS [Result] --> 0

GO
-- Problem 8. * Delete Employees and Departments -> check result into Judge 
CREATE PROC usp_DeleteEmployeesFromDepartment (@departmentId INT)
AS
BEGIN
	-- Start with: create query that returns all employee ID's whitch are into given deprtment to be deleted and use it below

	-- 1. First delete all records from EmployeesProjects where EmployeeID's are part of the deprtment to be deleted
	DELETE FROM EmployeesProjects
	WHERE EmployeeID IN (
						SELECT EmployeeID FROM Employees
						WHERE DepartmentID = @departmentId
						)

	-- 2. set ManagerID to NULL into table Employees (where ManagerID is manager on the employees to be deleted ffrom the given department) first check whether the column ManagerID allows NULL
	UPDATE Employees
	SET ManagerID = NULL
	WHERE ManagerID IN (
						SELECT EmployeeID FROM Employees
						WHERE DepartmentID = @departmentId
					   )

	-- 3. set ManagerID column to receive NULL into table Departments (where DepartmentID = given department) first check (using Design) whether the column ManagerID allows NULL, if NOT, ALTER column to SET it's constraint to allow NULL
	ALTER TABLE Departments
	ALTER COLUMN ManagerID INT NULL --(was NOT NULL)

	-- 4. set ManagerID to NULL where Manager is an Employee who is going to be deleted
	UPDATE Departments
	SET ManagerID = NULL
	WHERE ManagerID IN (
						SELECT EmployeeID FROM Employees
						WHERE DepartmentID = @departmentId
					   )

	-- 5. delete employees from Employees (by the given DepartmentID)
	DELETE FROM Employees
	WHERE DepartmentID = @departmentId

	-- 6. delete given department from Departments
	DELETE FROM Departments
	WHERE DepartmentID = @departmentId

	-- 7. Select the number of employees from the given department (must return 0)
	SELECT COUNT(*) AS [EmployeesCount] FROM Employees
	WHERE DepartmentID = @departmentId

END
	-- ok 100/100 --

--EXEC usp_DeleteEmployeesFromDepartment 1

--DROP PROC [dbo].[usp_DeleteEmployeesFromDepartment]

GO
	-- Queries for Bank Database -----
USE Bank

GO
-- Problem 9. Find Full Name -> check result into Judge 
CREATE PROC usp_GetHoldersFullName 
AS
BEGIN 
	SELECT CONCAT(FirstName, ' ', LastName) AS [Full Name] FROM AccountHolders AS ah
	--JOIN Accounts AS a ON ah.Id = a.Id -- work with this JOIN statement too
END
	-- ok 100/100 --

GO
-- Problem 10. People with Balance Higher Than -> check result into Judge
CREATE PROC usp_GetHoldersWithBalanceHigherThan(@Number MONEY)
AS
BEGIN
SELECT FirstName AS [First Name], LastName AS [Last Name] 
	FROM Accounts AS a
		JOIN AccountHolders AS ah ON ah.Id = a.AccountHolderId
		GROUP BY FirstName, LastName
		--GROUP BY ah.Id, FirstName, LastName -- or GROUP BY ah.Id, FirstName, LastName
		HAVING SUM(Balance) >= @Number
	ORDER BY FirstName,
			 LastName
END
	-- ok 100/100 --

GO
-- Problem 11. Future Value Function -> check result into Judge
CREATE FUNCTION ufn_CalculateFutureValue(@Sum DECIMAL(18,4), @YearlyInterestRate FLOAT, @NumberOfYears  INT)
RETURNS DECIMAL(18,4)
AS
BEGIN 
	DECLARE @futureValue DECIMAL(18,4) 

	SET @futureValue = @Sum * (POWER((1 + @YearlyInterestRate), @NumberOfYears))

	RETURN @futureValue;
END
	-- ok 100/100 --
GO
SELECT  dbo.ufn_CalculateFutureValue (1000, 0.1, 5) AS [Output]

GO
-- Problem 12. Calculating Interest for next 5 years -> check result into Judge
CREATE PROC usp_CalculateFutureValueForAccount (@AccountId INT, @InterestRate FLOAT)
AS
BEGIN 
	SET @AccountId = 1;
	
	SET @interestRate = 0.1;

	DECLARE @numberOfYears INT;
	SET @numberOfYears = 5;

	SELECT TOP(1) ah.Id AS [Account Id], 
				  ah.FirstName AS [FirstName], 
				  ah.LastName AS [Last Name], 
				  a.Balance AS [Curret Balance],
				  dbo.ufn_CalculateFutureValue (a.Balance, @interestRate, @numberOfYears)
				FROM Accounts AS a
	JOIN AccountHolders AS ah ON a.AccountHolderId = ah.Id
	WHERE ah.Id = @AccountId

END
	-- ok 100/100 --

GO
	-- Queries for Diablo Database --
USE Diablo

GO
-- Problem 13. *Scalar Function: Cash in User Games Odd Rows -> check result into Judge
-- This is Table-valued-function, returns TABLE -- 
-- after AS do not use BEGIN/END statement, just RETURN --

CREATE FUNCTION ufn_CashInUsersGames (@GameName NVARCHAR(50))
RETURNS TABLE
AS
RETURN
	  (
	   SELECT SUM(k.Cash) AS [SumCash] 
	  	 FROM
	  	   (
		     SELECT g.[Name], 
	  	    	       ug.Cash,
	  	    	       ROW_NUMBER() OVER(PARTITION BY g.[Name] ORDER BY ug.Cash DESC) AS [Order]
	  	     FROM Games AS g
	  	     JOIN UsersGames AS ug ON g.Id = ug.GameId
	  	     GROUP BY [Name], Cash
		    ) AS k
	  	WHERE k.[Order] % 2 != 0 AND k.[Name] LIKE @GameName
	  )
--	-- ok 100/100 --
--GO


-- or without GROUP BY -- 
--CREATE FUNCTION ufn_CashInUsersGames (@GameName NVARCHAR(50))
--RETURNS TABLE
--AS
--RETURN
--	  (
--	  SELECT SUM(k.Cash) AS [SumCash] 
--	  	FROM
--	  		(SELECT g.[Name], 
--	  			   ug.Cash,
--	  			   ROW_NUMBER() OVER(PARTITION BY g.[Name] ORDER BY ug.Cash DESC) AS [Order]
--	  			FROM Games AS g
--	  		JOIN UsersGames AS ug ON g.Id = ug.GameId 
--	  		WHERE g.[Name] LIKE @GameName
--			) AS k
--	   WHERE k.[Order] % 2 != 0
--	   )
	-- ok 100/100 --
GO

SELECT * FROM [dbo].[ufn_CashInUsersGames] ('Love in a mist')
GO

-- Problem  -> check result into Judge
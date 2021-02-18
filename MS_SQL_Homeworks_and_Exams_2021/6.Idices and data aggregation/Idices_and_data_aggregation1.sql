	-- Exercises: Indices and Data Aggregation --

USE Gringotts 
--Problem 1. Records’ Count -> check result into Judge --
SELECT COUNT(*) AS [Count] 
	FROM WizzardDeposits
	-- ok 100/100 --

--Problem 2. Longest Magic Wand -> check result into Judge --
SELECT MAX(MagicWandSize) AS [LongestMagicWand]
FROM WizzardDeposits
--MIN or AVG(MagicWandSize)
	-- ok 100/100 --

--Problem 3. Longest Magic Wand Per Deposit Groups -> check result into Judge --
SELECT DepositGroup, MAX(MagicWandSize) AS [LongestMagicWand]
	FROM WizzardDeposits
GROUP BY DepositGroup
	-- ok 100/100 --

--Problem 4. *Smallest Deposit Group Per Magic Wand Size -> check result into Judge --
SELECT TOP (2) Res.DepositGroup FROM
(SELECT DepositGroup, AVG(MagicWandSize) AS [AvgM]
	FROM WizzardDeposits
GROUP BY DepositGroup) AS [res]
ORDER BY res.AvgM ASC, 
	 res.DepositGroup ASC
	-- ok 100/100 --
-- or --
SELECT TOP (2) DepositGroup
	FROM WizzardDeposits
GROUP BY DepositGroup
ORDER BY AVG(MagicWandSize) ASC

--Problem 5. Deposits Sum -> check result into Judge --
SELECT DepositGroup, SUM(DepositAmount) AS [TotalSum]
	FROM WizzardDeposits
GROUP BY DepositGroup
	-- ok 100/100 --

--Problem 6. Deposits Sum for Ollivander Family -> check result into Judge --
SELECT DepositGroup, SUM(DepositAmount) AS [TotalSum]
	FROM WizzardDeposits
WHERE MagicWandCreator LIKE 'Ollivander family'
GROUP BY DepositGroup
	-- ok 100/100 --

--Problem 7. Deposits Filter -> check result into Judge --
SELECT DepositGroup, SUM(DepositAmount) AS [TotalSum] 
	FROM WizzardDeposits
WHERE MagicWandCreator LIKE 'Ollivander family'
GROUP BY DepositGroup
HAVING SUM(DepositAmount) < 150000
ORDER BY [TotalSum] DESC
	-- ok 100/100 --

--Problem 8. Deposit Charge -> check result into Judge --
SELECT res.DepositGroup, res.MagicWandCreator, res.MinDepositCharge 
	FROM
		(SELECT DepositGroup, 
			    MagicWandCreator, 
			    MIN(DepositCharge) AS [MinDepositCharge],
			    DENSE_RANK() OVER (PARTITION BY DepositGroup, MagicWandCreator ORDER BY DepositCharge ASC) AS [Rank]
			FROM WizzardDeposits
		GROUP BY DepositGroup, MagicWandCreator, DepositCharge) AS [res]
WHERE res.[Rank] = 1
ORDER BY res.MagicWandCreator ASC,
		 res.DepositGroup ASC
	-- ok 100/100 --

--or--
SELECT  DepositGroup, 
	MagicWandCreator, 
	MIN(DepositCharge) AS [MinDepositCharge],
 FROM WizzardDeposits
GROUP BY DepositGroup, MagicWandCreator, DepositCharge
ORDER BY MagicWandCreator ASC,
	 DepositGroup ASC

--Problem 9. Age Groups -> check result into Judge --
SELECT AgeGroup, 
	   COUNT(AgeGroup) AS [WizardCount]
	   --COUNT(*) AS [WizardCount] <- can use COUNT(*)
	   	FROM
		(SELECT 
			CASE
				WHEN Age <= 10 THEN '[0-10]'
				WHEN Age BETWEEN 11 AND 20 THEN '[11-20]'
				WHEN Age BETWEEN 21 AND 30 THEN '[21-30]'
				WHEN Age BETWEEN 31 AND 40 THEN '[31-40]'
				WHEN Age BETWEEN 41 AND 50 THEN '[41-50]'
				WHEN Age BETWEEN 51 AND 60 THEN '[51-60]'
				ELSE '[61+]'
			END AS [AgeGroup]
		FROM WizzardDeposits) AS [AgeGroupQuery]
GROUP BY AgeGroupQuery.AgeGroup
	-- ok 100/100 --

-- or second variant --
SELECT 
	CASE
		WHEN Age <= 10 THEN '[0-10]'
		WHEN Age BETWEEN 11 AND 20 THEN '[11-20]'
		WHEN Age BETWEEN 21 AND 30 THEN '[21-30]'
		WHEN Age BETWEEN 31 AND 40 THEN '[31-40]'
		WHEN Age BETWEEN 41 AND 50 THEN '[41-50]'
		WHEN Age BETWEEN 51 AND 60 THEN '[51-60]'
		ELSE '[61+]'
	END AS [AgeGroup],
	COUNT(*) AS [WizardCount]
			FROM WizzardDeposits 
GROUP BY (CASE
		WHEN Age <= 10 THEN '[0-10]'
		WHEN Age BETWEEN 11 AND 20 THEN '[11-20]'
		WHEN Age BETWEEN 21 AND 30 THEN '[21-30]'
		WHEN Age BETWEEN 31 AND 40 THEN '[31-40]'
		WHEN Age BETWEEN 41 AND 50 THEN '[41-50]'
		WHEN Age BETWEEN 51 AND 60 THEN '[51-60]'
		ELSE '[61+]'
	END)
	-- ok 100/100 --

--Problem 10. First Letter -> check result into Judge --
SELECT DISTINCT(LEFT(FirstName, 1)) AS [FirstLetter]
	FROM
(SELECT FirstName  
	FROM WizzardDeposits
WHERE DepositGroup LIKE 'Troll Chest') AS [FirstNameQuery]
ORDER BY [FirstLetter]
	-- ok 100/100 --

SELECT DISTINCT(FLetter) AS [FirstLetter] 
	FROM
(SELECT LEFT(FirstName, 1) AS [FLetter]
	FROM WizzardDeposits
	WHERE DepositGroup LIKE 'Troll Chest'
GROUP BY FirstName) AS [res]
ORDER BY [FirstLetter]
	-- ok 100/100 --

--or--
SELECT LEFT(FirstName, 1) AS [FLetter]
	FROM WizzardDeposits
	WHERE DepositGroup LIKE 'Troll Chest'
GROUP BY [FLetter] -- or -> GROUP BY LEFT(FirstName, 1)

--Problem 11. Average Interest  -> check result into Judge --
SELECT DepositGroup, IsDepositExpired, AVG(DepositInterest) AS [AverageInterest] 
	FROM WizzardDeposits
WHERE DepositStartDate > CONVERT(DATETIME, '01/01/1985', 103)
GROUP BY DepositGroup, IsDepositExpired
ORDER BY DepositGroup DESC,
		 IsDepositExpired ASC
	-- ok 100/100 -- 

SELECT DepositGroup, IsDepositExpired, AVG(DepositInterest) AS [AverageInterest] 
	FROM WizzardDeposits
WHERE DepositStartDate > '01/01/1985' --'1985/01/01'
GROUP BY DepositGroup, IsDepositExpired
ORDER BY DepositGroup DESC,
		 IsDepositExpired ASC
	-- ok 100/100 -- 

--Problem 12. *Rich Wizard, Poor Wizard -> check result into Judge --
SELECT SUM(w1.DepositAmount - w2.DepositAmount) AS [SumDifference] 
	FROM WizzardDeposits AS w1
JOIN WizzardDeposits AS w2 ON w1.Id + 1 = w2.Id
	-- ok 100/100 -- 

-- or use LEAD() funnction --> full/extended sintax/with more curr info --
SELECT SUM([CurrDiff]) AS [SumDifference]
	FROM
(SELECT 
	FirstName AS [Host],
	DepositAmount AS [HostDeposit],
	LEAD(FirstName) OVER(ORDER BY Id ASC) AS [Guest],
	LEAD(DepositAmount) OVER(ORDER BY Id ASC) AS [GuestDeposit],
	(DepositAmount - LEAD(DepositAmount) OVER(ORDER BY Id ASC)) AS [CurrDiff]
	FROM WizzardDeposits) AS [res]
WHERE [CurrDiff] IS NOT NULL
	-- ok 100/100 --

-- or shorter sitax --
SELECT SUM([CurrDiff]) AS [SumDifference]
	FROM
(SELECT
	(DepositAmount - LEAD(DepositAmount) OVER(ORDER BY Id ASC)) AS [CurrDiff]
	FROM WizzardDeposits) AS [res]
WHERE [CurrDiff] IS NOT NULL
	-- ok 100/100 --


USE SoftUni

--Problem 13. Departments Total Salaries -> check result into Judge --
SELECT DepartmentID, 
	   SUM(Salary) AS [TotalSalary] 
	FROM Employees
GROUP BY DepartmentID
ORDER BY DepartmentID
	-- ok 100/100 --

--Problem 14. Employees Minimum Salaries -> check result into Judge --
SELECT DepartmentID,
	   MIN(Salary) AS [MinimumSalary]
	FROM Employees
WHERE DepartmentID IN (2, 5, 7) 
  AND HireDate > CONVERT(DATETIME, '01/01/2000', 103)
GROUP BY DepartmentID
--ORDER BY DepartmentID
	-- ok 100/100 --

--Problem 15. Employees Average Salaries -> check result into Judge --

SELECT * INTO NewEmplTable FROM Employees
WHERE Salary > 30000

--SELECT * FROM NewEmplTable

DELETE FROM NewEmplTable
WHERE ManagerID = 42

UPDATE NewEmplTable
SET Salary += 5000
WHERE DepartmentID = 1

SELECT DepartmentID,
	   AVG(Salary) AS [AverageSalary]
	FROM NewEmplTable
GROUP BY DepartmentID
	-- ok 100/100 --

--Problem 16. Employees Maximum Salaries --
SELECT DepartmentID,
	   MAX(Salary) AS [MaxSalary]
FROM Employees
GROUP BY DepartmentID
HAVING MAX(Salary) NOT BETWEEN 30000 AND 70000
	--  100/100 --  THIS IS WRONG solution (acording to me) even thow Judge give 100/100 points !!!!
-----------------------
	SELECT DepartmentID,
	   Salary AS [MaxSalary]
FROM Employees
GROUP BY  DepartmentID, Salary
HAVING MAX(Salary) NOT BETWEEN 30000 AND 70000
ORDER BY DepartmentID
--------------------

SELECT DepartmentID,
	   MAX(Salary) AS MaxSalary
FROM Employees
WHERE Salary NOT BETWEEN 30000 AND 70000
GROUP BY DepartmentID
	-- 0/100 --

	-- second variant --
SELECT res.DepartmentID, 
	   res.[MaxSalary] 
	FROM
(SELECT DepartmentID,
	    MAX(Salary) AS [MaxSalary],
		DENSE_RANK() OVER(PARTITION BY DepartmentID ORDER BY Salary DESC) AS [Rank]
	FROM Employees
	WHERE Salary NOT BETWEEN 30000 AND 70000
GROUP BY DepartmentID, Salary
) AS [res]
WHERE res.[Rank] = 1
	-- 0/100 --
-------------
SELECT res.DepartmentID, 
	   res.[MaxSalary] 
	FROM
(SELECT DepartmentID,
	    Salary AS [MaxSalary],
		DENSE_RANK() OVER(PARTITION BY DepartmentID ORDER BY Salary DESC) AS [Rank]
	FROM Employees
GROUP BY DepartmentID, Salary
HAVING Salary NOT BETWEEN 30000 AND 70000
) AS [res]
WHERE res.[Rank] = 1
	-- 0/100 --

--Problem 17. Employees Count Salaries -> check result into Judge --
SELECT COUNT(Salary) AS [Count] 
	FROM Employees
WHERE ManagerID IS NULL
	-- ok 100/100 --

--Problem 18. *3rd Highest Salary -> check result into Judge --
SELECT res.DepartmentID,
	   res.Salary AS [ThirdHighestSalary] FROM
(SELECT DepartmentID, 
	   Salary,
	   DENSE_RANK() OVER (PARTITION BY DepartmentID ORDER BY Salary DESC) AS [Rank]
	FROM Employees
GROUP BY DepartmentID, Salary) AS res
WHERE res.[Rank] = 3
	-- ok 100/100 --

--or -> use DISTINCT intead of GROUP BY --
SELECT DISTINCT res.DepartmentID,
	   res.Salary AS [ThirdHighestSalary] FROM
(SELECT DepartmentID, 
	   Salary,
	   DENSE_RANK() OVER (PARTITION BY DepartmentID ORDER BY Salary DESC) AS [Rank]
	FROM Employees) AS res
WHERE res.[Rank] = 3

--Problem 19. **Salary Challenge -> check result into Judge --
SELECT TOP(10) e1.FirstName, 
	       e1.LastName, 
       	       e1.DepartmentID 
FROM Employees AS e1
WHERE e1.Salary > (
		   SELECT 
		   AVG(Salary) AS [DepartmentAvrgSalary]
		   FROM Employees AS e2
		   WHERE e1.DepartmentID = e2.DepartmentID
		   GROUP BY DepartmentID --will work without this-- 
		   )
ORDER BY DepartmentID
	-- ok 100/100 --
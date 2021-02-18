-- Exercises: Joins, Subqueries, CTE and Indices --

USE SoftUni
--Problem 1.Employee Address -> check the result into Judge
SELECT TOP(5) e.EmployeeId, e.JobTitle, e.AddressId, a.AddressText 
FROM Employees AS e, Addresses AS a  
WHERE e.AddressId = a.AddressId
ORDER BY e.AddressID ASC
	-- ok 100/100 -- 
-- or --
SELECT TOP(5) e.EmployeeId, e.JobTitle, e.AddressId, a.AddressText 
	FROM Employees AS e 
JOIN Addresses AS a ON e.AddressId = a.AddressId
ORDER BY e.AddressID ASC
	-- ok 100/100 --
			-- same result on both queries above --

--Problem 2.Addresses with Towns -> check the result into Judge
SELECT TOP(50) e.FirstName, e.LastName, t.[Name] AS Town, a.AddressText 
	FROM Employees AS e, Towns AS t, Addresses AS a
WHERE e.AddressID = a.AddressID 
	AND a.TownID = t.TownID
ORDER BY e.FirstName ASC,
		 e.LastName ASC
	-- ok 100/100 --
-- or --
SELECT TOP(50) e.FirstName, e.LastName, t.[Name] AS Town, a.AddressText 
	FROM Employees AS e
JOIN Addresses AS a ON e.AddressID = a.AddressID
JOIN Towns AS t ON t.TownID = a.TownID
ORDER BY e.FirstName ASC,
		 e.LastName ASC
	-- ok 100/100 --
			-- same result on both queries above --

--Problem 3.Sales Employee -> check the result into Judge
SELECT e.EmployeeID, e.FirstName, e.LastName, d.[Name] AS DepartmentName 
	FROM Employees AS e, Departments AS d
WHERE d.[Name] LIKE 'Sales'
--WHERE d.[Name] = 'Sales'
	AND d.DepartmentID = e.DepartmentID
ORDER BY e.EmployeeID ASC
	-- ok 100/100 --
-- or --
SELECT e.EmployeeID, e.FirstName, e.LastName, d.[Name] AS DepartmentName 
	FROM Employees AS e
JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
	WHERE d.[Name] LIKE 'Sales'
	--AND d.[Name] = 'Sales' --can use -> WHERE or AND ... LIKE or =
ORDER BY e.EmployeeID ASC
	-- ok 100/100 --
			-- same result on both queries above --

--Problem 4.Employee Departments -> check the result into Judge
SELECT TOP(5) e.EmployeeID, e.FirstName, e.Salary, d.[Name] AS DepartmentName
	FROM Employees AS e, Departments AS d
WHERE e.Salary > 15000 
	AND d.DepartmentID = e.DepartmentID
ORDER BY d.DepartmentID ASC 
	-- ok 100/100 --
-- or -- 
SELECT TOP(5) e.EmployeeID, e.FirstName, e.Salary, d.[Name] AS DepartmentName 
	FROM Employees AS e 
JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
WHERE e.Salary > 15000  -- can use WHERE or AND
ORDER BY d.DepartmentID ASC
	-- ok 100/100 --
			-- same result on both queries above --

--Problem 5.Employees Without Project -> check the result into Judge
SELECT TOP(3) e.EmployeeID, e.FirstName 
	FROM Employees AS e
LEFT JOIN EmployeesProjects AS ep --it is LEFT due to I start and want to take all data from Employees DB 
	ON e.EmployeeID = ep.EmployeeID
WHERE ep.ProjectID IS NULL
ORDER BY e.EmployeeID ASC
	-- ok 100/100 --

	-- or reverce/oposite querie --
SELECT TOP(3) e.EmployeeID, e.FirstName 
	FROM EmployeesProjects AS ep
RIGHT OUTER JOIN Employees AS e --it is RIGHT due to I start from EmployeesProjects DB and want to take all data from Emploees DB
	ON e.EmployeeID = ep.EmployeeID
WHERE ep.ProjectID IS NULL
ORDER BY e.EmployeeID ASC


--Problem 6.Employees Hired After -> check the result into Judge
SELECT e.FirstName, e.LastName, e.HireDate, d.[Name] AS [DeptName]
	FROM Employees AS e
JOIN Departments AS d 
	ON e.DepartmentID = d.DepartmentID
--WHERE d.[Name] LIKE 'Sales' 
   --OR d.[Name] LIKE 'Finance'
WHERE d.[Name] IN ('Sales', 'Finance')
	AND e.HireDate > CONVERT(DATETIME, '1.1.1999')
ORDER BY e.HireDate ASC
	-- ok 100/100 --

--Problem 7.Employees with Project -> check the result into Judge
SELECT TOP(5) e.EmployeeID, e.FirstName, p.[Name] AS [PojectName]
	FROM Employees AS e
JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
JOIN Projects AS p ON p.ProjectID = ep.ProjectID
WHERE p.StartDate > CONVERT(DATETIME, '08.13.2002') -- manualy revert 13.08.2002(d-m-y) to 08.13.2002(m-d-y) 
--WHERE p.StartDate > CONVERT(DATETIME, '13.08.2002', 104) -- convert German(104) format 13.08.2002(d-m-y) to US(101) format 08.13.2002(mm-dd-yyyy) 
	AND p.EndDate IS NULL
ORDER BY e.EmployeeID ASC 
	-- ok 100/100 --

--Problem 8.Employee 24 -> check the result into Judge
SELECT e.EmployeeID, e.FirstName, 
	(CASE
		WHEN DATEPART(YEAR, p.StartDate) >= 2005 THEN 'NULL'
		ELSE p.[Name]
	END) AS [ProjectName]
FROM Employees AS e
JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
JOIN Projects AS p ON p.ProjectID = ep.ProjectID
WHERE e.EmployeeID = 24 
	-- ok 100/100 --          

--Problem 9.Employee Manager -> check the result into Judge
SELECT e.EmployeeID, e.FirstName, e.ManagerID, 
	(CASE
		WHEN e.ManagerID = 3 THEN 'Roberto'
		ELSE 'JoLynn'
	END) AS [ManagerName]
		FROM Employees AS e
WHERE e.ManagerID IN (3,7)
ORDER BY e.EmployeeID ASC
	-- ok 100/100 -- 

-- or better --
SELECT e.EmployeeId, e.FirstName, e.ManagerID, m.FirstName AS [ManagerName]
	FROM Employees AS e
JOIN Employees AS m ON m.EmployeeId = e.ManagerId
WHERE e.ManagerId IN (3,7) 
ORDER BY e.EmployeeId ASC
	-- ok 100/100 -- 

--Problem 10.Employee Summary -> check the result into Judge
SELECT TOP(50) 
	e.EmployeeID, 
	CONCAT(e.FirstName, ' ', e.LastName) AS [EmploeeName], 
	CONCAT(m.FirstName, ' ', m.LastName) AS [ManagerName], 
	d.[Name] AS [DepartmentName] 
		FROM Employees AS e
JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
JOIN Employees AS m ON m.EmployeeID = e.ManagerID
ORDER BY e.EmployeeID ASC
	-- ok 100/100 -- 
-- or båtter to use LEFT OUTER JOIN -- due to some employees without manager will not enter into the result
SELECT TOP(50) 
	e.EmployeeID, 
	CONCAT(e.FirstName, ' ', e.LastName) AS [EmploeeName], 
	CONCAT(m.FirstName, ' ', m.LastName) AS [ManagerName], 
	d.[Name] AS [DepartmentName] 
		FROM Employees AS e
JOIN Departments AS d ON d.DepartmentID = e.DepartmentID
LEFT OUTER JOIN Employees AS m ON m.EmployeeID = e.ManagerID
ORDER BY e.EmployeeID ASC
	-- ok 100/100 --

--Problem 11.Min Average Salary -> check the result into Judge
SELECT MIN(av.AvgSalary) AS MinAverageSalary
FROM
	(SELECT e.DepartmentID, AVG(e.Salary) AS AvgSalary
	 FROM Employees AS e
	 GROUP BY e.DepartmentID) AS av 
	-- ok 100/100 --

SELECT TOP(1) AVG(Salary) AS MinAverageSalary 
	FROM Employees
GROUP BY DepartmentID
ORDER BY AVG(Salary)
	-- ok 100/100 --

SELECT TOP(1)
	(SELECT AVG(Salary) FROM Employees AS e
		WHERE e.DepartmentID = d.DepartmentID) AS MinAverageSalary
FROM Departments AS d
ORDER BY MinAverageSalary
	-- ok 100/100 --
	
-----------------------------------------------------------
	-- some exercises over AVG() --

--SELECT Salary FROM Employees
--WHERE Salary >
--(SELECT AVG(Salary) FROM Employees)
--ORDER BY Salary ASC

--SELECT AVG(Salary) FROM Employees --> 18504 lv.

	-- sintax --
--SELECT c.DepartmenID_Column, AVG(c.Salary_Column) AS AVGSalary
--FROM Company_DB AS c
--GROUP BY c.DepartmenID_Column

--SELECT * FROM Products_DB
--WHERE Price_Column > (SELECT AVG(Price_Column) FROM Products_DB)
--------------------------------------------------------------------------

USE [Geography]
--Problem 12.Highest Peaks in Bulgaria -> check the result into Judge
SELECT c.CountryCode, m.MountainRange, p.PeakName, p.Elevation
	FROM MountainsCountries AS mc
JOIN Countries AS c ON mc.CountryCode = c.CountryCode
	AND c.CountryCode LIKE 'BG'
LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
LEFT JOIN Peaks AS p ON p.MountainId = m.Id
	WHERE p.Elevation > 2835
ORDER BY p.Elevation DESC
	-- ok 100/100 --

-- or --
SELECT c.CountryCode, m.MountainRange, p.PeakName, p.Elevation
	FROM MountainsCountries AS mc
JOIN Countries AS c ON mc.CountryCode = c.CountryCode
JOIN Mountains AS m ON mc.MountainId = m.Id
JOIN Peaks AS p ON p.MountainId = m.Id
	WHERE p.Elevation > 2835 AND c.CountryCode LIKE 'BG'
ORDER BY p.Elevation DESC
	-- ok 100/100 --

--Problem 13.Count Mountain Ranges -> check the result into Judge
SELECT mc.CountryCode--, COUNT(m.MountainRange) AS [MoutainRanges]
	FROM MountainsCountries AS mc
JOIN Mountains AS m ON mc.MountainId = m.Id
JOIN Countries AS c ON mc.CountryCode = c.CountryCode
	WHERE mc.CountryCode IN ('BG', 'RU','US')
	-- !!! -- does not work like this -> will need data aggregation

-- or use GROUP BY --
SELECT mc.CountryCode, COUNT(m.MountainRange) AS [MountainRanges]
	FROM MountainsCountries AS mc
RIGHT JOIN Mountains AS m ON mc.MountainId = m.Id
	WHERE mc.CountryCode IN ('BG', 'RU','US')
GROUP BY mc.CountryCode
ORDER BY COUNT(m.MountainRange) DESC
	-- ok 100/100 --

-- or --
SELECT mc.CountryCode, COUNT(m.MountainRange) AS [MountainRanges]
	FROM MountainsCountries AS mc
JOIN Mountains AS m ON mc.MountainId = m.Id
	WHERE mc.CountryCode IN ('BG', 'RU','US')
GROUP BY mc.CountryCode
ORDER BY [MountainRanges] DESC

-- or shortest variant --
SELECT mc.CountryCode, COUNT(mc.MountainId) AS [MountainRanges]
	FROM MountainsCountries AS mc
WHERE mc.CountryCode IN ('BG', 'RU', 'US')
GROUP BY mc.CountryCode
ORDER BY [MountainRanges] DESC

--Problem 14.Countries with or without Rivers -> check the result into Judge
	-- start from mappig table CountriesRivers --
SELECT TOP(5) c.CountryName, r.RiverName 
	FROM CountriesRivers AS cr
RIGHT JOIN Countries AS c ON cr.CountryCode = c.CountryCode
LEFT JOIN Rivers AS r ON cr.RiverId = r.Id
WHERE c.ContinentCode LIKE 'AF'
ORDER BY c.CountryName ASC
	-- ok 100/100 --

	-- start from Countries table --
SELECT TOP(5) c.CountryName, r.RiverName
	FROM Countries AS c
LEFT JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
LEFT JOIN Rivers AS r ON r.Id = cr.RiverId
	WHERE c.ContinentCode LIKE 'AF'
ORDER BY c.CountryName ASC
	-- ok 100/100 --

--Problem 15. *Continents and Currencies -> check the result into Judge
SELECT k.ContinentCode, k.CurrencyCode, k.CountCurrency AS [CurrencyUsage]
 FROM
	(SELECT 
		ContinentCode, 
		CurrencyCode, 
		COUNT(CurrencyCode) AS [CountCurrency],
		DENSE_RANK() OVER (PARTITION BY ContinentCode ORDER BY COUNT(CurrencyCode) DESC) AS [CurrencyRank]
			FROM Countries
	GROUP BY ContinentCode, CurrencyCode
	HAVING COUNT(CurrencyCode) > 1) AS [k] --remove any currency that is used in only one country --
WHERE k.CurrencyRank = 1 --take the currency with rank 1, given by the DENCE_RANK selection above -- 
ORDER BY ContinentCode
	-- ok 100/100 --

--Problem 16.Countries Without Any Mountains -> check the result into Judge
SELECT COUNT(*) AS [Count]
	FROM Countries AS c
LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
WHERE mc.MountainId IS NULL
	-- ok 100/100 --

--Problem 17.Highest Peak and Longest River by Country -> check the result into Judge

SELECT TOP(5) HighLong.CountryName, 
			  HighLong.Elevation AS [HighestPeakElevation],
			  HighLong.[Length] AS [LongestRiverLength]
			FROM
		(SELECT c.CountryName, 
			    p.Elevation,
				r.[Length],
			    DENSE_RANK() OVER (PARTITION BY c.CountryName ORDER BY p.Elevation DESC) AS [PeakRank],
			    DENSE_RANK() OVER (PARTITION BY c.CountryName ORDER BY r.[Length] DESC) AS [RiverRank]
			FROM Countries AS c
		LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
		LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
		LEFT JOIN Peaks AS p ON m.Id = p.MountainId
		LEFT JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
		LEFT JOIN Rivers AS r ON cr.RiverId = r.Id
		GROUP BY c.CountryName, m.MountainRange, p.Elevation, cr.RiverId, r.[Length]) AS HighLong
WHERE [PeakRank] = 1 AND [RiverRank] = 1
ORDER BY HighLong.Elevation DESC, HighLong.[Length] DESC, HighLong.CountryName
	-- ok 100/100 --

	-- or shorter and easier variant --
SELECT TOP(5) c.CountryName, 
			  MAX(p.Elevation) AS [HighestPeakElevation],
			  MAX(r.[Length]) AS [LongestRiverLength]
			FROM Countries AS c
		LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
		LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
		LEFT JOIN Peaks AS p ON m.Id = p.MountainId
		LEFT JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
		LEFT JOIN Rivers AS r ON cr.RiverId = r.Id
		GROUP BY c.CountryName
ORDER BY [HighestPeakElevation] DESC, [LongestRiverLength] DESC, c.CountryName ASC
	-- ok 100/100 --

--Problem 18.Highest Peak Name and Elevation by Country -> check the result into Judge
-- this below show all info(countries, peak names, elevation of each peak, mountains) --
SELECT --TOP(5)
	c.CountryName AS [Country],
	ISNULL(p.PeakName, '(no highest peak)') AS [Highest Peak Name],
	ISNULL(p.Elevation, '0') AS [Highest Peak Elevation],
	ISNULL(m.MountainRange, '(no mountain)') AS [Mountain]
		FROM Countries AS c
LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
LEFT JOIN Peaks AS p ON m.Id = p.MountainId
ORDER BY [Country] ASC,
		 [Highest Peak Name] ASC
	-- ok 100/100 --

-- select the highest one peak per country along with country, mountain --
SELECT --TOP(5)
	res.CountryName AS [Country],
	ISNULL(res.PeakName, '(no highest peak)') AS [Highest Peak Name],
	ISNULL(res.Elevation, '0') AS [Highest Peak Elevation],
	ISNULL(res.MountainRange, '(no mountain)') AS [Mountain]
	 FROM
		(SELECT 
			c.CountryName,
			p.PeakName,
			p.Elevation,
			m.MountainRange,
			DENSE_RANK() OVER (PARTITION BY c.CountryName ORDER BY p.Elevation DESC) AS [PeakRank]
				FROM Countries AS c
		LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
		LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
		LEFT JOIN Peaks AS p ON m.Id = p.MountainId
		GROUP BY c.CountryName, p.PeakName, p.Elevation, m.MountainRange) AS [res]
WHERE [PeakRank] = 1
ORDER BY [Country] ASC,
	 [Highest Peak Name] ASC
	-- ok 100/100 --

	-- or --
SELECT --TOP(5)
	[Country],
	[Highest Peak Name],
	[Highest Peak Elevation],
	[Mountain]
	 FROM
		(SELECT 
			c.CountryName AS [Country],
			ISNULL(p.PeakName, '(no highest peak)') AS [Highest Peak Name],
			ISNULL(p.Elevation, '0') AS [Highest Peak Elevation],
			ISNULL(m.MountainRange, '(no mountain)') AS [Mountain],
			DENSE_RANK() OVER (PARTITION BY c.CountryName ORDER BY p.Elevation DESC) AS [PeakRank]
				FROM Countries AS c
		LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
		LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
		LEFT JOIN Peaks AS p ON m.Id = p.MountainId
		GROUP BY c.CountryName, p.PeakName, p.Elevation, m.MountainRange) AS [res]
WHERE [PeakRank] = 1
ORDER BY [Country] ASC,
	 [Highest Peak Name] ASC

------------- row file --------------------
SELECT 
	c.CountryName AS [Country],
	p.PeakName AS [Highest Peak Name],
	p.Elevation AS [Highest Peak Elevation],
	m.MountainRange AS [Mountain]
	--CASE
	--	WHEN p.PeakName IS NULL THEN '(no highest peak)'
	--	ELSE p.PeakName
	--END AS [Highest Peak Name],
	--CASE
	--	WHEN p.Elevation IS NULL THEN '0'
	--	ELSE p.Elevation
	--END AS [Highest Peak Elevation],
	--CASE
	--	WHEN m.MountainRange IS NULL THEN '(no mountain)'
	--	ELSE m.MountainRange
	--END AS [Mountain]
		FROM Countries AS c
LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
LEFT JOIN Peaks AS p ON m.Id = p.MountainId
ORDER BY [Country] ASC,
		 [Highest Peak Name] ASC
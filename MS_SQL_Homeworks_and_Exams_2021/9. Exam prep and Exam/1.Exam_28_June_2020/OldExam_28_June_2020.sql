CREATE DATABASE [ColonialJourney]

USE [ColonialJourney]

-- DO NOT sent above code to Judge --
	-- Section 1. DDL --
CREATE TABLE Planets(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(30) NOT NULL
	)

CREATE TABLE Spaceports(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(50) NOT NULL,
	PlanetId INT FOREIGN KEY REFERENCES Planets(Id) NOT NULL
	)

CREATE TABLE Spaceships(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(50) NOT NULL,
	Manufacturer VARCHAR(30) NOT NULL,
	LightSpeedRate INT DEFAULT 0 
	)

CREATE TABLE Colonists(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName VARCHAR(20) NOT NULL,
	LastName VARCHAR(20) NOT NULL,
	Ucn VARCHAR(10) UNIQUE NOT NULL,
	BirthDate DATE NOT NULL
	)

CREATE TABLE Journeys(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	JourneyStart DATETIME NOT NULL,
	JourneyEnd DATETIME NOT NULL,
	Purpose VARCHAR(11) CHECK (Purpose IN ('Medical', 'Technical', 'Educational', 'Military')),
	DestinationSpaceportId INT FOREIGN KEY REFERENCES Spaceports(Id) NOT NULL,
	SpaceshipId INT FOREIGN KEY REFERENCES Spaceships(Id) NOT NULL
	)

CREATE TABLE TravelCards(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	CardNumber CHAR(10) UNIQUE NOT NULL,
	JobDuringJourney VARCHAR(8) CHECK (JobDuringJourney IN ('Pilot', 'Engineer', 'Trooper', 'Cleaner', 'Cook')),
	ColonistId INT FOREIGN KEY REFERENCES Colonists(Id) NOT NULL,
	JourneyId INT FOREIGN KEY REFERENCES Journeys(Id) NOT NULL
	)

	-- ok 30/30 --

	-- Section 2. DML --
-- 2. Insert --
INSERT INTO Planets([Name])
VALUES 
	('Mars'),	
	('Earth'),	
	('Jupiter'),	
	('Saturn')

INSERT INTO Spaceships([Name], Manufacturer, LightSpeedRate)
VALUES
	('Golf', 'VW', 3),	
	('WakaWaka', 'Wakanda', 4),	
	('Falcon9', 'SpaceX', 1),	
	('Bed', 'Vidolov', 6)

	-- ok 2/2 --
-- 3. Update --
UPDATE Spaceships
SET LightSpeedRate += 1
WHERE Id BETWEEN 8 AND 12

	-- ok 4/4 --

-- 4. Delete --
--Delete first three inserted Journeys (be careful with the relationships).
DELETE FROM TravelCards
WHERE JourneyId IN (
					SELECT Id FROM Journeys
					WHERE Id IN (1,2,3)
					)

DELETE FROM Journeys
WHERE Id IN (1,2,3)

	-- ok 4/4 --


	-- Section 3. Querying --
-- 5. Select all military journeys --
SELECT Id,
	   FORMAT(JourneyStart, 'dd/MM/yyyy') AS [JourneyStart],
	   FORMAT(JourneyEnd, 'dd/MM/yyyy') AS [JourneyEnd]
	FROM Journeys
WHERE Purpose LIKE 'Military'
ORDER BY JourneyStart

	-- ok 3/3 --

-- 6. Select all pilots --
SELECT c.Id,
	   CONCAT(c.FirstName, ' ', c.LastName) AS [full_name]
	FROM TravelCards AS tc
JOIN Colonists AS c ON tc.ColonistId = c.Id
WHERE tc.JobDuringJourney LIKE 'Pilot'
ORDER BY c.Id

	-- ok 5/5 --

-- 7. Count colonists --
SELECT c.Id, c.FirstName, c.LastName, COUNT(*) AS [count]
	FROM Colonists AS c
JOIN TravelCards AS tc ON c.Id = tc.ColonistId
JOIN Journeys AS j ON tc.JourneyId = j.Id
WHERE j.Purpose LIKE 'Technical'
GROUP BY c.FirstName, c.LastName, c.Id

------ correct query below -------
SELECT COUNT(*) AS [count]
	FROM Colonists AS c
JOIN TravelCards AS tc ON c.Id = tc.ColonistId
JOIN Journeys AS j ON tc.JourneyId = j.Id
WHERE j.Purpose LIKE 'Technical'
	-- ok 6/6 --

-- 8. Select spaceships with pilots younger than 30 years --

--SELECT DATEDIFF(YEAR, CONVERT(DATE,  BirthDate, 104), '01/01/2019') AS [DateDiff]
	FROM Colonists


SELECT k.[Name], k.Manufacturer
	FROM 
(SELECT s.[Name], 
		s.Manufacturer, 
		DATEDIFF(YEAR, CONVERT(DATE, c.BirthDate, 104), '01/01/2019') AS [DateDiff]
	FROM TravelCards AS tc
JOIN Colonists AS c ON c.Id = tc.ColonistId
JOIN Journeys AS j ON j.Id = tc.JourneyId
JOIN Spaceships AS s ON s.Id = j.SpaceshipId
WHERE tc.JobDuringJourney LIKE 'Pilot' AND DATEDIFF(YEAR, CONVERT(DATE, c.BirthDate, 104), '01/01/2019') < 30
GROUP BY s.[Name], s.Manufacturer, c.BirthDate) AS k
ORDER BY k.[Name]
	-- ok 7/7 --

-- 9. Select all planets and their journey count --
SELECT p.[Name] AS [PlanetName], COUNT(*) AS [JourneysCount]
	FROM Journeys AS j
JOIN Spaceports AS s ON j.DestinationSpaceportId = s.Id
JOIN Planets AS p ON s.PlanetId = p.Id
GROUP BY p.[Name]
ORDER BY [JourneysCount] DESC, p.[Name]
	-- ok 7/7 --

-- 10. Select Second Oldest Important Colonist --
SELECT k.JobDuringJourney, k.FullName, k.[Rank] AS [JobRank]
	FROM
(SELECT tc.JobDuringJourney, 
	   CONCAT(c.FirstName, ' ', c.LastName) AS [FullName],
	   DENSE_RANK() OVER (PARTITION BY tc.JobDuringJourney ORDER BY c.BirthDate) AS [Rank]
FROM TravelCards AS tc
JOIN Colonists AS c ON tc.ColonistId = c.Id
GROUP BY tc.JobDuringJourney, c.BirthDate, c.FirstName, c.LastName) AS k
WHERE k.[Rank] = 2
	-- ok 12/12 --
--or just use JobRank --
SELECT k.JobDuringJourney, k.FullName, k.[JobRank]
	FROM
(SELECT tc.JobDuringJourney, 
	   CONCAT(c.FirstName, ' ', c.LastName) AS [FullName],
	   DENSE_RANK() OVER (PARTITION BY tc.JobDuringJourney ORDER BY c.BirthDate) AS [JobRank]
FROM TravelCards AS tc
JOIN Colonists AS c ON tc.ColonistId = c.Id
GROUP BY tc.JobDuringJourney, c.BirthDate, c.FirstName, c.LastName) AS k
WHERE k.[JobRank] = 2

		-- Section 4. Programmability --
-- 11. Get Colonists Count --
GO
CREATE FUNCTION dbo.udf_GetColonistsCount(@PlanetName VARCHAR (30))
RETURNS INT
AS
BEGIN
	DECLARE @countOfColonists INT = 
		(SELECT COUNT(*) AS [Count]
			FROM Colonists AS c
		JOIN TravelCards AS tc ON c.Id = tc.ColonistId
		JOIN Journeys AS j ON tc.JourneyId = j.Id
		JOIN Spaceports AS sp ON sp.Id = j.DestinationSpaceportId
		JOIN Planets AS p ON p.Id = sp.PlanetId
		WHERE p.[Name] LIKE @PlanetName);

	RETURN @countOfColonists;
END
	-- ok 10/10 --
GO
SELECT dbo.udf_GetColonistsCount('Otroyphus') AS [Count];


-- 12. Change Journey Purpose --
GO
CREATE PROC usp_ChangeJourneyPurpose(@JourneyId INT, @NewPurpose VARCHAR(11))
AS
BEGIN
	DECLARE @oldJourneyId INT = (SELECT Id FROM Journeys WHERE Id = @JourneyId);

	DECLARE @oldPurpose VARCHAR(11) = (SELECT Purpose FROM Journeys WHERE Id = @JourneyId);

	IF(@oldJourneyId IS NULL)
	--BEGIN
		THROW 50001, 'The journey does not exist!', 1
		--ROLLBACK
		--RAISERROR ('The journey does not exist!', 16, 1);
		--RETURN
	--END

	IF(@oldPurpose = @NewPurpose)
	--BEGIN
		THROW 50002, 'You cannot change the purpose!', 1
		--ROLLBACK
		--RAISERROR ('You cannot change the purpose!', 16, 1);
		--RETURN
	--END
	
	UPDATE Journeys
	SET Purpose = @NewPurpose
	WHERE Id = @JourneyId
END

	-- ok 10/10 -- BEGIN and END could stay combined with THROW...!!!
	-- in Judje -> ROLBACK, RAISERROR and RETURN does not work --

	-- or -----
--GO
--CREATE PROC usp_ChangeJourneyPurpose(@JourneyId INT, @NewPurpose VARCHAR(11))
--AS
--BEGIN
--BEGIN TRANSACTION

--BEGIN TRY
--	IF (NOT EXISTS(SELECT j.Purpose FROM Journeys AS j WHERE j.Id = @journeyId)
--		)
--	BEGIN
--			ROLLBACK
--			RAISERROR('The journey does not exist!', 16, 1)
--			RETURN
--	END

--	DECLARE @CurrentPurpose VARCHAR(11) = (SELECT j.Purpose FROM Journeys AS j WHERE j.Id = @journeyId)

--	IF (@CurrentPurpose = @NewPurpose)
--	BEGIN
--			ROLLBACK
--			RAISERROR('You cannot change the purpose!', 16,1)
--			RETURN
--	END
--	COMMIT
--END TRY

--BEGIN CATCH
--	SELECT ERROR_MESSAGE() AS [Error Message] 
--END CATCH

--	UPDATE Journeys
--		SET Purpose = @NewPurpose
--		WHERE Id = @JourneyId
--END
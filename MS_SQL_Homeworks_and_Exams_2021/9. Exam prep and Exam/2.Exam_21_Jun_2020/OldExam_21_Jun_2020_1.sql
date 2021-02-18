-- Database MS SQL Exam – 21 Jun 2020 --
-- Trip Service --

CREATE DATABASE [TripService]

USE [TripService]

-- Section 1. DDL (30 pts) --
-- 1. Database design --
CREATE TABLE  Cities(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] NVARCHAR(20) NOT NULL,
	CountryCode CHAR(2) NOT NULL
	)

CREATE TABLE Hotels(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name]  NVARCHAR(30) NOT NULL,
	CityId INT FOREIGN KEY REFERENCES Cities(Id) NOT NULL,
	EmployeeCount INT NOT NULL,
	BaseRate DECIMAL(15,2)
	)

CREATE TABLE Rooms(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	Price DECIMAL(15,2) NOT NULL,
	[Type] NVARCHAR(20) NOT NULL,
	Beds INT NOT NULL,
	HotelId INT FOREIGN KEY REFERENCES Hotels(Id) NOT NULL
	)

CREATE TABLE Trips(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	RoomId INT FOREIGN KEY REFERENCES Rooms(Id) NOT NULL,
	BookDate DATE NOT NULL,
	ArrivalDate DATE NOT NULL,
	ReturnDate DATE NOT NULL,
	CancelDate DATE,
	CONSTRAINT chk_BookDate CHECK(BookDate < ArrivalDate),
	CONSTRAINT chk_ArrivalDate CHECK(ArrivalDate < ReturnDate)
	)

CREATE TABLE Accounts(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName NVARCHAR(50) NOT NULL,
	MiddleName NVARCHAR(20),
	LastName NVARCHAR(50) NOT NULL,
	CityId INT FOREIGN KEY REFERENCES Cities(Id) NOT NULL,
	BirthDate DATE NOT NULL,
	Email VARCHAR(100) UNIQUE NOT NULL
	)

CREATE TABLE AccountsTrips(
	AccountId INT REFERENCES Accounts(Id) NOT NULL,
	TripId INT REFERENCES Trips(Id) NOT NULL,
	Luggage INT CHECK (Luggage >= 0) NOT NULL,
	PRIMARY KEY (AccountId, TripId)
	)
	-- ok 30/30 --

-- Section 2. DML (10 pts) -- 
-- 2. Insert --
INSERT INTO Accounts(FirstName, MiddleName, LastName, CityId, BirthDate, Email)
VALUES
	('John', 'Smith', 'Smith', 34, '1975-07-21', 'j_smith@gmail.com'),
	('Gosho', NULL, 'Petrov', 11, '1978-05-16', 'g_petrov@gmail.com'),
	('Ivan', 'Petrovich', 'Pavlov', 59,	'1849-09-26', 'i_pavlov@softuni.bg'),
	('Friedrich', 'Wilhelm', 'Nietzsche', 2, '1844-10-15', 'f_nietzsche@softuni.bg')

INSERT INTO Trips(RoomId, BookDate, ArrivalDate, ReturnDate, CancelDate)
VALUES
	(101, '2015-04-12', '2015-04-14', '2015-04-20', '2015-02-02'),
	(102, '2015-07-07', '2015-07-15', '2015-07-22', '2015-04-29'),
	(103, '2013-07-17', '2013-07-23', '2013-07-24', NULL),
	(104, '2012-03-17', '2012-03-31', '2012-04-01', '2012-01-10'),
	(109, '2017-08-07', '2017-08-28', '2017-08-29', NULL)
	-- ok 3/3 --

-- 3. Update --
UPDATE Rooms
SET Price *= 1.14
WHERE HotelId IN (5, 7, 9)
	-- ok 3/3 --

-- 4. Delete --
--Delete all of Account ID 47’s account’s trips from the mapping table.
DELETE FROM AccountsTrips
WHERE AccountId = 47
-- no need to sent this below --
DELETE FROM Accounts
WHERE Id = 47
	-- ok 3/3 --

	-- Section 3. Querying (40 pts) --
-- 5. EEE-Mails --
SELECT a.FirstName, 
	   a.LastName, 
	   FORMAT(a.BirthDate, 'MM-dd-yyyy') AS [BirthDate], 
	   c.[Name] AS [Hometown], 
	   a.Email 
	FROM Accounts AS a
JOIN Cities AS c ON a.CityId = c.Id
WHERE LEFT(Email, 1) LIKE 'e' --or-> a.Email LIKE 'e%'
ORDER BY c.[Name]
	-- ok 3/3 --

-- 6. City Statistics --
SELECT c.[Name] AS City, COUNT(*) AS Hotels 
	FROM Cities AS c
JOIN Hotels AS h ON c.Id = h.CityId
GROUP BY c.[Name]
ORDER BY Hotels DESC, c.[Name]
	-- ok 3/3 --

-- 7. Longest and Shortest Trips -- 
SELECT a.Id, 
	   CONCAT(a.FirstName, ' ', a.LastName) AS [FullName],
	   MAX(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate)) AS [LongestTrip], 
	   MIN(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate)) AS [ShortestTrip]
	FROM Accounts AS a
JOIN AccountsTrips AS atr ON a.Id = atr.AccountId
JOIN Trips AS t ON atr.TripId = t.Id
WHERE a.MiddleName IS NULL AND t.CancelDate IS NULL
GROUP BY a.Id, a.FirstName, a.LastName
ORDER BY [LongestTrip] DESC, 
		 [ShortestTrip]
	-- ok 3/3 --


-- 8. Metropolis --
SELECT TOP(10) c.Id, 
			   c.[Name], 
			   c.CountryCode, 
			   COUNT(c.Id) AS [Accounts]
	FROM Cities AS c
JOIN Accounts AS a ON c.Id = a.CityId
GROUP BY c.Id, c.[Name], c.CountryCode
ORDER BY [Accounts] DESC
	-- ok 3/3 --

-- 9. Romantic Getaways -- 
--Find all accounts, which have had one or more trips to a hotel in their hometown.
SELECT a.Id,	
	   a.Email,
	   c.[Name] AS [City],
	   COUNT(atr.TripId) AS [Trips]
	FROM Accounts AS a
		JOIN AccountsTrips AS atr 
			ON a.Id = atr.AccountId
		JOIN Trips AS t 
			ON atr.TripId = t.Id
		JOIN Rooms AS r 
			ON t.RoomId = r.Id
		JOIN Hotels AS h 
			ON r.HotelId = h.Id
		JOIN Cities AS c 
			ON h.CityId = c.Id -- AND c.Id = a.CityId -- <- could use this additional join here, instead of WHERE clause below
WHERE a.CityId = h.CityId
GROUP BY a.Id, 
		 a.Email, 
		 c.[Name]
ORDER BY [Trips] DESC,
		 a.Id
	-- ok 3/3 --

-- 10. GDPR Violation --
-- here into SELCT can use CASE WHEN...THEN ...END statement instead of IIF --
SELECT t.Id,
	   CONCAT(FirstName, ' ', ISNULL(MiddleName + ' ', ''), LastName) AS [Full Name],
	   c.[Name] AS [From],
	   c2.[Name] AS [To],
	   IIF(t.CancelDate IS NOT NULL, 'Canceled', CAST(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate) AS NVARCHAR(50)) + ' days') AS [Duration]
	FROM Trips AS t
		JOIN AccountsTrips AS atr ON t.Id = atr.TripId
		JOIN Accounts AS a ON atr.AccountId = a.Id
		JOIN Cities AS c ON a.CityId = c.Id
		JOIN Rooms AS r ON t.RoomId = r.Id
		JOIN Hotels AS h ON r.HotelId = h.Id 
		JOIN Cities AS c2 ON h.CityId = c2.Id
ORDER BY [Full Name],
		 t.Id
	-- ok 4/4 --


	-- Section 4. Programmability (14 pts) --
-- 11. Available Room -- --https://pastebin.com/2VTwb02R --
GO
CREATE FUNCTION udf_GetAvailableRoom(@HotelId INT, @Date DATE, @People INT)
RETURNS VARCHAR(max)
AS
BEGIN
	DECLARE @result VARCHAR(max);

	DECLARE @currHotelId INT = (SELECT Id FROM Hotels WHERE Id = @HotelId);

	DECLARE @roomId INT =
	(SELECT TOP(1) r.Id 
		FROM Rooms AS r
	JOIN Hotels AS h ON r.HotelId = h.Id
	JOIN Trips AS t ON r.Id = t.RoomId
	WHERE h.Id = @currHotelId AND r.Beds >= @People AND @Date NOT BETWEEN t.ArrivalDate AND t.ReturnDate
	ORDER BY r.Price DESC);

	IF(@roomId IS NOT NULL)
	BEGIN
		SET @result = 
		(SELECT TOP(1) CONCAT('Room ', CAST(r.Id AS VARCHAR(100)),':', ' ', CAST(r.[Type] AS VARCHAR(200)), ' ', + '(', CAST(r.Beds AS VARCHAR(100)) + ' beds) - $', CAST(((r.Price + h.BaseRate) * @People) AS VARCHAR(max)))
		FROM Rooms AS r
		JOIN Hotels AS h ON r.HotelId = h.Id
		JOIN Trips AS t ON r.Id = t.RoomId
		WHERE h.Id = @currHotelId 
		  AND r.Beds >= @People 
		  AND @Date NOT BETWEEN t.ArrivalDate AND t.ReturnDate
		ORDER BY (r.Price + h.BaseRate) * @People DESC)
	END
	ELSE
	BEGIN
		SET @result = 'No rooms available';
	END

	--SET @result = (SELECT IIF(@roomId IS NOT NULL, 
	--	(SELECT TOP(1) CONCAT('Room ', CAST(r.Id AS VARCHAR(100)),':', ' ', CAST(r.[Type] AS VARCHAR(200)), ' ', + '(', CAST(r.Beds AS VARCHAR(100)) + ' beds) - $', CAST(((r.Price + h.BaseRate) * @People) AS VARCHAR(max)))
	--	FROM Rooms AS r
	--	JOIN Hotels AS h ON r.HotelId = h.Id
	--	JOIN Trips AS t ON r.Id = t.RoomId
	--	WHERE h.Id = @currHotelId AND r.Beds >= @People AND @Date NOT BETWEEN t.ArrivalDate AND t.ReturnDate
	--  ORDER BY r.Price DESC), 
	--	'No rooms available'))

	RETURN @result;
END
	-- 5/7 --
GO
SELECT dbo.udf_GetAvailableRoom(112, '2011-12-17', 2)
---------
CREATE FUNCTION udf_GetAvailableRoom(@HotelId INT, @DATE DATE, @People INT)
RETURNS VARCHAR(MAX)
AS
BEGIN
    DECLARE @roomId INT;
    DECLARE @TYPE VARCHAR(20);
    DECLARE @beds INT;
    DECLARE @totalPrice DECIMAL(15,2);
 
        SELECT TOP(1) 
			@roomId = t.RoomId, 
			@TYPE = r.TYPE, 
			@beds = r.Beds,
			@totalPrice = (h.BaseRate + r.Price) * @People
        FROM Hotels AS h
        JOIN Rooms AS r ON r.HotelId = h.Id
        JOIN Trips AS t ON t.RoomId = r.Id
			WHERE 
				(h.Id = @HotelId) AND
				(r.Beds >= @People) AND
				(YEAR(@DATE) = YEAR(t.ArrivalDate)) AND
				(t.CancelDate IS NOT NULL OR
				NOT (@DATE BETWEEN t.ArrivalDate AND t.ReturnDate))
        ORDER BY (h.BaseRate + r.Price) * @People DESC

    IF @roomId IS NULL
	BEGIN
		RETURN 'No rooms available';
	END
 
    RETURN CONCAT('Room ',@roomId,': ',@TYPE ,' (',@beds,' beds',')',' - $',@totalPrice);
END
	-- ok 7/7 --

------------ solve 11'th Problem - new - last -----
GO

CREATE FUNCTION udf_GetAvailableRoom(@HotelId INT, @Date DATE, @People INT)
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE @result VARCHAR(MAX) = 
		(SELECT TOP(1) CONCAT('Room ', CONVERT(VARCHAR, r.Id),': ', CONVERT(VARCHAR, r.[Type]), ' (',CONVERT(VARCHAR, r.Beds), ' beds) - $',CONVERT(VARCHAR, ((h.BaseRate + r.Price) * @People))) 
			FROM Rooms AS r
			JOIN Hotels AS h ON h.Id = r.HotelId
			--JOIN Trips AS t ON t.RoomId = r.Id -- this check is made/moved below in the second SELECT
		WHERE r.Beds >= @People 
			AND r.HotelId = @HotelId
			AND 
				NOT EXISTS( SELECT * FROM Trips AS t
							WHERE @Date BETWEEN t.ArrivalDate AND t.ReturnDate 
										AND t.CancelDate IS NULL
										AND t.RoomId = r.Id)
		ORDER BY (h.BaseRate + r.Price) * @People DESC)

	IF(@result IS NULL)
	BEGIN
		SET @result = 'No rooms available';
		-- or --
		-- RETURN 'No rooms available';
	END

	RETURN @result;
END
	-- ok 7/7 --
GO

SELECT dbo.udf_GetAvailableRoom(112, '2011-12-17', 2)


-- 12. Switch Room --
GO
CREATE PROC usp_SwitchRoom(@TripId INT, @TargetRoomId INT)
AS
BEGIN 
	--If the target room ID is in a different hotel, than/where the trip is in, raise an exception with the message “Target room is in another hotel!”.
	DECLARE @currHotelId INT = (SELECT h.Id 
								FROM Trips AS t 
								JOIN Rooms AS r ON t.RoomId = r.Id
								JOIN Hotels AS h ON r.HotelId = h.Id
								WHERE t.Id = @TripId); --10) -- SELECT @currHotelId -- return Id 6
	--or--
	--SELECT r.HotelId FROM Trips AS t JOIN Rooms AS r ON t.RoomId = r.Id WHERE t.Id = @TripId --10 -> ok it's 6
	-----

	DECLARE @newHotelId INT = (SELECT r.HotelId 
								FROM Rooms AS r 
								WHERE r.Id = @TargetRoomId);

	IF(@currHotelId != @newHotelId)
	--BEGIN
		THROW 50001, 'Target room is in another hotel!', 1
	--END

	--If the target room doesn’t have enough beds for all the trip’s accounts, raise an exception with the message “Not enough beds in target room!”.
	DECLARE @countOfPeople INT = (SELECT COUNT(atr.AccountId) FROM AccountsTrips AS atr
									JOIN Trips AS t ON atr.TripId = t.Id
									WHERE t.Id = @TripId) --10) --SELECT @countOfPeople -- return 2
	-- or --
	--DECLARE @countOfPeople INT = (SELECT COUNT(*) FROM AccountsTrips AS atr WHERE atr.TripId = 10); --ok return 2
	-------
	DECLARE @newRoomCountOfBeds INT = (SELECT r.Beds FROM Rooms AS r WHERE r.Id = @TargetRoomId);

	IF(@newRoomCountOfBeds < @countOfPeople)
	--BEGIN
		THROW 50002, 'Not enough beds in target room!', 1
	--END

	--If all the above conditions pass, change the trip’s room ID to the target room ID.
	UPDATE Trips 
	SET RoomId = @TargetRoomId
	WHERE Id = @TripId
END

-- ok 7/7 --
GO

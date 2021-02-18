-- MS SQL Exam – 27 June 2020 - 
CREATE DATABASE WMS

USE WMS

--Section 1. DDL --
CREATE TABLE Clients(
	ClientId INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	Phone CHAR(12) CHECK(LEN(Phone) = 12) NOT NULL
	)

CREATE TABLE Mechanics(
	MechanicId INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	[Address] VARCHAR(255) NOT NULL,
	)

CREATE TABLE Models(
	ModelId INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(50) UNIQUE NOT NULL
	)

CREATE TABLE Jobs(
	JobId INT PRIMARY KEY IDENTITY NOT NULL,
	ModelId INT REFERENCES Models(ModelId) NOT NULL,
	[Status] VARCHAR(11) DEFAULT 'Pending' CHECK ([Status] IN ('Pending', 'In Progress', 'Finished')) NOT NULL,
	ClientId INT REFERENCES Clients(ClientId) NOT NULL,
	MechanicId INT REFERENCES Mechanics(MechanicId),
	IssueDate DATE NOT NULL,
	FinishDate DATE
	)

CREATE TABLE Orders(
	OrderId INT PRIMARY KEY IDENTITY NOT NULL,
	JobId INT REFERENCES Jobs(JobId) NOT NULL,
	IssueDate DATE,
	Delivered BIT DEFAULT 0 NOT NULL
	)

CREATE TABLE Vendors(
	VendorId INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(50) UNIQUE NOT NULL
	)

CREATE TABLE Parts(
	PartId INT PRIMARY KEY IDENTITY NOT NULL,
	SerialNumber VARCHAR(50) UNIQUE NOT NULL,
	[Description] VARCHAR(255),
	Price DECIMAL(6,2) CHECK(Price > 0) NOT NULL,
	VendorId INT REFERENCES Vendors(VendorId) NOT NULL,
	StockQty INT DEFAULT 0 CHECK(StockQty >= 0) NOT NULL
	)

CREATE TABLE OrderParts(
	OrderId INT REFERENCES Orders(OrderId) NOT NULL,
	PartId INT REFERENCES Parts(PartId) NOT NULL,
	Quantity INT DEFAULT 1 CHECK(Quantity > 0) NOT NULL
	PRIMARY KEY(OrderId, PartId)
	)

CREATE TABLE PartsNeeded(
	JobId INT REFERENCES Jobs(JobId) NOT NULL,
	PartId INT REFERENCES Parts(PartId) NOT NULL,
	Quantity INT DEFAULT 1 CHECK(Quantity > 0) NOT NULL
	PRIMARY KEY(JobId, PartId)
	)
	-- îê 30/30 --


-- Section 2. DML --
-- 2.	Insert --
INSERT INTO Clients(FirstName, LastName, Phone)
VALUES
('Teri', 'Ennaco', '570-889-5187'),
('Merlyn', 'Lawler', '201-588-7810'),
('Georgene', 'Montezuma', '925-615-5185'),
('Jettie', 'Mconnell', '908-802-3564'),
('Lemuel', 'Latzke', '631-748-6479'),
('Melodie', 'Knipp', '805-690-1682'),
('Candida', 'Corbley', '908-275-8357')

INSERT INTO Parts(SerialNumber, [Description], Price, VendorId)
VALUES
('WP8182119','Door Boot Seal', 117.86, 2),
('W10780048','Suspension Rod', 42.81, 1),
('W10841140','Silicone Adhesive', 6.77, 4),
('WPY055980','High Temperature Adhesive', 13.94, 3)
	-- îê 2/2 --

-- 3.	Update --
UPDATE Jobs
SET MechanicId = (SELECT MechanicId 
					FROM Mechanics 
					WHERE FirstName LIKE 'Ryan' AND LastName LIKE 'Harnos')
WHERE [Status] LIKE 'Pending'

UPDATE Jobs
SET [Status] = 'In Progress'
WHERE [Status] LIKE 'Pending'
	-- ok 4/4 --

-- or in one UPDATE, using comma (,) to write several SET's in a row --
UPDATE Jobs
SET MechanicId = 
	(SELECT MechanicId 
		FROM Mechanics 
		WHERE FirstName LIKE 'Ryan' AND LastName LIKE 'Harnos'), 
			[Status] = 'In Progress'
WHERE [Status] LIKE 'Pending'
-----------------------------

-- 4.	Delete --
DELETE FROM OrderParts WHERE OrderId = 19
DELETE FROM Orders WHERE OrderId = 19
	-- ok 4/4 --

	-- Section 3. Querying --
-- 5. Mechanic Assignments --
SELECT
	CONCAT(m.FirstName, ' ', m.LastName) AS [Mechanic], 
	j.[Status] AS [Status],
	j.IssueDate AS [IssueDate]
FROM Mechanics AS m
JOIN Jobs AS j ON m.MechanicId = j.MechanicId
ORDER BY m.MechanicId,
		 j.IssueDate,
		 j.JobId
	-- ok 3/3 --

-- 6.	Current Clients --
SELECT  CONCAT(c.FirstName, ' ', c.LastName) AS [Client],
		DATEDIFF(DAY, j.IssueDate, '24 April 2017') AS [Days going],
		j.[Status] AS [Status]
	FROM Clients AS c
JOIN Jobs AS j ON c.ClientId = j.ClientId
WHERE j.[Status] != 'Finished'
ORDER BY [Days going] DESC, 
		 c.ClientId
	-- ok 5/5 --

-- 7.	Mechanic Performance --
SELECT
		CONCAT(m.FirstName, ' ', m.LastName) AS [Mechanic],
		AVG(DATEDIFF(DAY, j.IssueDate, j.FinishDate)) AS [Average Days]
	FROM Mechanics AS m
JOIN Jobs AS j ON m.MechanicId = j.MechanicId
WHERE j.[Status] NOT LIKE 'In Progress'
GROUP BY m.MechanicId, m.FirstName, m.LastName
ORDER BY m.MechanicId
	-- ok 6/6 --

-- 8.	Available Mechanics --
SELECT
	CONCAT(m.FirstName, ' ', m.LastName) AS [Available] 
	FROM Mechanics AS m
LEFT JOIN Jobs AS j ON m.MechanicId = j.MechanicId
WHERE j.JobId IS NULL OR (
						  SELECT COUNT(j.MechanicId)
						  	FROM Jobs AS j
						  WHERE j.[Status] NOT LIKE 'Finished' AND j.MechanicId = m.MechanicId
						  GROUP BY j.MechanicId, j.[Status]
						  ) IS NULL
GROUP BY m.MechanicId, m.FirstName, m.LastName
ORDER BY m.MechanicId
	-- ok 7/7 --

	-- or -> this below is GOOD solved problem using ALL operator --
SELECT
	CONCAT(m.FirstName, ' ', m.LastName) AS [Available] 
	FROM Mechanics AS m
LEFT JOIN Jobs AS j ON m.MechanicId = j.MechanicId
WHERE j.JobId IS NULL 
					OR 'Finished' = ALL (
										 SELECT j.[Status]
										 	FROM Jobs AS j
										 WHERE j.MechanicId = m.MechanicId
										 )
GROUP BY m.MechanicId, m.FirstName, m.LastName
ORDER BY m.MechanicId
	-- ok 7/7 --


-- 9.	Past Expenses --
	-- (here IMPORTANT is to have LEFT JOIN and to RETURN '0' if NO parts were ordered for the job) --
SELECT j.JobId AS [JobID], 
	   ISNULL(SUM(p.Price * op.Quantity), 0) AS [Total] 
	FROM Jobs AS j
LEFT JOIN Orders AS o ON j.JobId = o.JobId
LEFT JOIN OrderParts AS op ON o.OrderId = op.OrderId
	 JOIN Parts AS p ON op.PartId = p.PartId
WHERE j.[Status] LIKE 'Finished'
GROUP BY j.JobId
ORDER BY [Total] DESC, 
		 j.JobId
	-- 7/7 --


-- 10.	Missing Parts --
SELECT p.PartId AS [PartId],
	   p.[Description] AS [Description],
	   SUM(pn.Quantity) AS [Requitred],
	   SUM(p.StockQty) AS [In Stock],
	   IIF(o.Delivered = 0, pn.Quantity, 0) AS [Ordered] -- or -> 0 AS [Ordered] <-??
	FROM Jobs AS j
LEFT JOIN PartsNeeded AS pn ON j.JobId = pn.JobId
LEFT JOIN Orders AS o ON j.JobId = o.JobId
	 JOIN Parts AS p ON pn.PartId = p.PartId
WHERE j.[Status] NOT LIKE 'Finished' AND o.Delivered IS NULL
GROUP BY p.PartId, p.[Description], o.Delivered, pn.Quantity
HAVING SUM(p.StockQty) < SUM(pn.Quantity)
ORDER BY [PartId]
	-- ok 12/12 --

	-- or --
SELECT p.PartId AS [PartId],
	   p.[Description] AS [Description],
	   pn.Quantity AS [Requitred],
	   p.StockQty AS [In Stock],
	   IIF(o.Delivered = 0, pn.Quantity, 0) AS [Ordered] -- or -> 0 AS [Ordered] <-??
	FROM Jobs AS j
LEFT JOIN Orders AS o ON j.JobId = o.JobId
LEFT JOIN OrderParts AS op ON o.OrderId = op.OrderId
LEFT JOIN PartsNeeded AS pn ON j.JobId = pn.JobId
	 JOIN Parts AS p ON pn.PartId = p.PartId
WHERE j.[Status] NOT LIKE 'Finished' 
					 AND (p.StockQty + IIF(o.Delivered = 0, op.Quantity, 0)) < pn.Quantity
ORDER BY [PartId]


	-- Section 4. Programmability --
-- 11.	Place Order --
GO
CREATE PROC usp_PlaceOrder(@JobId INT, @SerialNumber VARCHAR(50), @OrderQuantity INT)
AS
BEGIN
	-- The quantity cannot be zero or negative; error message ID 50012 "Part quantity must be more than zero!"
	IF(@OrderQuantity <= 0)
		THROW 50012, 'Part quantity must be more than zero!', 1
		
	-- The job with given ID must exist in the database; error message ID 50013 "Job not found!"
	DECLARE @checkJobExists INT = (SELECT j.JobId FROM Jobs AS j WHERE j.JobId = @JobId);

	IF(@checkJobExists IS NULL)
		THROW 50013, 'Job not found!', 1

	--	An order cannot be placed for a job that is Finished; error message ID 50011 "This job is not active!"
	DECLARE @checkJobIsInProgress VARCHAR(11) = (SELECT j.[Status] FROM Jobs AS j 
													WHERE j.JobId = @JobId)

	IF(@checkJobIsInProgress LIKE 'Finished')
		THROW 50011, 'This job is not active!', 1

	-- The part with given serial number must exist in the database ID 50014 "Part not found!"
	DECLARE @checkPartIdExists VARCHAR(50) = (SELECT p.PartId FROM Parts AS p WHERE p.SerialNumber = @SerialNumber);

	IF(@checkPartIdExists IS NULL)
		THROW 50014, 'Part not found!', 1


	-- If an order already exists for the given job that and the order is not issued (order’s issue date is NULL if oredr exist but the order is not sent to the client), add the new product to it...(we can add new product)
	DECLARE @checkIfOrderIdExists INT = (SELECT o.OrderId FROM Orders AS o WHERE o.JobId = @JobId AND o.IssueDate IS NULL);
	-- if the order does not exist(@checkIfOrderIdExists IS NULL), create new order...
	IF(@checkIfOrderIdExists IS NULL)
	BEGIN
		-- When a new order is created, set it’s IssueDate to NULL...
		INSERT INTO Orders(JobId, IssueDate)
		VALUES
			(@JobId, NULL)
	END

	--if order exists SET IssueDate to NULL
	SET @checkIfOrderIdExists = (SELECT o.OrderId FROM Orders AS o WHERE o.JobId = @JobId AND o.IssueDate IS NULL);

	-- after that check whether the part exists into the order...
	DECLARE @hasPartInThisOrder INT = (SELECT op.Quantity FROM OrderParts AS op WHERE op.OrderId = @checkIfOrderIdExists AND op.PartId = @checkPartIdExists);

	-- if do NOT exists ... (part quantityt IS NULL)
	IF(@hasPartInThisOrder IS NULL)
	BEGIN
		--add the new product into the existing order...
		INSERT INTO OrderParts(OrderId, PartId, Quantity)
		VALUES
			(@checkIfOrderIdExists, @checkPartIdExists, @OrderQuantity)
	END
	ELSE
	BEGIN
		--if part exist, add the given quantity into the existing order...
		UPDATE OrderParts
		SET Quantity += @OrderQuantity
		WHERE OrderId = @checkIfOrderIdExists 
	END
END
	-- ok 10/10 --

GO
-- 12.	Cost Of Order --
CREATE FUNCTION udf_GetCost(@JobId INT)
RETURNS DECIMAL(15,2)
AS
BEGIN
	DECLARE @result DECIMAL(15,2) = 
		(
		SELECT SUM(p.Price) AS [Result] 
			FROM Jobs AS j
		JOIN Orders AS o ON j.JobId = o.JobId
		JOIN OrderParts AS op ON o.OrderId = op.OrderId
		JOIN Parts AS p ON op.PartId = p.PartId
		WHERE j.JobId = @JobId
		)

	IF(@result IS NULL)
	BEGIN
		SET @result = 0;
	END

	RETURN @result;
END
	-- ok 10/10 --
GO

--  Exercises: Triggers and Transactions --

USE Bank

GO
-- Problem 1. Create Table Logs --
CREATE TABLE Logs(
	LogId INT PRIMARY KEY IDENTITY NOT NULL,
	AccountId INT FOREIGN KEY REFERENCES Accounts(Id),
	OldSum MONEY NOT NULL,
	NewSum MONEY NOT NULL
)

GO

CREATE TRIGGER tr_AccountChanges ON Accounts FOR UPDATE  
AS
BEGIN
	DECLARE @accountId INT = (SELECT Id FROM inserted)
	DECLARE @oldSum MONEY = (SELECT Balance FROM inserted)
	DECLARE @newSum MONEY = (SELECT Balance FROM deleted)

	INSERT INTO Logs(AccountId, OldSum, NewSum) -- here firs is oldSum after that is newSum
	VALUES
		(@accountId, @newSum, @oldSum) -- here firs is newSum after that is oldSum
END
	--  ok 100/100 --
GO

UPDATE Accounts
SET Balance -= 10
WHERE Id = 1

SELECT * FROM Accounts
WHERE Id = 1

SELECT * FROM Logs

-- Problem 2. Create Table Emails --
CREATE TABLE NotificationEmails(
	Id INT PRIMARY KEY IDENTITY NOT NULL, 
	Recipient INT FOREIGN KEY REFERENCES Accounts(Id) NOT NULL, 
	[Subject] NVARCHAR(200) NOT NULL, 
	Body NVARCHAR(500) NOT NULL
	)
	
GO
--CREATE TRIGGER tr_NotiicationsForChangesOnLogs ON Logs FOR INSERT 
--AS
--BEGIN 
--	DECLARE @recipient INT = (SELECT TOP(1) l.AccountId FROM Logs AS l)

--	DECLARE @subject NVARCHAR(200) = (CONCAT('Balance change for account: ', (SELECT TOP(1) l.AccountId FROM Logs AS l)))

--	DECLARE @oldBalance MONEY = (SELECT TOP(1) l.OldSum FROM Logs AS l)
--	DECLARE @newBalance MONEY = (SELECT TOP(1) l.NewSum FROM Logs AS l)

--	DECLARE @body NVARCHAR(500) = (CONCAT('On ', (SELECT FORMAT (GETDATE(), 'MMM dd yyyy hh:mm tt')), ' your balance was changed from ', @oldBalance, ' to ', @newBalance, '.'))

--	INSERT INTO NotificationEmails (Recipient, [Subject], Body)
--	VALUES
--		(@recipient, @subject, @body)
--END
--GO
	-- Judge -> 100/100,  but it affect the top 1 row into Logs --

GO
CREATE TRIGGER tr_NotiicationsForChangesOnLogs ON Logs FOR INSERT 
AS
BEGIN 
	DECLARE @recipient INT = (SELECT TOP(1) AccountId FROM inserted)

	DECLARE @oldBalance MONEY = (SELECT TOP(1) OldSum FROM inserted)
	DECLARE @newBalance MONEY = (SELECT TOP(1) NewSum FROM inserted)

	DECLARE @subject NVARCHAR(200) = (CONCAT('Balance change for account: ', @recipient))

	DECLARE @body NVARCHAR(500) = (CONCAT('On ', (SELECT FORMAT (GETDATE(), 'MMM dd yyyy hh:mm tt')), ' your balance was changed from ', @oldBalance, ' to ', @newBalance, '.'))

	INSERT INTO NotificationEmails (Recipient, [Subject], Body)
	VALUES
		(@recipient, @subject, @body)
END
		-- ok 100/100 ---
GO

--SELECT FORMAT (GETDATE(), 'MMM dd yyyy hh:mmtt') <- made output to required format --

GO

-- Problem 3. Deposit Money --
CREATE PROC usp_DepositMoney (@AccountId INT, @MoneyAmount DECIMAL(18,4))
AS
BEGIN TRANSACTION
	DECLARE @account INT = (SELECT Id FROM Accounts WHERE Id = @AccountId);

	--DECLARE @moneyToDeposite DECIMAL(18,4)
	--SET @moneyToDeposite = @MoneyAmount;

	IF(@MoneyAmount < 0)
	BEGIN
		--THROW 50001, 'Amount can not be negative number!', 1
		ROLLBACK
		RAISERROR('Amount can not be negative or equal to zero!', 16, 1)
		RETURN
	END

	IF(@account IS NULL)
	BEGIN
		--THROW 50002, 'Account is invalid!', 1
		ROLLBACK
		RAISERROR('Account is invalid!', 16, 1)
		RETURN
	END

	UPDATE Accounts
	SET Balance += @MoneyAmount
	WHERE Id = @account;

COMMIT
	-- 100/100 --
EXEC usp_DepositMoney 1, 20
SELECT * FROM Accounts WHERE ID = 1

GO
-- Problem 4. Withdraw Money --
CREATE PROC usp_WithdrawMoney (@AccountId INT, @MoneyAmount DECIMAL(15,4))
AS
BEGIN TRANSACTION
	DECLARE @account INT = (SELECT Id FROM Accounts WHERE Id = @AccountId);

	DECLARE @currBalance DECIMAL(15,4) = (SELECT Balance FROM Accounts WHERE Id = @AccountId);

	IF(@account IS NULL)
	BEGIN
		ROLLBACK
		RAISERROR('Invalid account!', 16, 1)
		RETURN
	END

	IF(@MoneyAmount < 0)
	BEGIN
		ROLLBACK
		RAISERROR('Amount must be positive number!', 16, 1)
		RETURN
	END

	IF(@currBalance < @MoneyAmount)
	BEGIN
		ROLLBACK
		RAISERROR('Money into account are less than the required withdraw!', 16, 1)
		RETURN
	END

	UPDATE Accounts
	SET Balance -= @MoneyAmount
	WHERE Id = @account;

COMMIT
	-- ok 100/100 --

EXEC usp_WithdrawMoney 5, 40000
SELECT * FROM Accounts WHERE ID = 5

GO
-- Problem 5. Money Transfer --
CREATE PROC usp_TransferMoney(@SenderId INT, @ReceiverId INT, @Amount DECIMAL(15,4))
AS
BEGIN TRANSACTION

	IF(@Amount < 0)
	BEGIN
		ROLLBACK
		RAISERROR('Amount must be positive number!', 16, 1)
		RETURN
	END

	EXEC usp_WithdrawMoney @SenderId, @Amount
	EXEC usp_DepositMoney @ReceiverId, @Amount

COMMIT
	-- ok 100/100 --
EXEC usp_TransferMoney 5, 1, 5000
EXEC usp_TransferMoney @SenderId = 5, @ReceiverId = 1, @Amount = 5000
SELECT * FROM Accounts WHERE Id = 1 OR Id = 5

GO

USE Diablo

GO
-- Problem 6. Trigger -- NOT to check in Judge --
	-- SubProblem 1. Users should not be allowed to buy items with higher level than their own  level. Create a trigger that restricts that. The trigger should prevent inserting items that are above specified level while allowing all others to be inserted.

SELECT * 
	FROM Users AS u
JOIN UsersGames AS ug ON u.Id = ug.UserId
JOIN UserGameItems AS ugi ON ug.GameId = ugi.UserGameId
JOIN Items AS i ON ugi.ItemId = i.Id

INSERT INTO UserGameItems(ItemId, UserGameId)
VALUES (2, 38)

INSERT INTO UserGameItems(ItemId, UserGameId)
VALUES (14, 38)

SELECT * FROM UserGameItems

GO

CREATE TRIGGER tr_PreventBuyItemsWIthHigherLevel ON UserGameItems INSTEAD OF INSERT
AS
BEGIN 
	DECLARE @itemId INT = (SELECT ItemId FROM inserted)
	DECLARE @userId INT = (SELECT UserGameId FROM inserted)

	DECLARE @itemLevel INT = (SELECT MinLevel FROM Items WHERE Id = @itemId)
	DECLARE @usersGamesLevel INT = (SELECT [Level] FROM UsersGames WHERE Id = @userId)

	IF(@itemLevel > @usersGamesLevel)
	BEGIN 
		ROLLBACK
		RAISERROR('Not allow to buy Items with level higer than the level of the User', 16, 1)
		RETURN
	END

	INSERT INTO UserGameItems(ItemId, UserGameId)
	VALUES 
		(@itemId, @userId)
END
	-- the trigger is working properly --

GO
	-- SubProblem 2. Add bonus cash of 50000 to users: baleremuda, loosenoise, inguinalself, buildingdeltoid, monoxidecos in the game "Bali".

UPDATE UsersGames
SET Cash += 50000
WHERE GameId = (SELECT Id FROM Games WHERE [Name] LIKE 'Bali')
		AND 
	  UserId IN (SELECT Id FROM Users WHERE Username IN ('baleremuda', 'loosenoise', 'inguinalself', 'buildingdeltoid', 'monoxidecos'))

--SELECT u.Username, ug.Cash, g.[Name] FROM Users AS u
--JOIN UsersGames AS ug ON u.Id = ug.UserId
--JOIN Games AS g ON ug.GameId = g.Id
--WHERE Username IN ('baleremuda', 'loosenoise', 'inguinalself', 'buildingdeltoid', 'monoxidecos')
--	AND g.[Name] LIKE 'Bali'
--ORDER BY u.Username

	-- SubProblem 3. There are two groups of items that you must buy for the above users. The first are items with id between 251 and 299 including. Second group are items with id between 501 and 539 including. Take off cash from each user for the bought items.
-- Here it is appropriate to create user stored-procedure --
GO

GO
CREATE OR ALTER PROC usp_BuyItems(@ItemId INT, @UserId INT, @GameName VARCHAR(100))
AS
BEGIN TRANSACTION
	DECLARE @currItemId INT = (SELECT Id FROM Items WHERE Id = @ItemId);

	IF(@currItemId IS NULL)
	BEGIN
		ROLLBACK
		RAISERROR('Unexisting item!', 16, 1)
		RETURN
	END;

	DECLARE @currUserId INT = (SELECT Id FROM Users WHERE Id = @UserId);

	IF(@currUserId IS NULL)
	BEGIN
		ROLLBACK
		RAISERROR('Unexisting user!', 16, 2)
		RETURN
	END;

	DECLARE @userCashForTheGame MONEY = (SELECT Cash FROM UsersGames AS ug
											JOIN Games AS g ON ug.GameId = g.Id
											JOIN Users AS u ON ug.UserId = u.Id
											WHERE g.[Name] = @GameName AND u.Id = @UserId --or ->ug.UserId = @UserId
										   );
	--DECLARE @userCashForTheGame MONEY = (SELECT Cash FROM UsersGames WHERE UserId = @UserId AND GameId = @GameId)

	DECLARE @currItemPrice MONEY = (SELECT Price FROM Items WHERE Id = @ItemId);
	
	IF((@userCashForTheGame - @currItemPrice) < 0)
	BEGIN
		ROLLBACK
		RAISERROR('Not enought money for this item!', 16, 2)
		RETURN
	END;

	DECLARE @gameNameId INT = (SELECT Id FROM Games WHERE [Name] = @GameName);

	UPDATE UsersGames
	SET Cash -= @currItemPrice
	WHERE UserId = @UserId AND GameId = @gameNameId;

	INSERT INTO UserGameItems (ItemId, UserGameId) 
	VALUES (@currItemId, @currUserId);

COMMIT

-- items from 251 to 299 and from 501 to 539
DECLARE @firstSetItemsToBuy INT = 251

WHILE(@firstSetItemsToBuy <= 299)
BEGIN

	EXEC usp_BuyItems @firstSetItemsToBuy, 12, 'Bali';
	EXEC usp_BuyItems @firstSetItemsToBuy, 22, 'Bali';
	EXEC usp_BuyItems @firstSetItemsToBuy, 37, 'Bali';
	EXEC usp_BuyItems @firstSetItemsToBuy, 52, 'Bali';
	EXEC usp_BuyItems @firstSetItemsToBuy, 61, 'Bali';

	SET @firstSetItemsToBuy += 1;
END;

DECLARE @secondSetItemsToBuy INT = 501;

WHILE(@secondSetItemsToBuy <= 539)
BEGIN

	EXEC usp_BuyItems @secondSetItemsToBuy, 12, 'Bali';
	EXEC usp_BuyItems @secondSetItemsToBuy, 22, 'Bali';
	EXEC usp_BuyItems @secondSetItemsToBuy, 37, 'Bali';
	EXEC usp_BuyItems @secondSetItemsToBuy, 52, 'Bali';
	EXEC usp_BuyItems @secondSetItemsToBuy, 61, 'Bali';

	SET @secondSetItemsToBuy += 1;
END;

GO

	-- SubProblem 4. Select all users in the current game ("Bali") with their items. Display username, game name, cash and item name. Sort the result by username alphabetically, then by item name alphabetically. 

SELECT u.Username, g.[Name], ug.Cash, i.[Name] 
	FROM Users AS u
JOIN UsersGames AS ug ON u.Id = ug.UserId
JOIN Games AS g ON ug.GameId = g.Id
JOIN UserGameItems AS ugi ON ug.Id = ugi.UserGameId
JOIN Items AS i ON ugi.ItemId = i.Id
WHERE g.[Name] LIKE 'Bali'
ORDER BY u.Username, i.[Name]


-- Problem 7. *Massive Shopping --

	--DECLARE @groupOfItems INT = (SELECT Id FROM Items 
	--							 WHERE (Id BETWEEN 251 AND 299) OR (Id BETWEEN 501 AND 539));
	--DECLARE @groupOfUsers INT = (SELECT u.Id  
	--								FROM Users AS u
	--							JOIN UsersGames AS ug ON u.Id = ug.UserId
	--							JOIN Games AS g ON ug.GameId = g.Id
	--							WHERE Username IN ('baleremuda', 'loosenoise', 'inguinalself', 'buildingdeltoid', 'monoxidecos')
	--							AND g.[Name] LIKE @GameName
	--							);

USE SoftUni
GO
-- Problem 8. Employees with Three Projects --
CREATE PROC usp_AssignProject(@EmployeeID INT, @ProjectID INT)
AS
BEGIN TRANSACTION
	DECLARE @currEmployeeID INT = (SELECT EmployeeID FROM Employees WHERE EmployeeID = @EmployeeID);
	DECLARE @currProjecttID INT = (SELECT ProjectID FROM Projects WHERE ProjectID = @ProjectID);

	IF(@currEmployeeID IS NULL)
	BEGIN
		ROLLBACK
		RAISERROR ('No sutch Employee', 16, 1);
		RETURN
	END

	IF(@currProjecttID IS NULL)
	BEGIN
		ROLLBACK
		RAISERROR ('No sutch Project', 16, 1);
		RETURN
	END

	DECLARE @countProjectsCurrEmployee INT = (SELECT COUNT(*) FROM EmployeesProjects WHERE EmployeeID = @EmployeeID);

	IF(@countProjectsCurrEmployee >= 3)
	BEGIN
		ROLLBACK
		RAISERROR ('The employee has too many projects!', 16, 1);
		RETURN
	END

	DECLARE @checkCurrUserIsInProject INT = (SELECT ProjectId FROM EmployeesProjects 
											 WHERE EmployeeID = @EmployeeID AND ProjectID = @ProjectID);

	IF(@checkCurrUserIsInProject IS NOT NULL)
	BEGIN
		ROLLBACK
		RAISERROR ('The employee is already into this projects!', 16, 1);
		RETURN
	END

	INSERT INTO EmployeesProjects(EmployeeID, ProjectID)
	VALUES
		(@EmployeeID, @ProjectID)

COMMIT
-- ok 100/100 --

EXEC usp_AssignProject 2, 3

SELECT * FROM EmployeesProjects WHERE EmployeeID = 2

GO
-- Problem 9. Delete Employees --
CREATE TABLE Deleted_Employees(
	EmployeeId INT PRIMARY KEY IDENTITY NOT NULL, 
	FirstName VARCHAR(50) NOT NULL, 
	LastName VARCHAR(50) NOT NULL, 
	MiddleName VARCHAR(50), 
	JobTitle VARCHAR(50) NOT NULL, 
	DepartmentId INT NOT NULL, 
	Salary MONEY NOT NULL
	)

GO

CREATE TRIGGER tr_Record_Deleted_Employees ON Employees FOR DELETE
AS
BEGIN
	DECLARE @FirstName VARCHAR(50) = (SELECT FirstName FROM deleted);
	DECLARE @LastName VARCHAR(50) = (SELECT LastName FROM deleted);
	DECLARE @MiddleName VARCHAR(50) = (SELECT MiddleName FROM deleted);
	DECLARE @JobTitle VARCHAR(50) = (SELECT JobTitle FROM deleted);
	DECLARE @DepartmentId INT = (SELECT DepartmentID FROM deleted);
	DECLARE @Salary MONEY = (SELECT Salary FROM deleted);

	INSERT INTO Deleted_Employees (FirstName, LastName, MiddleName, JobTitle, DepartmentId, Salary)
	VALUES
		(@FirstName, @LastName, @MiddleName, @JobTitle, @DepartmentId, @Salary)
END

DELETE FROM Employees
WHERE EmployeeID = 291

SELECT * FROM Employees

SELECT * FROM Deleted_Employees
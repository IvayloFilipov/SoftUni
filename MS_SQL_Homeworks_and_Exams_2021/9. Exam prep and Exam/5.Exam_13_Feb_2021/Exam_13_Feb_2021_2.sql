-- Exam 13 Feb 2021 --
-- Section 1. DDL (30 pts) --
CREATE DATABASE Bitbucket

USE Bitbucket
-- 1.	Database Design --
CREATE TABLE Users(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	Username VARCHAR(30) NOT NULL,
	[Password] VARCHAR(30) NOT NULL,
	Email VARCHAR(50) NOT NULL
	)

CREATE TABLE Repositories(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(50) NOT NULL
	)

CREATE TABLE RepositoriesContributors(
	RepositoryId INT REFERENCES Repositories(Id) NOT NULL,
	ContributorId INT REFERENCES Users(Id) NOT NULL,
	PRIMARY KEY (RepositoryId, ContributorId)
	)

CREATE TABLE Issues(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	Title VARCHAR(255) NOT NULL,
	IssueStatus CHAR(6) NOT NULL,
	RepositoryId INT FOREIGN KEY REFERENCES Repositories(Id) NOT NULL,
	AssigneeId INT FOREIGN KEY REFERENCES Users(Id) NOT NULL
	)

CREATE TABLE Commits(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Message] VARCHAR(255) NOT NULL,
	IssueId INT FOREIGN KEY REFERENCES Issues(Id),
	RepositoryId INT FOREIGN KEY REFERENCES Repositories(Id) NOT NULL,
	ContributorId INT FOREIGN KEY REFERENCES Users(Id) NOT NULL
	)

CREATE TABLE Files(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] VARCHAR(100) NOT NULL,
	Size DECIMAL(15,2) NOT NULL,
	ParentId INT FOREIGN KEY REFERENCES Files(Id),
	CommitId INT FOREIGN KEY REFERENCES Commits(Id) NOT NULL
	)
	-- ok 30/30 --


	-- Section 2. DML (10 pts) --
-- 2.	Insert --
INSERT INTO Files([Name], Size, ParentId, CommitId)
VALUES
('Trade.idk', 2598.0, 1, 1),
('menu.net', 9238.31, 2, 2),
('Administrate.soshy', 1246.93, 3, 3),
('Controller.php', 7353.15, 4, 4),
('Find.java', 9957.86, 5, 5),
('Controller.json', 14034.87, 3, 6),
('Operate.xix', 7662.92, 7, 7)

INSERT INTO Issues(Title, IssueStatus, RepositoryId, AssigneeId)
VALUES
('Critical Problem with HomeController.cs file', 'open', 1, 4),
('Typo fix in Judge.html', 'open', 4, 3),
('Implement documentation for UsersService.cs', 'closed', 8, 2),
('Unreachable code in Index.cs', 'open', 9, 8)
	-- ok 3/3 --

-- 3.	Update --
UPDATE Issues
SET IssueStatus = 'closed'
WHERE AssigneeId = 6
	-- ok 3/3 --

-- 4.	Delete --
DELETE FROM RepositoriesContributors
WHERE RepositoryId = 3

DELETE FROM Issues
WHERE RepositoryId = 3
	-- ok 4/4 --

	-- Section 3. Querying (40 pts) --
-- 5.	Commits --
SELECT Id AS Id, 
		[Message] AS [Message],
		RepositoryId, 
		ContributorId 
FROM Commits
ORDER BY Id ASC, 
		[Message] ASC, 
		RepositoryId ASC, 
		ContributorId ASC
	-- ok 2/2 --

-- 6.	Front-end --
SELECT Id, [Name], Size 
FROM Files
WHERE Size > 1000 AND [Name] LIKE '%html%'
ORDER BY Size DESC, Id ASC, [Name] ASC
	-- ok 4/4 --

-- 7.	Issue Assignment --
SELECT i.Id, 
		CONCAT(u.Username, ' ', ':', ' ', i.Title) AS [IssueAssignee]
FROM Issues AS i
JOIN Users AS u ON i.AssigneeId = u.Id
ORDER BY i.Id DESC, i.AssigneeId
	-- ok 8/8 --

-- 8. Single Files --
SELECT f.Id, f.[Name], CONCAT(f.Size,'KB') AS Size
	FROM Files AS f
LEFT JOIN Files AS f2 ON f.Id = f2.ParentId
WHERE f2.ParentId IS NULL
ORDER BY f.Id, 
		 f.[Name], 
		 f.Size DESC
			-- ok 8/8 --

	--- or ---
SELECT Id, [Name], CAST(Size AS VARCHAR) + 'KB' AS Size 
	FROM Files
WHERE Id NOT IN (
				SELECT ParentId FROM Files 
				WHERE ParentId IS NOT NULL
				)
ORDER BY Id, 
		[Name], 
		Size DESC


-- 9.	Commits in Repositories --
--Задача 9 - Трябва селектирате TOP(5) Id, Name, COUNT(*) от RepositoriesContributors и да ги JOIN-ете с Repositories и Commits
SELECT TOP(5) r.id, r.Name,  COUNT(c.id) as Commits 
	FROM RepositoriesContributors AS rc
JOIN Repositories AS r ON rc.RepositoryId = r.Id
JOIN Commits AS c ON r.Id = c.RepositoryId
GROUP BY r.Id, r.Name
ORDER BY Commits desc, r.id, r.Name
	-- ok 10/10 --

-- 10.	Average Size --
SELECT u.Username, AVG(f.Size) AS AvgSise 
	FROM Users AS u
 JOIN Commits AS c ON u.Id = c.ContributorId
 JOIN Files AS f ON c.Id = f.CommitId
GROUP BY  u.Username 
ORDER BY AvgSise DESC, u.Username
	-- ok 8/8 -

-- 11.	All User Commits --
GO
CREATE FUNCTION udf_AllUserCommits(@username VARCHAR(30))
RETURNS INT
AS
BEGIN
	DECLARE @result INT = (
				SELECT COUNT(c.[Message]) AS [Output] 
					FROM Users AS u
				JOIN Commits AS c ON u.Id = c.ContributorId
				WHERE u.Username = @username
				)

	RETURN @result;
END
	-- ok 10/10 --
GO

-- 12. Search for Files --
CREATE PROC usp_SearchForFiles(@fileExtension VARCHAR(50))
AS
BEGIN
		SELECT Id, 
		[Name], 
		CONCAT(Size,'KB') AS [Size] 
		FROM Files
	WHERE [Name] LIKE ('%' + @fileExtension)
	ORDER BY Id, [Name], [Size] DESC 

END
-- ok 10/10 --
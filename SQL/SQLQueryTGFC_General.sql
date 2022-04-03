-- Author: Thibaut Deliever
-- Query to create and initialize the database in msserver for the application 'TheGreatFinChallenge'.

-- Quick Commands for resets etc.
-- 1) DROP <DatabaseName>			(Complete loss of the database)
-- 2) DROP <TableName>				(Complete loss of the table)
-- 3) TRUNCATE TABLE <TableName>	(Delete of data, not the table itself)
-- 4) ALTER TABLE <TableName>		(Add of Remove specific column in table)
--    ADD <ColumnName> <DataType>
--    DROP COLUMN <ColumnName>
-- 5) INSERT INTO <TableName> (<DataType>, ...)
--    VALUES (<Data, ...)


--Initialize Database 'TheGreatFinChallenge' + tables.
--Make sure that 'TheGreatFinChallenge' and its subtables do not already exist in msserver.
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'TheGreatFinChallenge')
BEGIN
	CREATE DATABASE TheGreatFinChallenge
END
GO

USE TheGreatFinChallenge
GO

DROP TABLE IF EXISTS [Image];
DROP TABLE IF EXISTS [Activity];
DROP TABLE IF EXISTS [ActivityType];
DROP TABLE IF EXISTS [Discipline];
DROP TABLE IF EXISTS [User];
DROP TABLE IF EXISTS [Department];
DROP TABLE IF EXISTS [Directorate];


--Initialize Tables inside Database 'TheGreatFinChallenge'.
USE TheGreatFinChallenge
GO

CREATE TABLE [Directorate] (
	--Int				DirectorateId
	--Nvarchar			Name
	--Nvarchar			NameNormalized
	--DateTime			ChallengeStartDate
	--DateTime			ChallengeEndDate			
	DirectorateId		INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Name]				NVARCHAR(100) NOT NULL,
	[NameNormalized]	NVARCHAR(100) NOT NULL,
	ChallengeStartDate 	DATETIME NULL,
	ChallengeEndDate	DATETIME NULL
)

CREATE TABLE [Department] (
	--Int				DepartmentId
	--Int				DirectorateId		FK to DirectoryId
	--Nvarchar			Name
	--Nvarchar			NameNormalized
	--Int				AmountOfEmployees
	DepartmentId		INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	DirectorateId		INT FOREIGN KEY REFERENCES [Directorate](DirectorateId) NOT NULL,
	[Name]				NVARCHAR(100) NOT NULL,
	[NameNormalized]	NVARCHAR(100) NOT NULL,
	AmountOfEmployees 	INT NOT NULL
)

CREATE TABLE [User] (
	--Int			UserId			
	--Nvarchar		FirstName		
	--Nvarchar		LastName		
	--Bit			Admin		Admin=1; User=0
	--Nvarchar		Email			
	--Nvarchar		Password	
	--Nvarchar		Salt 	
	--Bit			GDPR		PublicActivities=1; PrivateActivities=0
	UserId			INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	DepartmentId	INT FOREIGN KEY REFERENCES [Department](DepartmentId) NOT NULL,
	FirstName		NVARCHAR(50) NOT NULL,
	LastName		NVARCHAR(50) NOT NULL,
	[Admin]			BIT NOT NULL,
	Email			NVARCHAR(100) UNIQUE NOT NULL,
	[Password] 		NVARCHAR(255) NOT NULL,
	[Salt]			NVARCHAR(255) NOT NULL,
	[Gdpr] 			BIT NOT NULL
)

CREATE TABLE [Discipline] (
	--Int				DisciplineId	
	--Nvarchar			Name	
	--Nvarchar			Color	
	--Varbinary			ImageData		
	--Varbinary			IconData	
	DisciplineId		INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Name]				NVARCHAR(100) NULL,
	[NameNormalized]	NVARCHAR(100) NULL,
	[Color]				NVARCHAR(7) NULL,
	[ImageData]			VARBINARY(MAX) NULL,
	[IconData]			VARBINARY(MAX) NULL
)


CREATE TABLE [ActivityType] (
	--Int			ActivityTypeId	
	--Int			DisciplineId	FK to UserId
	--Nvarchar		Name
	--Decimal		MET		
	--Varbinary		ImageData
	ActivityTypeId	INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	DisciplineId	INT FOREIGN KEY REFERENCES [Discipline](DisciplineId) NOT NULL,
	[Name]			NVARCHAR(100) NOT NULL,
	MET				FLOAT NOT NULL,
	[ImageData]		VARBINARY(MAX) NOT NULL
)

CREATE TABLE [Activity] (
	--Int			ActivityId	
	--Int			UserId			FK to UserId
	--Int			ActivityTypeId	FK to ActivityTypeId
	--Float			Distance			
	--Datetime		StartTime		
	--DateTime		EndTime		
	ActivityId		INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	UserId			INT FOREIGN KEY REFERENCES [User](UserId) NOT NULL,
	ActivityTypeId	INT FOREIGN KEY REFERENCES [ActivityType](ActivityTypeId) NOT NULL,
	Distance		FLOAT NOT NULL,
	StartTime		DATETIME NOT NULL,
	EndTime			DATETIME NOT NULL
)

CREATE TABLE [Image] (
	--Int			ImageId			
	--Int			UserId
	--Nvarchar		ImageName
	--Varbinary		ImageData
	--Bit			Public
	ImageId 		INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
    UserId 			INT FOREIGN KEY REFERENCES [User](UserId) NOT NULL,
	ImageData 		VARBINARY(MAX) NOT NULL
)


--Initialize data for table dbo.Directorate in database 'TheGreatFinChallenge'.
--Unique directorates
INSERT INTO [Directorate]([Name], [NameNormalized])
VALUES 
('Antwerpen', 'antwerpen'),
('Brussel', 'brussel'),
('Gent', 'gent'),
('Namen', 'namen');


--Initialize data for table dbo.Department in database 'TheGreatFinChallenge'.
--Unique departments
INSERT INTO [Department](DirectorateId, [Name], [NameNormalized], AmountOfEmployees)
VALUES 
(3, 'Directie', 'directie', 1),
(3, 'Informaticacel & Douanecel', 'informaticacelAndDouanecel', 1),
(3, 'Geschillen', 'geschillen', 1),
(3, 'Invordering', 'invordering', 1),
(3, 'Input', 'input', 1),
(3, 'Inspectie Gent 1', 'inspectieGent1', 1),
(3, 'Inspectie Gent 2', 'inspectieGent2', 1),
(3, 'Inspectie Brugge', 'inspectieBrugge', 1);


--Initialize data for table dbo.User in database 'TheGreatFinChallenge'.
--Unique users with own email and password: @Dummie123
--Password: gqcDhyRRIRr3FEm1QMiToj28k4W65cy3X/nsHLarE1M=
--Salt: XaY6iCflqGpd18sBBt/UdpV2psxfhMaNiV78j3Wl0KU=
INSERT INTO [User](DepartmentId, FirstName, LastName, [Admin], Email, [Password], [Salt], [Gdpr])
VALUES (1, 'Thibaut', 'Deliever', 1, 'thibaut.deliever@dummie.be', 'gqcDhyRRIRr3FEm1QMiToj28k4W65cy3X/nsHLarE1M=', 'XaY6iCflqGpd18sBBt/UdpV2psxfhMaNiV78j3Wl0KU=', 1),
(2, 'Thibaut1', 'Deliever', 1, 'thibaut1.deliever@dummie.be', 'gqcDhyRRIRr3FEm1QMiToj28k4W65cy3X/nsHLarE1M=', 'XaY6iCflqGpd18sBBt/UdpV2psxfhMaNiV78j3Wl0KU=', 1),
(3, 'Thibaut3', 'Deliever', 0, 'thibaut3.deliever@dummie.be', 'gqcDhyRRIRr3FEm1QMiToj28k4W65cy3X/nsHLarE1M=', 'XaY6iCflqGpd18sBBt/UdpV2psxfhMaNiV78j3Wl0KU=', 1);


--Initialize data for table dbo.Discipline in database 'TheGreatFinChallenge'.
--Unique disciplines
INSERT INTO [Discipline]([Name], [NameNormalized], [Color], ImageData, IconData)
VALUES 
('Cycling', 'cycling', '#800000', (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\disciplines\cycling.png', SINGLE_BLOB) as img), (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\icons\cycling-solid.svg', SINGLE_BLOB) as T1)),
('Hiking', 'hiking', '#fabed4', (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\disciplines\hiking.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\icons\hiking-solid.svg', SINGLE_BLOB) as T1)),
('Running', 'running', '#808000', (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\disciplines\running.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\icons\running-solid.svg', SINGLE_BLOB) as T1)),
('Swimming', 'swimming', '#469990', (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\disciplines\swimming.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\icons\swimming-solid.svg', SINGLE_BLOB) as T1)),
('Ball Sports', 'ballSports', '#000075', (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\disciplines\ball.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\icons\ball-solid.svg', SINGLE_BLOB) as T1)),
('Racket Sports', 'racketSports', '#e6194B', (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\disciplines\racket.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\icons\racket-solid.svg', SINGLE_BLOB) as T1)),
('Martial Arts', 'martialArts', '#ffe119', (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\disciplines\combat.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\icons\combat-solid.svg', SINGLE_BLOB) as T1)),
('Fitness', 'fitness', '#bfef45', (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\disciplines\fitness.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\icons\fitness-solid.svg', SINGLE_BLOB) as T1)),
('Winter Sports', 'winterSports', '#42d4f4', (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\disciplines\winter.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\icons\winter-solid.svg', SINGLE_BLOB) as T1)),
('Summer Sports', 'summerSports', '#911eb4', (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\disciplines\summer.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\icons\summer-solid.svg', SINGLE_BLOB) as T1)),
('Horse Riding', 'horseRiding', '#f032e6', (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\disciplines\horse.png', SINGLE_BLOB) as T1), (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\icons\horse-solid.svg', SINGLE_BLOB) as T1));


--Initialize data for table dbo.ActivityTypes in database 'TheGreatFinChallenge'.
--Unique activitytypes
INSERT INTO [ActivityType]([DisciplineId], [Name], MET, ImageData)
VALUES 
(1, 'Analogue bike - 16 to 19 km/h', 7, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\bike.png', SINGLE_BLOB) as img)),
(1, 'Analogue bike - 19 to 22 km/h', 8, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\bike.png', SINGLE_BLOB) as img)),
(1, 'Analogue bike - 22 to 25 km/h', 10, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\bike.png', SINGLE_BLOB) as img)),
(1, 'Analogue bike - 25 to 30 km/h', 12, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\bike.png', SINGLE_BLOB) as img)),
(1, 'Analogue bike - 30+ km/h', 16, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\bike.png', SINGLE_BLOB) as img)),
(1, 'E-bike - 16 to 19 km/h', 3.5, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\ebike.png', SINGLE_BLOB) as img)),
(1, 'E-bike - 19 to 22 km/h', 4, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\ebike.png', SINGLE_BLOB) as img)),
(1, 'E-bike - 22 to 25 km/h', 5, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\ebike.png', SINGLE_BLOB) as img)),
(1, 'E-Bike - 25 to 30 km/h', 6, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\ebike.png', SINGLE_BLOB) as img)),
(1, 'E-Bike - 30+ km/h', 8, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\ebike.png', SINGLE_BLOB) as img)),
(1, 'Mountainbike - Offroad', 9, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\mbike.png', SINGLE_BLOB) as img)),

(2, 'Hiking', 4.3, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\hiking.png', SINGLE_BLOB) as img)),

(3, '0 to 6 km/h', 6, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\running.png', SINGLE_BLOB) as img)),
(3, '6 to 8 km/h', 8.3, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\running.png', SINGLE_BLOB) as img)),
(3, '8 to 10 km/h', 9.8, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\running.png', SINGLE_BLOB) as img)),
(3, '10 to 11 km/h', 11, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\running.png', SINGLE_BLOB) as img)),
(3, '11 to 13 km/h', 11.8, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\running.png', SINGLE_BLOB) as img)),
(3, '13 to 14 km/h', 12.8, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\running.png', SINGLE_BLOB) as img)),
(3, '14 to 17 km/h', 14.5, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\running.png', SINGLE_BLOB) as img)),
(3, '17 to 19 km/h', 16, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\running.png', SINGLE_BLOB) as img)),
(3, '19+ km/h', 19, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\running.png', SINGLE_BLOB) as img)),

(4, 'Swimming', 6.8, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\swimming.png', SINGLE_BLOB) as img)),

(5, 'Basketball', 6.5, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\basketball.png', SINGLE_BLOB) as img)),
(5, 'Golf', 4.8, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\golf.png', SINGLE_BLOB) as img)),
(5, 'Handball', 8, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\handball.png', SINGLE_BLOB) as img)),
(5, 'Field Hockey', 7.8, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\hockey.png', SINGLE_BLOB) as img)),
(5, 'Football', 6.3, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\football.png', SINGLE_BLOB) as img)),
(5, 'Soccer', 7, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\soccer.png', SINGLE_BLOB) as img)),
(5, 'Volley-ball', 4, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\volley-ball.png', SINGLE_BLOB) as img)),

(6, 'Badminton', 5.5, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\badminton.png', SINGLE_BLOB) as img)),
(6, 'Padel', 5.3, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\padel.png', SINGLE_BLOB) as img)),
(6, 'Squash', 7.3, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\squash.png', SINGLE_BLOB) as img)),
(6, 'Table Tennis', 4, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\ttennis.png', SINGLE_BLOB) as img)),
(6, 'Tennis', 7.3, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\tennis.png', SINGLE_BLOB) as img)),

(7, 'Martial Arts', 5.3, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\martialarts.png', SINGLE_BLOB) as img)),

(8, 'Cardio - Strength training', 7.8, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\cardio.png', SINGLE_BLOB) as img)),
(8, 'Dancing - Aerobics', 7.3, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\aerobics.png', SINGLE_BLOB) as img)),
(8, 'Workout (General)', 6, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\workout.png', SINGLE_BLOB) as img)),

(9, 'Skiing - Snowboarding', 7, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\skiing.png', SINGLE_BLOB) as img)),
(9, 'Ice skating', 5.5, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\iceskating.png', SINGLE_BLOB) as img)),

(10, 'Roller skating', 7, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\rskating.png', SINGLE_BLOB) as img)),
(10, 'Inline skating', 9.8, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\iskating.png', SINGLE_BLOB) as img)),
(10, 'surfing - Sailing', 3.3, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\surfing.png', SINGLE_BLOB) as img)),

(11, 'Horse riding', 5.5, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\horse.png', SINGLE_BLOB) as img));


--Initialize data for table dbo.Images in database 'TheGreatFinChallenge'.
--Unique images
--Initialize Tables inside Database 'TheGreatFinChallenge'.
USE TheGreatFinChallenge
GO
INSERT INTO [Image]([UserId], ImageData)
VALUES 
(1, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\surfing.png', SINGLE_BLOB) as img)),
(2, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\iskating.png', SINGLE_BLOB) as img)),
(3, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\skiing.png', SINGLE_BLOB) as img)),
(1, (SELECT * FROM OPENROWSET(BULK N'C:\Users\thiba\Documents\GithubRepos\TheGreatFinChallenge\TheGreatFinChallenge\wwwroot\img\activityTypes\horse.png', SINGLE_BLOB) as img));
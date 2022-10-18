/* Check whether the database exists and delete if so */

IF EXISTS(SELECT 1 FROM master.dbo.sysdatabases 
		  WHERE name = 'BuildingManager')
BEGIN
	DROP DATABASE [BuildingManager]
	print '' print '*** dropping BuildingManager'
END
GO

print '' print '*** creating BuildingManager'
GO
CREATE DATABASE [BuildingManager]
GO

print '' print '*** using BuildingManager'
GO
USE [BuildingManager]
GO

print '' print '*** creating table Users'
GO
CREATE TABLE [dbo].[Users](
	[UserID]		[int]IDENTITY(100,1)	NOT NULL,
	[FirstName]		[nvarchar](50)			NOT NULL,
	[LastName]		[nvarchar](50)			NOT NULL,
	[Email]			[nvarchar](250)			NOT NULL,	
	[PasswordHash][nvarchar](100)			NOT NULL DEFAULT
	'9C9064C59F1FFA2E174EE754D2979BE80DD30DB552EC03E7E327E9B1A4BD594E',
	[Active]			[bit]				NOT NULL DEFAULT 1,
	CONSTRAINT [pk_Users_UserID] 
		PRIMARY KEY([UserID] ASC)
)
print '' print '*** adding index for LastName on the Users table'
GO
CREATE NONCLUSTERED INDEX [ix_Users_LastName]
	on [Users] ([LastName] ASC)

GO

print '' print '*** adding index for Email on Users table'
GO
CREATE NONCLUSTERED INDEX [ix_Users_Email]
	on [Users] ([Email] ASC)
GO

print '' print '*** adding index for Active on Users table'
GO
CREATE NONCLUSTERED INDEX [ix_Users_Active]
	on [Users] ([Active] ASC)
GO

print '' print '*** creating Role Table'
GO
CREATE TABLE [dbo].[Role](
		[RoleID] 	[nvarchar](50) 			NOT NULL,
		[Description]	[nvarchar](150)		NULL,
		CONSTRAINT [pk_Role_RoleID] PRIMARY KEY([RoleID] ASC)
)
GO

print '' print '*** creating UserRole table'
GO
CREATE TABLE [dbo].[UserRole](
	[UserID]		[int]					NOT NULL,
	[RoleID]		[nvarchar](50)			NOT NULL,
	CONSTRAINT [pk_UserID_RoleID] 
		PRIMARY KEY([UserID] ASC, [RoleID] ASC),
	CONSTRAINT [fk_role_UserID] FOREIGN KEY([UserID])
		REFERENCES [Users]([UserID]) ON UPDATE CASCADE,
	CONSTRAINT [fk_role_roleID] FOREIGN KEY([RoleID])
		REFERENCES [Role]([RoleID]) ON UPDATE CASCADE
)
GO

print '' print '*** creating Room table'
GO
CREATE TABLE [dbo].[Room](
	[RoomID]		[nvarchar](4)			NOT NULL,
	[RoomNotes]		[nvarChar](100)			NULL,
	
	CONSTRAINT [pk_RoomID] 
		PRIMARY KEY([RoomID])
)
GO

print '' print '*** creating Security table'
GO
CREATE TABLE [dbo].[Security](
	[SPanelID]		[nvarchar](15)			NOT NULL,
	[TestLast]		[DateTime]				NOT NULL,
	[TestNext]		[DateTime]				Null,
	CONSTRAINT [pk_SPanelID] 
		PRIMARY KEY([SPanelID])
)
GO

print '' print '*** FireAlarm FireAlarm table'
GO
CREATE TABLE [dbo].[FireAlarm](
	[FPanelID]		[nvarchar](15)			NOT NULL,
	[TestLast]		[DateTime]				NOT NULL,
	[TestNext]		[DateTime]				Null,
	CONSTRAINT [pk_FPanelID] 
		PRIMARY KEY([FPanelID])
)
GO

print '' print '*** creating Thermostat table'
GO
CREATE TABLE [dbo].[Thermostat](
	[ThermostatID]		[nvarchar](15)			NOT NULL,
	[Temp]				[int]					NOT NULL,
	[TempUnit]			[nvarchar](2)			NOT NULL,
	[FanSetting]		[nvarchar](5)			NOT NULL,
	[CoreSetting]		[nvarchar](5)			NOT NULL,
	CONSTRAINT [pk_ThermostatID] 
		PRIMARY KEY([ThermostatID])
)
GO

print '' print '*** creating RoomThermo table'
GO
CREATE TABLE [dbo].[RoomThermo](
	[RoomID]			[nvarchar](4)			NOT NULL,
	[ThermostatID]		[nvarchar](15)			NOT NULL,
	CONSTRAINT [pk_RoomID_ThermoID] 
		PRIMARY KEY([RoomID] ASC, [ThermostatID] ASC),
	CONSTRAINT [fk_Thermo_RoomID] FOREIGN KEY([RoomID])
		REFERENCES [Room]([RoomID]) ON UPDATE CASCADE,
	CONSTRAINT [fk_Thermo_ThermoID] FOREIGN KEY([ThermostatID])
		REFERENCES [Thermostat]([ThermostatID]) ON UPDATE CASCADE
)
GO

print '' print '*** creating RoomSecurity table'
GO
CREATE TABLE [dbo].[RoomSecurity](
	[RoomID]			[nvarchar](4)			NOT NULL,
	[SPanelID]			[nvarchar](15)			NOT NULL,
	CONSTRAINT [pk_RoomID_SPanelID] 
		PRIMARY KEY([RoomID] ASC, [SPanelID] ASC),
	CONSTRAINT [fk_Security_RoomID] FOREIGN KEY([RoomID])
		REFERENCES [Room]([RoomID]) ON UPDATE CASCADE,
	CONSTRAINT [fk_Security_SPanelID] FOREIGN KEY([SPanelID])
		REFERENCES [Security]([SPanelID]) ON UPDATE CASCADE
)
GO

print '' print '*** creating RoomFireAlarm table'
GO
CREATE TABLE [dbo].[RoomFireAlarm](
	[RoomID]			[nvarchar](4)			NOT NULL,
	[FPanelID]			[nvarchar](15)			NOT NULL,
	CONSTRAINT [pk_RoomID_FPanelID] 
		PRIMARY KEY([RoomID] ASC, [FPanelID] ASC),
	CONSTRAINT [fk_FireAlarm_RoomID] FOREIGN KEY([RoomID])
		REFERENCES [Room]([RoomID]) ON UPDATE CASCADE,
	CONSTRAINT [fk_FireAlarm_FPanelID] FOREIGN KEY([FPanelID])
		REFERENCES [FireAlarm]([FPanelID]) ON UPDATE CASCADE
)
GO

/**********************************************************Stored procedures below **********************************************************/
print '' print '*** creating sp_authenticate_user'
GO
CREATE PROCEDURE [sp_authenticate_user](
	@Email	[nvarchar](250),
	@PasswordHash [nvarchar](100)
)
AS
BEGIN
	SELECT  COUNT([UserID])
	FROM	[dbo].[Users]
	WHERE	[Email] = LOWER(@Email)
	AND		[PasswordHash] = @PasswordHash
	AND		[Active] = 1
END
GO

print '' print '*** creating sp_select_users_by_active'
GO
CREATE PROCEDURE [sp_select_users_by_active](
	@Active		[bit]
)
AS
BEGIN
	SELECT  [UserID],[FirstName],[LastName],[Email],[Active]
	FROM	[dbo].[Users]
	WHERE	[Active] = @Active
END
GO

print '' print '*** creating sp_update_email'
GO

CREATE PROCEDURE [sp_update_email](
	@OldEmail			[nvarchar](250),
	@NewEmail			[nvarchar](250),
	@PasswordHash		[nvarchar](100)
)
AS
BEGIN
	UPDATE [dbo].[Users]
	SET [Email] = LOWER(@NewEmail)
	WHERE [Email] = LOWER(@OldEmail)
	AND [PasswordHash] = @PasswordHash
	AND [Active] = 1
END
GO



print '' print '*** creating sp_insert_user'
GO
CREATE PROCEDURE [sp_insert_user](
	@FirstName 	[nvarchar](50),
	@LastName	[nvarchar] (50),
	@Email		[nvarchar] (250)
)
AS
BEGIN	
	INSERT INTO [dbo].[Users]
		([FirstName],[LastName],[Email])
		VALUES
			(@FirstName, @LastName, LOWER(@Email))
			RETURN SCOPE_IDENTITY()
END
GO

print '' print '*** creating sp_select_user_by_email'
GO

CREATE PROCEDURE [sp_select_user_by_email](
	@Email			[nvarchar](250)
	
)
AS
BEGIN
	SELECT [UserID], [FirstName], [LastName]
	FROM  [Users]
	WHERE [Email] = @Email
END
GO



print '' print '*** creating sp_update_password'
GO
CREATE PROCEDURE [sp_update_password](
	@UserID               [int],
    @OldPasswordHash		[nvarchar](100),
    @NewPasswordHash        [nvarchar](100)
    
)
AS
BEGIN
	UPDATE [dbo].[Users]
	SET [PasswordHash] = @NewPasswordHash
	WHERE [UserID] = @UserID
	AND [PasswordHash] = @OldPasswordHash
	AND [Active] = 1
    RETURN @@ROWCOUNT
END
GO

print '' print '*** creating sp_update_user'
GO
CREATE PROCEDURE [sp_update_user](
	@UserID [int],	
	@NewFirstName  [nvarchar](50),
	@NewLastName	[nvarchar](50),
	@NewEmail		[nvarchar](250),	
	@OldFirstName  [nvarchar](50),
	@OldLastName	[nvarchar](50),
	@OldEmail		[nvarchar](250)
)
AS
BEGIN
	UPDATE [dbo].[Users]
		SET [FirstName] = @NewFirstName,
			[LastName] = @NewLastName,
			[Email] = @NewEmail
		WHERE [UserID] = @UserID
		AND	[FirstName] = @OldFirstName
		AND [LastName] = @OldLastName
		AND [Email] = @OldEmail
		
		RETURN @@ROWCOUNT
END
GO

print '' print '*** creating sp_deactivate_user'
GO
CREATE PROCEDURE [sp_deactivate_user](
		@UserID [int]
			
)
AS
BEGIN
	UPDATE [dbo].[Users]
		SET [Active]= 0
		WHERE [UserID] = @UserID
		
		RETURN @@ROWCOUNT
	
END
GO

print '' print '*** creating sp_reactivate_user'
GO
CREATE PROCEDURE [sp_reactivate_user](
		@UserID [int]
)
AS
BEGIN
UPDATE [dbo].[Users]
		SET [Active]= 1
		WHERE [UserID] = @UserID
		
		RETURN @@ROWCOUNT
	
END
GO

print '' print '*** creating sp_select_roles_by_userID'
GO

CREATE PROCEDURE [sp_select_roles_by_userID](
	@UserID			[int]
	
)
AS
BEGIN
	SELECT [RoleID]
	FROM  [UserRole]
	WHERE [UserID] = @UserID
END
GO

print '' print '*** creating sp_select_user_by_id'
GO
CREATE PROCEDURE [sp_select_user_by_id](
	@UserID [int]
)
AS
BEGIN
	SELECT [UserID],[FirstName],[LastName],[Email],[Active]
	FROM	[dbo].[Users]
	WHERE [UserID] = @UserID
	
END
GO

print '' print '*** creating sp_insert_user_role'
GO
CREATE PROCEDURE [sp_insert_user_role](
	@UserID [int],
	@RoleID[nvarChar](50)
)
AS
BEGIN
	INSERT INTO [dbo].[UserRole]
		([UserID],[RoleID])
		VALUES
		(@UserID,@RoleID)
	
END
GO

print '' print '*** creating sp_delete_user_role'
GO
CREATE PROCEDURE [sp_delete_user_role](
	@UserID [int],
	@RoleID[nvarchar](50)
)
AS
BEGIN
	DELETE From [dbo].[UserRole]
	WHERE [UserID]= @UserID
	AND [RoleID] = @RoleID
	
END
GO

print '' print '*** creating sp_select_all_roles'
GO
CREATE PROCEDURE [sp_select_all_roles]
AS
BEGIN
	SELECT [RoleID]
	FROM [dbo].[Role]
	ORDER BY [RoleID]
END
GO

print '' print '*** creating sp_select_users_by_role'
GO
CREATE PROCEDURE [sp_select_users_by_role](
	@RoleID		[nvarchar](50)
)
AS
BEGIN
	SELECT  [UserID]
	FROM	[dbo].[UserRole]
	WHERE	[RoleID] = @RoleID
	ORDER BY [UserID]
END
GO

print '' print '*** creating sp_select_all_rooms'
GO
CREATE PROCEDURE [sp_select_all_rooms]
AS
BEGIN
	SELECT  [RoomID],[RoomNotes]
	FROM	[dbo].[Room]
END
GO

print '' print '*** creating sp_select_room_by_id'
GO
CREATE PROCEDURE [sp_select_room_by_id](
	@RoomID [nvarchar] (4)
)
AS
BEGIN
	SELECT [RoomID],[RoomNotes]
	FROM	[dbo].[Room]
	WHERE [RoomID] = @RoomID
	
END
GO

print '' print '*** creating sp_insert_room'
GO
CREATE PROCEDURE [sp_insert_room](
	@RoomID 	[nvarchar](4)
)
AS
BEGIN	
	INSERT INTO [dbo].[Room]
		([RoomID])
		VALUES
			(@RoomID)
			RETURN SCOPE_IDENTITY()
END
GO

print '' print '*** creating sp_delete_room'
GO
CREATE PROCEDURE [sp_delete_room](
	@RoomID		 [nvarchar](4)
			
)
AS
BEGIN
DELETE From [dbo].[Room]
	WHERE [RoomID]= @RoomID
		
END
GO


/**********************************************************Insert statements below **********************************************************/

print '' print '*** creating sample Users records'
GO
INSERT INTO [dbo].[Users]
([FirstName],[LastName],[Email])
VALUES
('Admin','admin','admin@syncbak.com'),
('Tim','Kris','tim.kris@syncbak.com')
GO

print '' print '*** creating sample Roles records'
GO
INSERT INTO [dbo].[Role]
([RoleID],[Description])
VALUES
('Admin',"Company Admin")
GO

print '' print '*** creating sample UserRoles records'
GO
INSERT INTO [dbo].[UserRole]
([UserID],[RoleID])
VALUES
(100,"Admin")
GO

print '' print '*** creating sample Room records'
GO
INSERT INTO [dbo].[Room]
([RoomID], [RoomNotes])
VALUES
("A01", "This is a room with things in it."),
("A02", "This is a room with things in it."),
("A03", "This is a room with things in it."),
("A04", "This is a room with things in it."),
("A05", "This is a room with things in it."),
("A06", "This is a room with things in it."),
("A07", "This is a room with things in it."),
("A08", "This is a room with things in it."),
("A09", "This is a room with things in it."),
("A10" , "This is a room with things in it."),
("A11", "This is a room with things in it."),
("A12", "This is a room with things in it."),
("A13", "This is a room with things in it."),
("A14", "This is a room with things in it."),
("A15", "This is a room with things in it."),
("A16", "This is a room with things in it."),
("A17", "This is a room with things in it."),
("A18", "This is a room with things in it."),
("A19", "This is a room with things in it."),
("A20", "This is a room with things in it."),
("A21", "This is a room with things in it."),
("A22", "This is a room with things in it."),
("A23", "This is a room with things in it."),
("A24", "This is a room with things in it."),
("A25", "This is a room with things in it."),
("A26", "This is a room with things in it."),
("A27", "This is a room with things in it."),
("A28", "This is a room with things in it."),
("A29", "This is a room with things in it."),
("A30", "This is a room with things in it."),
("A31", "This is a room with things in it."),
("A32", "This is a room with things in it."),
("A33", "This is a room with things in it."),
("A34", "This is a room with things in it."),
("A35", "This is a room with things in it."),
("A36", "This is a room with things in it."),
("A37", "This is a room with things in it."),
("A38", "This is a room with things in it."),
("A39", "This is a room with things in it."),
("A40", "This is a room with things in it."),
("A41", "This is a room with things in it."),
("A42", "This is a room with things in it."),
("A43", "This is a room with things in it."),
("A44", "This is a room with things in it."),
("A45", "This is a room with things in it."),
("A46", "This is a room with things in it."),
("A47", "This is a room with things in it."),
("A48", "This is a room with things in it."),
("A49", "This is a room with things in it."),
("A50", "This is a room with things in it."),
("A51", "This is a room with things in it."),
("A52", "This is a room with things in it."),
("A53", "This is a room with things in it."),
("B01", "This is a room with things in it."),
("B02", "This is a room with things in it."),
("B03", "This is a room with things in it."),
("B04", "This is a room with things in it."),
("B05", "This is a room with things in it."),
("B06", "This is a room with things in it."),
("B07", "This is a room with things in it."),
("B08", "This is a room with things in it."),
("B09", "This is a room with things in it."),
("B10", "This is a room with things in it."),
("B11", "This is a room with things in it."),
("B12", "This is a room with things in it."),
("B13", "This is a room with things in it."),
("B14", "This is a room with things in it."),
("B15", "This is a room with things in it."),
("B16", "This is a room with things in it."),
("B17", "This is a room with things in it."),
("B18", "This is a room with things in it."),
("B19", "This is a room with things in it."),
("B20", "This is a room with things in it."),
("LL01", "This is a room with things in it."),
("LL02", "This is a room with things in it."),
("LL03", "This is a room with things in it."),
("LL04", "This is a room with things in it."),
("LL05", "This is a room with things in it."),
("LL06", "This is a room with things in it."),
("LL07", "This is a room with things in it."),
("LL08", "This is a room with things in it."),
("LL09", "This is a room with things in it."),
("LL10", "This is a room with things in it."),
("LL11", "This is a room with things in it."),
("LL12", "This is a room with things in it."),
("LL13", "This is a room with things in it."),
("LL14", "This is a room with things in it."),
("LL15", "This is a room with things in it."),
("LL16", "This is a room with things in it."),
("LL17", "This is a room with things in it."),
("LL18", "This is a room with things in it."),
("LL19", "This is a room with things in it."),
("LL20", "This is a room with things in it.")
GO

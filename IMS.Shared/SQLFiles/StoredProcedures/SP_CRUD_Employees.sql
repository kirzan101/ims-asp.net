/****** Object:  StoredProcedure [dbo].[SP_CRUD_Employees]    Script Date: 12/07/2021 11:32:46 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--===================================================================================
--INSERT					-> @init_flag = 0
--UPDATE					-> @init_flag = 1
--===================================================================================

-- =================================================================================--
-- Author:		Christian Kim Escamilla												--
-- Create date: June 06, 2023 05:24 PM												--
-- Description:	SP CRUD Employees													--
-- =================================================================================--
CREATE PROCEDURE [dbo].[SP_CRUD_Employees]
(
	@ret_errmsg		NVARCHAR(80)	output,
	@init_flag		INT				output,	-- Can use any purpose
	
	@Id				INT				= NULL,
	@Salutation		NVARCHAR(35)	= NULL,
	@NickName		NVARCHAR(35)	= NULL,
	@FirstName		NVARCHAR(35)	= NULL,
	@MiddleName		NVARCHAR(35)	= NULL,		
	@LastName		NVARCHAR(35)	= NULL,
	@Suffix			NVARCHAR(35)	= NULL,
	@MobileNumber	NVARCHAR(35)	= NULL,
	@Designation	NVARCHAR(200)	= NULL,
	@Branch			NVARCHAR(100)	= NULL,
	@ImagePath		NVARCHAR(MAX)	= NULL,
	@UserId			INT				= NULL,
	@Active			bit				= 1
	
)
AS

DECLARE @ERR		INT
DECLARE @TABLE_NAME NVARCHAR(20)

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SET @ret_errmsg = 'Start'
	SET @TABLE_NAME = '[dbo].[Employees]'
	-- =================================================================================
	--		INSERT	
	--		[Employees]
	-- =================================================================================	
	IF @init_flag = 0
	BEGIN
		INSERT INTO [dbo].[Employees]
		(	
			 [Salutation]
			,[NickName]
			,[FirstName]
			,[MiddleName]
			,[LastName]
			,[Suffix]
			,[MobileNumber] 
			,[Designation]
			,[Branch]
			,[ImagePath]
			,[CreatedBy] 
			,[CreatedAt]
			,[Active]
		) 
		VALUES 
		(	
			 @Salutation
			,@NickName
			,@FirstName
			,@MiddleName
			,@LastName
			,@Suffix
			,@MobileNumber
			,@Designation
			,@Branch
			,@ImagePath
			,@UserId			
			,GETDATE()
			,@Active
		)
	
		SELECT @init_flag = @@IDENTITY			-- For output param; gets inserted id
		
		-- Check error
		SELECT @ERR = @@ERROR
		IF @ERR <> 0
		BEGIN
			SET @ret_errmsg = @TABLE_NAME + ' INSERT Error'
			RETURN -3
		END
	END
		
	
	-- =================================================================================
	-- 		UPDATE
	--		[Employees]
	-- =================================================================================
	ELSE IF @init_flag = 1
	BEGIN
		IF @Id IS NULL OR @Id <= 0
		BEGIN
			SET @ret_errmsg = 'Parameter error for UPDATE'
			RETURN -990
		END
		
		UPDATE [dbo].[Employees] SET
			 [Salutation]	= @Salutation    
			,[NickName]		= @NickName    
			,[FirstName]	= @FirstName    
			,[MiddleName]	= @MiddleName
			,[LastName]		= @LastName
			,[Suffix]		= @Suffix
			,[MobileNumber]	= @MobileNumber 
			,[Designation]	= @Designation 
			,[Branch]		= @Branch 
			,[ImagePath]	= @ImagePath 
			,[ModifiedBy]	= @UserId
			,[ModifiedAt]	= GETDATE()
		WHERE 
			[Id] = @Id
		-- Check error
		SELECT @ERR = @@ERROR
		IF @ERR <> 0
		BEGIN
			SET @ret_errmsg = @TABLE_NAME + ' UPDATE Error'
			RETURN @ERR
		END			
	END

	SET @ret_errmsg = 'Finished Successfully'
	RETURN 0
		
END
GO



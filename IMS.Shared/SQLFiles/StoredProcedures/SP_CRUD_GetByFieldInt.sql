SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =================================================================================--
-- Author:		Christian Escamilla													--
-- Create date: June 05, 2023 10:14 AM												--
-- Description:	Generic SP to get all records filtered by integer field value		--
-- =================================================================================--
CREATE PROCEDURE  [dbo].[SP_CRUD_GetByFieldInt]
(
	 @tableName		varchar(50)
	,@field			varchar(50)
	,@value			int
	,@order			varchar(5)  = 'ASC'
	,@limit			int			= NULL
	,@checkStatus	BIT = 1
	,@statusFlag	int = 1 -- 0 = ALL; 1 = Active; 2 = Inactive; 3 = Non-deleted
)
AS
DECLARE @sql			NVARCHAR(MAX)
DECLARE @whereClause	NVARCHAR(MAX)
DECLARE @limitScript	NVARCHAR(100) = ''
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @whereClause = ' WHERE ' + @field + ' = ' + CONVERT(NVARCHAR(MAX), @value) 

	IF @statusFlag = 0
	BEGIN
		SET @whereClause = @whereClause;
	END
	ELSE IF @statusFlag = 1
	BEGIN
		--SET ACTIVE ONLY RECORD filter
		IF COL_LENGTH(@tableName, 'Active') IS NOT NULL AND @checkStatus = 1
			SET @whereClause = @whereClause + ' AND [ACTIVE] = 1 AND [DELETED] = 0 '
	END
	ELSE IF @statusFlag = 2
	BEGIN
		SET @whereClause = @whereClause + ' AND [ACTIVE] = 0 AND [DELETED] = 0 '
	END
	ELSE IF @statusFlag = 3
	BEGIN
		SET @whereClause = @whereClause + ' AND [DELETED] != 1 '
	END

	

	--Set LIMIT
	IF @limit IS NOT NULL
		SET @limitScript = ' TOP( ' + CONVERT(NVARCHAR(12), @limit) + ') '

	SET @sql = ' SELECT ' + @limitScript + ' * FROM ' 
	+ @tableName 
	+ @whereClause
	+ ' ORDER BY  [Id] '+ @order

	PRINT @sql
	EXECUTE (@sql)
	
	RETURN 0		
END
GO



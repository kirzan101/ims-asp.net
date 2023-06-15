SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================--
-- Author:		Christian Escamilla													--
-- Create date: June 05, 2023 02:13 PM												--
-- Description:	Generic SP to get all records filtered by string field value		--
-- =================================================================================--
CREATE PROCEDURE  [dbo].[SP_CRUD_GetByFieldString]
(
	 @tableName varchar(50)
	,@field		varchar(50)
	,@value		varchar(150)
	,@order		varchar(5)  = 'ASC'
	,@limit		int			= NULL
	,@checkStatus	BIT = 0	
)
AS
DECLARE @sql			NVARCHAR(MAX)
DECLARE @whereClause	NVARCHAR(MAX)
DECLARE @limitScript	NVARCHAR(100) = ''
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Trim passed data
	SET @tableName = LTRIM(RTRIM(@tableName))
	SET @value = LTRIM(RTRIM(@value))
	SET @field = LTRIM(RTRIM(@field))
	SET @whereClause = ' WHERE ' + @field + ' = ''' + @value + '''' 

	--SET ACTIVE ONLY RECORD filter
	IF @checkStatus = 1
	BEGIN
		SET @whereClause = @whereClause + ' AND [DELETED] != 1 '
	END
	ELSE
	BEGIN
		IF COL_LENGTH(@tableName, 'Active') IS NOT NULL
			SET @whereClause = @whereClause + ' AND [ACTIVE] = 1 AND [DELETED] = 0 '
	END
	--Set LIMIT
	IF @limit IS NOT NULL
		SET @limitScript = ' TOP( ' + CONVERT(NVARCHAR(12), @limit) + ') '

	SET @sql = ' SELECT ' + @limitScript + ' * FROM ' 
	+ @tableName 
	+ @whereClause
	+ 'ORDER BY  [Id] '+ @order

	PRINT @sql
	EXECUTE (@sql)
	RETURN 0		
END
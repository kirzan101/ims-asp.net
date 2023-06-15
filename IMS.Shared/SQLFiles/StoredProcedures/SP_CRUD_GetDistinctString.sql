
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =====================================================================================--
-- Author:		RICK																	--
-- Create date: March 11, 2021 02:52 PM													--
-- Description:	Generic SP to get all distinct string field records from provided info	--
-- =====================================================================================--
CREATE PROCEDURE  [dbo].[SP_CRUD_GetDistinctString]
(
	 @tableName				varchar(150)
	,@distinctStrField		varchar(150)
	,@whereField			varchar(150) = NULL
	,@whereValue			varchar(150) = NULL
)
AS
DECLARE @sql			NVARCHAR(MAX)
DECLARE @whereClause	NVARCHAR(MAX) = ' WHERE 0 = 0 '
DECLARE @ERR			int
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--SET ACTIVE ONLY RECORD filter
	IF COL_LENGTH(@tableName, 'Active') IS NOT NULL
		SET @whereClause = @whereClause + ' AND ([ACTIVE] = 1 AND [DELETED] = 0) '

	IF (@whereField != NULL OR @whereField != '') OR
	(@whereValue != NULL OR @whereValue > 0)
	BEGIN
		SET @whereClause = @whereClause + ' AND ' + @whereField + ' = ' + CONVERT(VARCHAR(12), @whereValue)
	END

	SET @sql = ' SELECT DISTINCT([' + @distinctStrField + ']) FROM [dbo].' 
	+ '[' + @tableName  + ']'
	+ @whereClause
	+ ' ORDER BY  ['+ @distinctStrField + ']'

	PRINT @sql
	EXECUTE (@sql)

	SELECT @ERR = @@ERROR
	IF @ERR <> 0
	BEGIN
		RETURN -3
	END

	RETURN 0		
END


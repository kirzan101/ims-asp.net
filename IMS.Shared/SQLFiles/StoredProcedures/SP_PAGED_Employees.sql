SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



--===================================================================================
--Paged List				-> @init_flag = 0
--Filtered Row Count		-> @init_flag = 1
--total row count			-> @init_flag = 2
--===================================================================================
-- =================================================================================--
-- Author:		Christian Escamilla													--
-- Create date: June 06, 2023 09:58 AM												--
-- Description:	SP to get Paginated list of Employees								--
-- =================================================================================--
CREATE PROCEDURE  [dbo].[SP_PAGED_Employees]
(
	@init_flag			int = 0 -- NOTE: value = 0 : Paged List; 1: Filtered Row Count; 2: total row count
	,@Index				int	= 1
	,@PageSize			int	= 25
	,@Filter			nvarchar(20) = ''
	,@OrderField		nvarchar(20) = 'LastName'
	,@OrderDirection	nvarchar(4) = 'ASC'
	,@Status			int = 0 --ALL
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @Searchfilter	NVARCHAR(MAX) = ''
	DECLARE @sql			NVARCHAR(MAX)
	DECLARE @statusType		varchar(20)
	DECLARE @customFilter	varchar(MAX)

	SET @statusType =
	CASE 
		WHEN @status = 1 THEN 'ACTIVE'
		WHEN @status = 2 THEN 'INACTIVE'
		WHEN @status = 2 THEN 'DELETED'
		ELSE 'ALL'
	END

	--FOR FILTERING -- WILL BE USED FOR	@init_flag = 0 AND 1
	IF (@init_flag = 0 OR @init_flag = 1)
	BEGIN

		SET @Searchfilter = 'WHERE AA.[Id] IS NOT NULL '

		--SET THE SEARCH FILTER
		IF (@Filter != NULL OR @Filter != '')
		BEGIN
		SET @customFilter = ' AND (
		AA.[FirstName] LIKE ''' + @Filter + '%'' OR 
		AA.[MiddleName] LIKE ''' + @Filter + '%'' OR 
		AA.[LastName] LIKE ''' + @Filter + '%'' OR 
		AA.[MobileNumber] LIKE ''' + @Filter + '%'' OR 
		AA.[Designation] LIKE ''' + @Filter + '%'' OR 
		AA.[Branch] LIKE ''' + @Filter + '%''
		)' 

		SET @Searchfilter = @Searchfilter + @customFilter
		END

		-- Add temporary query
		IF (@Status = 1)
		BEGIN
			SET @customFilter = ' AND (AA.[ACTIVE] = 1 AND AA.[DELETED] = 0)'
		END

		IF (@Status = 2)
		BEGIN
			SET @customFilter = ' AND (AA.[ACTIVE] = 0 AND AA.[DELETED] = 0) '
		END

		IF @Status = 3
		BEGIN
			SET @customFilter = ' AND (AA.[ACTIVE] = 0 AND AA.[DELETED] = 1) '
		END

		SET @Searchfilter = @Searchfilter + @customFilter
	END

	IF @Status > 2 OR @Status < 0
	BEGIN
		SET @Status = 0 -- Force other values entered to default to 0
	END

	IF @init_flag = 0
	BEGIN
		DECLARE @newsize		INT
		SET @newsize = @Index - 1 + @PageSize

		SET @sql = ' WITH filteredList AS (
	SELECT ROW_NUMBER() OVER (ORDER BY ' + @OrderField + ' ' + @OrderDirection + ') AS [index], *
	FROM 
( SELECT
	AA.[Id]
   ,AA.[Salutation]
   ,AA.[NickName]
   , AA.[LastName] + '', '' 
	+ AA.[FirstName] + '' '' AS [Name]
   ,AA.[FirstName]
   ,AA.[MiddleName]
   ,AA.[LastName]
   ,AA.[Suffix]
   ,AA.[MobileNumber]
   ,AA.[Designation]
   ,AA.[Branch]
   ,AA.[Active]
   ,AA.[Deleted]
   ,CASE 
	WHEN AA.[Active] = 0 AND AA.[Deleted] = 0  THEN ''INACTIVE''
	WHEN AA.[Active] = 1 AND AA.[Deleted] = 0  THEN ''ACTIVE''
	WHEN AA.[Active] = 0 AND AA.[Deleted] = 1  THEN ''DELETED''
	ELSE '''' END AS [STATUS]
   ,AA.[CreatedAt]
   ,AA.[ModifiedAt]
	FROM [dbo].[Employees] AS  AA
		' + @Searchfilter +'
	) AS JoinedList )
SELECT * FROM filteredList 
WHERE [index] BETWEEN ' + CONVERT(NVARCHAR(12), @index) + ' AND ' + CONVERT(NVARCHAR(12), @newsize)
 + ' AND ([STATUS] = ''' + @statusType + ''' OR ' + CONVERT(NVARCHAR(12), @status) + ' = 0)';

		PRINT @sql
		EXECUTE (@sql)
	END
	ELSE IF @init_flag = 1
	BEGIN
		--FILTERED ROW COUNT
		SET @sql = ' SELECT COUNT(*) AS FILTERED_ROW_COUNT FROM 
	[dbo].[Employees] AS  AA
	' + @Searchfilter
		PRINT @sql
		EXECUTE (@sql)
	END
	ELSE IF @init_flag = 2
	BEGIN
		SELECT COUNT(*) AS filteredCount
		FROM [dbo].[Employees]
	END
END 


GO



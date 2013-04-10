if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Events_SetRssEvent]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Events_SetRssEvent]
GO

CREATE PROCEDURE dbo.Events_SetRssEvent
	@EventCategoryId Int,
	@Name nvarchar(50),
	@Description nvarchar(max),
	@StartTime DateTime,
	@UrlId Int,
	@EventLink nvarchar(max),
	@Hash varchar(32)
AS

/*

Summary: 

*/

If Not Exists(Select * From rt_Events Where Hash = @Hash And CategoryId = @EventCategoryId)
	Insert Into rt_Events (
		Name, Description, StartTime, EndTime, CategoryId, UrlId, Color, EventLink, Hash )
	Values (
		@Name, @Description, @StartTime, Null, @EventCategoryId, @UrlId, Null, @EventLink, @Hash )

Go

GRANT EXECUTE ON dbo.Events_SetRssEvent TO [Public]
Go

/*

sp_columns legalnoticeversions

*/
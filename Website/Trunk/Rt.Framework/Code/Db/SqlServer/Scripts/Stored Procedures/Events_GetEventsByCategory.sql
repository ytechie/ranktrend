if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Events_GetEventsByCategory]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Events_GetEventsByCategory]
Go

CREATE PROCEDURE dbo.[Events_GetEventsByCategory]
	@Start DateTime,
	@End DateTime,
	@EventCategoryId Int
AS

--Look up the default color for events without a color
Declare @DefaultColor Int

Select @DefaultColor = IsNull(gs.IntValue, -10185235)
From GlobalSettings gs
Where gs.Id = 5

Select e.[Name], e.StartTime, e.EndTime, ec.[Name] CategoryName,
	Urls.Url, IsNull(e.Color, @DefaultColor) Color, e.EventLink
From rt_Events e
Left Outer Join rt_EventCategories ec On e.CategoryId = ec.Id
Join Urls On e.UrlId = Urls.Id
Where CategoryId = @EventCategoryId
And (StartTime Between @Start And @End
			Or EndTime Between @Start And @End
			Or (StartTime < @Start And EndTime > @End))

Go

GRANT EXECUTE ON dbo.[Events_GetEventsByCategory] TO [Public]
Go

/*
select * from rt_events
select * from rt_eventcategories
select * from globalsettings

[Events_GetEventsByCategory] '1-1-07', '1-24-07', 1
[Events_GetEventsByCategory] '1-1-07', '1-24-07', 8

*/

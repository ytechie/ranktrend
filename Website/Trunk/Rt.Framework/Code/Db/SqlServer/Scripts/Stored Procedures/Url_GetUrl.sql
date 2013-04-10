if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Url_GetUrl]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Url_GetUrl]
Go

CREATE PROCEDURE dbo.[Url_GetUrl]
	@UrlId Int
AS

Select *
From Urls
Where Id = @UrlId

Go

GRANT EXECUTE ON dbo.[Url_GetUrl] TO [Public]
Go

/*

[Url_GetUrl] 1


*/


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Users_GetUserStatusReport]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Users_GetUserStatusReport]
GO

CREATE PROCEDURE dbo.Users_GetUserStatusReport
	@UserId UniqueIdentifier
AS

Select
	m.UserId,
	u.Username,
	m.Email,
	ui.FirstName,
	ui.LastName,
	m.IsApproved [AccountActive],
	m.CreateDate,
	u.LastActivityDate,
	GetUtcDate() ServerTime
From aspnet_Membership m
Join aspnet_Users u On u.UserId = m.UserId
Join UserInformation ui On ui.Userid = m.UserId
Where m.UserId = @UserId

Select Url
From Urls
Where UserId = @UserId

Select dt.Description, d.Name, u.Url, d.LastCheckAttempt
From ConfiguredDatasources d
Join Urls u On d.UrlId = u.Id
Join DataSourceTypes dt On dt.Id = d.DatasourceTypeId
Where u.UserId = @UserId

GO

GRANT EXECUTE ON dbo.Users_GetUserStatusReport TO [Public]
Go

/*
select * from aspnet_users
select * from aspnet_membership
select * from userinformation

Users_GetUserStatusReport 'D463118B-5A61-49BC-BA97-91B06FAE2774'
*/


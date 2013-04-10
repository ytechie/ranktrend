if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Email_SendMassEmail]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Email_SendMassEmail]
GO

CREATE PROCEDURE dbo.Email_SendMassEmail
	@EmailTemplateId Int,
	@FromAddress Varchar(50),
	@RoleName nVarChar(256),
	@ApplicationName nVarChar(256)
AS

/*
Declare @EmailTemplateId Int,
	@RoleName nVarChar(256),
	@ApplicationId UniqueIdentifier,
	@FromAddress Varchar(50)
Select @EmailTemplateId = 5, @RoleName = 'Administrators', @FromAddress = 'SystemMailer@RankTrend.com',
	@ApplicationId = '1E14E9F4-97FD-44FA-B447-F6425D8F8A5A'
*/

If @RoleName Like 'EmailFilter[_]%'
Begin
	Print 'Email filter used'

	Declare @FilteredEmailUsers table(
		[UserId] UniqueIdentifier )

	If @RoleName Is Not Null And @RoleName != ''
		Insert Into @FilteredEmailUsers (UserId)
			Exec @RoleName

	Insert Into EMailQueue
		Select @FromAddress, ui.FirstName + ' ' + ui.LastName, m.Email, t.Subject, t.Message, t.Html, Null, GetUtcDate(), Null, 0, m.UserId, 0
		From EmailTemplates t
		Join @FilteredEmailUsers f on f.UserId = f.UserId
		Join aspnet_Membership m On m.UserId = f.UserId
		Left Outer Join UserInformation ui On ui.UserId = m.UserId
		Where t.Id = @EmailTemplateId
			And m.UserId Not In ( Select UserId From rt_UsersBouncedEmails Where BounceCount > 1 )

End
Else
Begin
	Print 'Role used'

	Declare @RoleId UniqueIdentifier,
		@Applicationid UniqueIdentifier
	Select @ApplicationId = ApplicationId From aspnet_Applications Where ApplicationName = @ApplicationName
	Select @RoleId = RoleId From aspnet_Roles Where RoleName = @RoleName And ApplicationId = @Applicationid
	Select @ApplicationId ApplicationId, @RoleId RoleId

	Insert Into EmailQueue
		Select @FromAddress, ui.FirstName + ' ' + ui.LastName, m.Email, t.Subject, t.Message, t.Html, Null, GetUtcDate(), Null, 0, uir.UserId, 0
		From EmailTemplates t
		Join aspnet_Roles r On r.RoleId = @RoleId
		Join aspnet_UsersInRoles uir On r.RoleId = uir.RoleId
		Join aspnet_Membership m On m.UserId = uir.UserId
		Left Outer Join UserInformation ui On ui.UserId = uir.UserId
		Where t.Id = @EmailTemplateId
			And m.UserId Not In ( Select UserId From rt_UsersBouncedEmails Where BounceCount > 1 )
			And m.IsApproved = 1
End
GO

GRANT EXECUTE ON dbo.Email_SendMassEmail TO [Public]
Go

/*
Email_SendMassEmail 5, 'SystemMailer@RankTrend.com', 'Administrators', 'RT'

Email_SendMassEmail 5, 'SystemMailer@RankTrend.com', 'EmailFilter_UnverifiedUsers', 'RT'



select * from emailqueue

*/


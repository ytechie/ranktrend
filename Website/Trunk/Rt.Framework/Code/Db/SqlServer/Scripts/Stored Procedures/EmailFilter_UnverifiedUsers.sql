if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[EmailFilter_UnverifiedUsers]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[EmailFilter_UnverifiedUsers]
GO

CREATE PROCEDURE dbo.EmailFilter_UnverifiedUsers
AS

Select UserId
From aspnet_membership
Where IsApproved = 0
	And CreateDate < DateAdd(m, -1, GetUtcDate())

GO

--Add the Description to the stored procedure's extended properties.
Exec sp_addExtendedProperty
	@name = 'Description',
	@value = 'Users whose accounts are not yet validated and were created over a month ago.',
	@level0type = 'schema', @level0name = dbo,
	@level1type = 'Procedure', @level1name = [EmailFilter_UnverifiedUsers]

GO

GRANT EXECUTE ON dbo.EmailFilter_UnverifiedUsers TO [Public]
Go

/*

EmailFilter_UnverifiedUsers

*/
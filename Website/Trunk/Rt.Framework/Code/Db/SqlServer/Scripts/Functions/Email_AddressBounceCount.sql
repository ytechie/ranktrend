If exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Email_AddressBounceCount]') and xtype in (N'FN', N'IF', N'TF'))
drop function [dbo].[Email_AddressBounceCount]
GO

create function dbo.[Email_AddressBounceCount] (@EmailAddress Varchar(255))
  Returns Bit
As

/*

Summary: Gets the number of times that an email address has bounced.

*/

Begin

Declare @BounceCount Int

Select @BounceCount = Count(*)
From EmailQueue eq
Where Bounced = 1
And eq.ToAddress = @EmailAddress

Return @BounceCount

End

Go

/*

Select dbo.[Email_AddressBounceCount]('info@spectrom.com.au')
Select dbo.[Email_AddressBounceCount]('superjason@new.rr.com')

select * from emailqueue where bounced = 1

*/
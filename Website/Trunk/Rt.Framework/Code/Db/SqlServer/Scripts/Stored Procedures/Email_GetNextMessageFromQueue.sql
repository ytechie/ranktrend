if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Email_GetNextMessageFromQueue]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Email_GetNextMessageFromQueue]
GO

CREATE PROCEDURE dbo.Email_GetNextMessageFromQueue
AS

/*

Summary: Gets the next email from the emailqueue for the email engine.

Todo: remove the global settings that used to provide the numbers for how this worked.
*/

Select Top 1 *
From EmailQueue eq
Where SentOn Is Null --Get emails that were not sent yet
--If it's at 10 tries, it will have been trying for at least a week
And NumberOfTries <= 10
--Send each one exponentially farther from the last one
--For example, attempt at 10, 20, 40, 80, 160, 320, etc
And (LastTry Is Null Or DateAdd(mi, 10 * Power(2, NumberOfTries), LastTry) < GetUtcDate())
And dbo.[Email_AddressBounceCount](eq.[ToAddress]) <= 2
Order By QueuedOn Asc, --Get oldest emails first
NumberOfTries Asc --Get the emails we haven't tried yet first

GO

GRANT EXECUTE ON dbo.Email_GetNextMessageFromQueue TO [Public]
Go

/*
select (10 * Power(2, 10)) / 60.0

select * from emailqueue

Email_GetNextMessageFromQueue

sp_columns emailqueue



*/
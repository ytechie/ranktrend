if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Plans_GetPlans]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[Plans_GetPlans]
GO

CREATE PROCEDURE dbo.Plans_GetPlans
AS

Select *
From rt_Plans

GO

GRANT EXECUTE ON dbo.Plans_GetPlans TO [Public]
Go

/*

*/


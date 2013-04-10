if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Leads_IncrementLeadHit]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Leads_IncrementLeadHit]
Go

CREATE PROCEDURE [dbo].[Leads_IncrementLeadHit]
  @LeadKey Varchar(Max)
AS

/*

Summary:
Increments the lead hit counter for a particular lead.

*/

Update rt_LeadSources
Set HitCount = HitCount + 1
Where RedirectPageName = @LeadKey

Go

GRANT EXECUTE ON dbo.[Leads_IncrementLeadHit] TO [Public]
Go

/*
select * from rawdata
sp_columns rawdata
select * from rt_leadsources

Leads_IncrementLeadHit 1

*/


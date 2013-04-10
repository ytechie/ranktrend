if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Leads_LeadEffectiveness]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Leads_LeadEffectiveness]
Go

CREATE PROCEDURE [dbo].[Leads_LeadEffectiveness]
AS

Declare @SignupCounts Table
(
	LeadId Int,
	[Count] Int
)

--Insert Into @SignupCounts
--Select Distinct(u.LeadId), Count(u.LeadId) 'Lead Count'
--From Users u
--Where LeadId Is Not Null
--Group By u.LeadId

Select [RedirectPageName] 'Lead Code',
		Description 'Lead Description',
		CONVERT(money, Cost) 'Cost',
		EstimatedDistribution 'Estimated Distribution',
--		'Sign-up Rate' =
--		convert(varchar(100), 
--			(convert(numeric(10,7),
--				Case HitCount 
--					when 0 then 0 
--					else Coalesce(Cast(sc.[Count] As Float) / Cast(HitCount As Float), 0) 
--				end
--			)*100)) 
--		+ '%',
--		HitCount 'Clicks',
--		Coalesce(sc.[Count], 0) 'Sign Ups',
--		'Cost/Click' =
--		CONVERT(money, 
--			Case HitCount 
--				when 0 then 0 
--				else Coalesce(Cost / Cast(HitCount As Float), 0) 
--			end
--		),
--		'Cost/Sign-up'=
--		CONVERT(money, 
--			Coalesce(Cost / Cast(sc.[Count] As Float), 0) 
--		),
--		'Overall Sign-up Rate' = 
--		convert(varchar(100), 
--			(convert(numeric(10,7),
--				Coalesce(Cast(sc.[Count] As Float) / Cast(EstimatedDistribution As Float), 0)
--			)*100)) 
--		+ '%',
		'Cost/1000 (CPM)' =  
		CONVERT(money, 
			Case  
				when HitCount = 0 then 0 
				when EstimatedDistribution = 0 then 0 
				else Coalesce(Coalesce(Cost , 0)/ (Cast(EstimatedDistribution As float)/1000), 0)
			end 
		),
		'Clicks/1000' = 
		CONVERT(money, 
			Case HitCount 
				when 0 then 0 
				else Coalesce(Cast(HitCount As float) / (Cast(EstimatedDistribution As float)/1000), 0)
			end
		)
From rt_LeadSources ls
--Left Outer Join @SignupCounts sc On sc.LeadId = ls.[Id]

Go

GRANT EXECUTE ON dbo.[Leads_LeadEffectiveness] TO [Public]
Go

/*
select * from rt_leadsources

[Leads_LeadEffectiveness]

*/


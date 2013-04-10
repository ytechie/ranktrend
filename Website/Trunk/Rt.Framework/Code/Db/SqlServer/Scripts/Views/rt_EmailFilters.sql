IF object_id(N'dbo.rt_EmailFilters', 'V') IS NOT NULL
	DROP VIEW dbo.rt_EmailFilters
GO

CREATE VIEW dbo.rt_EmailFilters AS

Select o.name, Replace(o.name, 'EmailFilter_', '') DisplayName, ex.value Description
From sysobjects o
Left Outer Join sys.extended_properties ex On o.Id = ex.major_id And class = 1 And ex.name = 'Description'
Where o.xtype = 'P' And o.name Like 'EmailFilter[_]%'

/*
select * from rt_EmailFilters
select * from sysobjects

select *
from sys.extended_properties
*/
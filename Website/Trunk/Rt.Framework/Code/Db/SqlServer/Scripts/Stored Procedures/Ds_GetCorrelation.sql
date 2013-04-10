if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Ds_GetCorrelation]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Ds_GetCorrelation]
Go

CREATE PROCEDURE dbo.[Ds_GetCorrelation]
	@Datasource1 Int,
	@Datasource2 Int
AS

Declare @Data Table
(
	Date DateTime,
	Val1 Float,
	Val2 Float
)

Insert Into @Data (Date)
Select Distinct(Cast(floor(cast(rd1.[Timestamp] as float)) as datetime))
From RawData rd1
Where rd1.ConfiguredDatasourceId = @Datasource1
And Success = 1
And [Timestamp] Is Not Null
--Get data for the last 6 months
And [Timestamp] > DateAdd(mm, -6, GetUtcDate())

--These updates are the bottleneck!!!!
Update @Data
Set Val1 = (Select Avg(rd.FloatValue)
			From RawData rd
			Where rd.ConfiguredDatasourceId = @Datasource1
			And Success = 1
			And FloatValue Is Not Null
			And Cast(floor(cast(rd.[Timestamp] as float)) as datetime) = Date)

Update @Data
Set Val2 = (Select Avg(rd.FloatValue)
			From RawData rd
			Where rd.ConfiguredDatasourceId = @Datasource2
			And Success = 1
			And FloatValue Is Not Null
			And Cast(floor(cast(rd.[Timestamp] as float)) as datetime) = Date)

--Remove nulls
Delete From @Data
Where Val1 Is Null Or Val2 Is Null

Declare @Avg1 Float, @Avg2 Float
Declare @Correlation Float

-- calculate average values
SELECT @Avg1 = Avg(Val1), @Avg2 = Avg(Val2)
From @Data
  
-- perform actual calculation
SELECT @Correlation =
   CASE
      WHEN SQRT(SUM(POWER((Val1 - @Avg1),2)) * SUM(POWER((Val2 - @Avg2),2))) = 0 THEN 
         Null  -- return error code for divide-by-zero
      ELSE
         POWER(
            SUM((Val1 - @Avg1) * (Val2 - @Avg2)) /
            SQRT(SUM(POWER((Val1 - @Avg1),2)) * SUM(POWER((Val2 - @Avg2),2)))
         ,2)
   END
FROM @Data

Select @Correlation

Go

GRANT EXECUTE ON dbo.[Ds_GetCorrelation] TO [Public]
Go

/*

Exec [Ds_GetCorrelation] 29, 30
Exec [Ds_GetCorrelation] 29, 3

*/


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Keywords_BulkImport]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
  drop procedure [dbo].[Keywords_BulkImport]
Go

CREATE PROCEDURE dbo.[Keywords_BulkImport]
	@UrlId Int,
	@Keywords Xml,
	@DatasourceTypeId Int
AS

Declare @Keyword Varchar(Max)

Declare keyword_cursor Cursor Fast_Forward Read_Only For
Select T.Item.value('@phrase', 'varchar(max)')
From @Keywords.nodes('keywords/keyword') As T(Item)

Open keyword_cursor

Fetch Next From keyword_cursor
Into @Keyword

While @@Fetch_Status = 0
Begin
	Exec Keywords_AddKeyword @UrlId, @Keyword, @DatasourceTypeId

	Fetch Next From keyword_cursor
	Into @Keyword
End

Close keyword_cursor
Deallocate keyword_cursor

Go

GRANT EXECUTE ON dbo.[Keywords_BulkImport] TO [Public]
Go

/*

select top 100 * from configureddatasources
select * from datasourceparameters
select * from datasourcetypes
select * from urls

[Keywords_BulkImport] 1, '<keywords><keyword phrase="superjason"></keyword><keyword phrase="jason young"></keyword></keywords>', 1

*/
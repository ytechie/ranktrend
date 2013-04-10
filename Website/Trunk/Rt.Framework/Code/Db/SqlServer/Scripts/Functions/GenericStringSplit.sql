CREATE FUNCTION [dbo].[GenericStringSplit]

(

      @List VARCHAR(8000),

      @Separator VARCHAR(10)

)

RETURNS

@ItemsList TABLE

(

      Item SQL_VARIANT

)

AS

BEGIN

      DECLARE @CurrItem SQL_VARIANT, @Pos INT

      SET @List = LTRIM(RTRIM(@List))+ @Separator

      SET @Pos = CHARINDEX(@Separator, @List, 1)

      IF REPLACE(@List, @Separator, '') <> ''

      BEGIN

            WHILE @Pos > 0

            BEGIN

                  SET @CurrItem = LTRIM(RTRIM(LEFT(@List, (@Pos  -1 ))))

                  IF @CurrItem <> ''

                  BEGIN

                        INSERT INTO @ItemsList (Item)

                        VALUES (@CurrItem)

                  END

                  SET @List = RIGHT(@List,(LEN(@List) - (@Pos + LEN(@Separator)-1)))

                  SET @Pos = CHARINDEX(@Separator, @List, 1)

            END

      END  

      RETURN

END
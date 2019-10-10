CREATE FUNCTION dbo.STRING_SPLIT
    (
       @List       NVARCHAR(50),
       @Delimiter  NVARCHAR(3)
    )
    RETURNS TABLE
    WITH SCHEMABINDING
    AS
       RETURN 
       (  
          SELECT [value] = y.i.value('(./text())[1]', 'nvarchar(5)')
          FROM 
          ( 
            SELECT x = CONVERT(XML, '<i>' 
              + REPLACE(@List, @Delimiter, '</i><i>') 
              + '</i>').query('.')
          ) AS a CROSS APPLY x.nodes('i') AS y(i)
       );
    GO
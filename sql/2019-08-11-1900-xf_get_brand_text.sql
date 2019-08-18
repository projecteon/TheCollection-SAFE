CREATE FUNCTION [dbo].[xf_get_brand_text]
(
	@brand_id int
)
RETURNS nvarchar(255)
AS
BEGIN
	DECLARE @sRet nvarchar(255)

    SELECT TOP 1 @sRet = a.s_name
      FROM tco_brand a WITH (NOLOCK)
	 WHERE a.id      = @brand_id

	RETURN ISNULL(@sRet, '')
END
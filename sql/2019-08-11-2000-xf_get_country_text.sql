CREATE FUNCTION [dbo].[xf_get_country_text]
(
	@country_id int
)
RETURNS nvarchar(255)
AS
BEGIN
	DECLARE @sRet nvarchar(255)

    SELECT TOP 1 @sRet = a.s_name
      FROM tcs_country a WITH (NOLOCK)
	 WHERE a.id      = @country_id

	RETURN ISNULL(@sRet, '')
END
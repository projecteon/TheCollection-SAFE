CREATE FUNCTION [dbo].[xf_get_bagtype_text]
(
	@bagtype_id int
)
RETURNS nvarchar(255)
AS
BEGIN
	DECLARE @sRet nvarchar(255)

    SELECT TOP 1 @sRet = a.s_name
      FROM tco_bagtype a WITH (NOLOCK)
	 WHERE a.id      = @bagtype_id

	RETURN ISNULL(@sRet, '')
END
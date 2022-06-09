namespace Migrations

open SimpleMigrations

[<Migration(202205120001L, "Create Function <xf_get_bagtype_text>")>]
type Create_xf_get_bagtype_text() =
  inherit Migration()
  override u.Up() =
    base.Execute(@"
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
    ");
  override u.Down() =
    base.Execute("DROP FUNCTION [dbo].[xf_get_bagtype_text]");

[<Migration(202205120002L, "Create Function <xf_get_brand_text>")>]
type Create_xf_get_brand_text() =
  inherit Migration()
  override u.Up() =
    base.Execute(@"
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
    ");
  override u.Down() =
    base.Execute("DROP FUNCTION [dbo].[xf_get_brand_text]");

[<Migration(202205120003L, "Create Function <xf_get_country_text>")>]
type Create_xf_get_country_text() =
  inherit Migration()
  override u.Up() =
    base.Execute(@"
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
    ");
  override u.Down() =
    base.Execute("DROP FUNCTION [dbo].[xf_get_country_text]");


[<Migration(202205120004L, "Create Function <xf_get_country_tlp_code>")>]
type Create_xf_get_country_code() =
  inherit Migration()
  override u.Up() =
    base.Execute(@"
CREATE FUNCTION [dbo].[xf_get_country_tlp_code]
(
	@country_id int
)
RETURNS nvarchar(2)
AS
BEGIN
	DECLARE @sRet nvarchar(2)

    SELECT TOP 1 @sRet = a.s_tld_code
      FROM tcs_country a WITH (NOLOCK)
	 WHERE a.id      = @country_id

	RETURN ISNULL(@sRet, '')
END
    ");
  override u.Down() =
    base.Execute("DROP FUNCTION [dbo].[xf_get_country_tlp_code]");
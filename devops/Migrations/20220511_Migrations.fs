namespace Migrations

open SimpleMigrations

[<Migration(202205112051L, "Create Table <tcuac_users>")>]
type Create_tcuac_users() =
  inherit Migration()
  override u.Up() =
    base.Execute(@"
CREATE TABLE [dbo].[tcuac_users](
  [id] [int] IDENTITY(1,1) NOT NULL,
  [s_email] [nvarchar](255) NOT NULL,
  [s_password] [varchar](255) NOT NULL,
  [s_refreshtoken] [varchar](255) NULL,
  [dt_refreshtoken_expire] [datetime] NULL,
  [dt_created] [datetime] NOT NULL,
  [dt_modified] [datetime] NOT NULL,
CONSTRAINT [PK__tcuac_us__3213E83F1086DD39] PRIMARY KEY CLUSTERED
(
  [id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
    ");
  override u.Down() =
    base.Execute("DROP TABLE [dbo].[tcuac_users]");

[<Migration(2022051122241L, "Create Table <tcs_country>")>]
type Create_tcs_country() =
  inherit Migration()
  override u.Up() =
    base.Execute(@"
CREATE TABLE [dbo].[tcs_country](
  [id] [int] IDENTITY(1,1) NOT NULL,
  [s_name] [nvarchar](255) NOT NULL,
  [s_tld_code] [nvarchar](2) NULL,
  [s_name_nl] [nvarchar](255) NULL,
  [dt_created] [datetime2](7) NOT NULL,
  [dt_modified] [datetime2](7) NOT NULL,
 CONSTRAINT [PK__tcs_coun__3213E83FD6254ED2] PRIMARY KEY CLUSTERED
(
  [id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
    ");
  override u.Down() =
    base.Execute("DROP TABLE [dbo].[tcs_country]");

[<Migration(2022051122243L, "Create Table <tco_brand>")>]
type Create_tco_brand() =
  inherit Migration()
  override u.Up() =
    base.Execute(@"
CREATE TABLE [dbo].[tco_brand](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[s_name] [nvarchar](255) NOT NULL,
	[dt_created] [datetime2](7) NOT NULL,
	[dt_modified] [datetime2](7) NOT NULL,
 CONSTRAINT [PK__tco_bran__3213E83F34E28DED] PRIMARY KEY CLUSTERED
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
    ");
  override u.Down() =
    base.Execute("DROP TABLE [dbo].[tco_brand]");

[<Migration(2022051122244L, "Create Table <tco_bagtype>")>]
type Create_tco_bagtype() =
  inherit Migration()
  override u.Up() =
    base.Execute(@"
CREATE TABLE [dbo].[tco_bagtype](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[s_name] [nvarchar](255) NOT NULL,
	[dt_created] [datetime2](7) NOT NULL,
	[dt_modified] [datetime2](7) NOT NULL,
 CONSTRAINT [PK__tco_bagt__3213E83F3FA4998E] PRIMARY KEY CLUSTERED
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
    ");
  override u.Down() =
    base.Execute("DROP TABLE [dbo].[tco_bagtype]");

[<Migration(2022051122245L, "Create Table <tcd_files>")>]
type Create_tcd_files() =
  inherit Migration()
  override u.Up() =
    base.Execute(@"
CREATE TABLE [dbo].[tcd_files](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[i_archive] [int] NOT NULL,
	[s_uri] [nvarchar](255) NOT NULL,
	[s_filename] [nvarchar](255) NOT NULL,
	[dt_created] [datetime2](7) NOT NULL,
	[dt_modified] [datetime2](7) NOT NULL,
 CONSTRAINT [PK__tcd_file__3213E83F7A19C272] PRIMARY KEY CLUSTERED
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[tcd_files] ADD  CONSTRAINT [DF__tcd_files__d_cre__17036CC0]  DEFAULT (getdate()) FOR [dt_created]

ALTER TABLE [dbo].[tcd_files] ADD  CONSTRAINT [DF__tcd_files__d_mod__17F790F9]  DEFAULT (getdate()) FOR [dt_modified]
    ");
  override u.Down() =
    base.Execute(@"
ALTER TABLE [dbo].[tcd_files] DROP  CONSTRAINT [DF__tcd_files__d_cre__17036CC0];
ALTER TABLE [dbo].[tcd_files] DROP  CONSTRAINT [DF__tcd_files__d_mod__17F790F9],
DROP TABLE [dbo].[tcd_files]
    ");

[<Migration(2022051122246L, "Create Table <tcd_teabag>")>]
type Create_tcd_teabag() =
  inherit Migration()
  override u.Up() =
    base.Execute(@"
CREATE TABLE [dbo].[tcd_teabag](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ro_brand] [int] NOT NULL,
	[ro_bagtype] [int] NOT NULL,
	[rs_country] [int] NULL,
	[rf_image] [int] NULL,
	[i_archive] [int] NULL,
	[s_flavour] [nvarchar](255) NOT NULL,
	[s_hallmark] [nvarchar](255) NULL,
	[s_serie] [nvarchar](255) NULL,
	[s_serialnumber] [nvarchar](255) NULL,
	[s_search_terms] [nvarchar](500) NULL,
	[d_created] [date] NOT NULL,
	[dt_created] [datetime2](7) NOT NULL,
	[dt_modified] [datetime2](7) NOT NULL,
 CONSTRAINT [PK__tcd_teab__3213E83F8B218774] PRIMARY KEY CLUSTERED
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
    ");
  override u.Down() =
    base.Execute("DROP TABLE [dbo].[tcd_teabag]");
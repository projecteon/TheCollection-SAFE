CREATE TABLE [dbo].[tcd_teabag](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ro_brand] [int] NOT NULL,
	[ro_bagtype] [int] NOT NULL,
	[rs_country] [int] NULL,
	[rf_image] [int] NULL,
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

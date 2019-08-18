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

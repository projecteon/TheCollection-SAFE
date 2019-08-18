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

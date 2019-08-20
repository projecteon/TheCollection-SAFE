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
GO

ALTER TABLE [dbo].[tcd_files] ADD  CONSTRAINT [DF__tcd_files__i_imp__160F4887]  DEFAULT ((0)) FOR [i_import_id]
GO

ALTER TABLE [dbo].[tcd_files] ADD  CONSTRAINT [DF__tcd_files__d_cre__17036CC0]  DEFAULT (getdate()) FOR [dt_created]
GO

ALTER TABLE [dbo].[tcd_files] ADD  CONSTRAINT [DF__tcd_files__d_mod__17F790F9]  DEFAULT (getdate()) FOR [dt_modified]
GO

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[AppSettingByCafs]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].[AppSettingByCafs]

CREATE TABLE [dbo].[AppSettingByCafs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CreateTime] [datetime2](3) NOT NULL,
	[CreateUserId] [nvarchar](100) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Key] [nvarchar](100) NULL,
	[Value] [nvarchar](max) NULL,
	[Description] [nvarchar](1000) NULL,
	[LatestModifiedTime] [datetime2](3) NULL,
 CONSTRAINT [PK_AppSettingByCafs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


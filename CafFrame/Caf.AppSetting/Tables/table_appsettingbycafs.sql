
DROP TABLE IF EXISTS AppSettingByCafs
CREATE TABLE AppSettingByCafs (
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CreateTime] [datetime2](3) NOT NULL,
	[CreateUserId] [nvarchar](100) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Key] [nvarchar](100) NULL,
	[Value] [nvarchar](max) NULL,
	[Description] [nvarchar](1000) NULL,
	[LatestModifiedTime] [datetime2](3) NULL,
    CONSTRAINT [PK_AppSettingByCafs] PRIMARY KEY CLUSTERED ( [Id] ASC )
);

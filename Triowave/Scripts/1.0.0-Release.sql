USE [GeneralDW]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[StockPrice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Symbol] [nvarchar](50) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Open] [decimal](18, 6) NULL,
	[High] [decimal](18, 6) NULL,
	[Low] [decimal](18, 6) NULL,
	[Close] [decimal](18, 6) NULL,
	[Volume] [int] NULL,
 CONSTRAINT [PK_StockPrice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Symbols](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Symbol] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Exchange] [nvarchar](50) NULL,
	[AssetType] [nvarchar](50) NULL,
	[IpoDate] [date] NULL,
	[Status] [bit] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_Symbols] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

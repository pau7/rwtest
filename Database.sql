USE [RWTest]
GO

/****** Object:  Table [dbo].[Product]    Script Date: 09/04/2017 17:54:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Category](
       [Id] [bigint] IDENTITY(1,1) NOT NULL,
       [Name] [varchar](max) NULL,
       [Parent_id] [bigint] NULL,
CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
       [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[Category]  WITH CHECK ADD  CONSTRAINT [Parent_id] FOREIGN KEY([Parent_id])
REFERENCES [dbo].[Category] ([Id])
GO

ALTER TABLE [dbo].[Category] CHECK CONSTRAINT [Parent_id]
GO

CREATE TABLE [dbo].[Product](
       [Id] [bigint] IDENTITY(1,1) NOT NULL,
       [Code] [varchar](max) NULL,
       [Name] [varchar](max) NULL,
       [Category_id] [bigint] NULL,
       [Model] [varchar](max) NULL,
       [Price] [decimal](19, 5) NULL,
       [Colour] [varchar](max) NULL,
       [Size] [varchar](max) NULL,
       [Description] [varchar](max) NULL,
CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
       [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Category] FOREIGN KEY([Category_Id])
REFERENCES [dbo].[Category] ([Id])

ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Category]

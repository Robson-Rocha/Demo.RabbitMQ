use [master];
go

/****** Object:  Database [DemoRabbit]    Script Date: 5/11/2019 6:35:27 AM ******/
CREATE DATABASE [DemoRabbit]
GO

USE [DemoRabbit]
GO

/****** Object:  Table [Lojas]    Script Date: 5/11/2019 6:35:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Lojas](
	[LojaId] [int] NOT NULL,
	[NomeLoja] [nvarchar](50) NULL,
 CONSTRAINT [PK_Lojas] PRIMARY KEY CLUSTERED 
(
	[LojaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [Produtos]    Script Date: 5/11/2019 6:35:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Produtos](
	[ProdutoId] [int] NOT NULL,
	[NomeProduto] [nvarchar](50) NOT NULL,
	[ValorProduto] [nvarchar](10) NOT NULL,
	[DataUltimaAlteracao] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Produtos] PRIMARY KEY CLUSTERED 
(
	[ProdutoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT [Lojas] ([LojaId], [NomeLoja]) VALUES (1, N'Loja 1'),(2, N'Loja 2'),(3, N'Loja 3'),(4, N'Loja 4');
GO
INSERT [Produtos] ([ProdutoId], [NomeProduto], [ValorProduto], [DataUltimaAlteracao]) VALUES 
(1, N'Teclado', N'50', CAST(N'2019-05-11T09:08:53.8733333' AS DateTime2)),
(2, N'Mouse', N'35', CAST(N'2019-05-11T09:09:04.8600000' AS DateTime2)),
(3, N'Monitor', N'500', CAST(N'2019-05-11T09:09:30.3033333' AS DateTime2)),
(4, N'Impressora', N'250', CAST(N'2019-05-11T09:27:00.0000000' AS DateTime2))
GO

ALTER TABLE [Produtos] ADD  CONSTRAINT [DF_Produtos_DataUltimaAlteracao]  DEFAULT (getdate()) FOR [DataUltimaAlteracao]
GO

USE [master]
GO
ALTER DATABASE [DemoRabbit] SET  READ_WRITE 
GO

USE [SkillShareFinal]
GO
/****** Object:  Table [dbo].[CPAccesses]    Script Date: 3/17/2019 9:54:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CPAccesses](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NULL,
	[CModule] [varchar](50) NULL,
	[CMethod] [varchar](50) NULL,
	[Activity] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CPLangs]    Script Date: 3/17/2019 9:54:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CPLangs](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Code] [varchar](50) NULL,
	[Activity] [bit] NULL,
	[Created] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CPLoginLogs]    Script Date: 3/17/2019 9:54:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CPLoginLogs](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](50) NULL,
	[Token] [varchar](500) NULL,
	[Activity] [bit] NULL,
	[Created] [datetime2](7) NULL,
	[IP] [varchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CPMenus]    Script Date: 3/17/2019 9:54:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CPMenus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ParentID] [int] NULL,
	[LangID] [int] NULL,
	[Type] [varchar](50) NULL,
	[Summary] [nvarchar](500) NULL,
	[Files] [ntext] NULL,
	[Content] [ntext] NULL,
	[Activity] [bit] NULL,
	[Created] [datetime2](7) NULL,
	[Name] [nvarchar](50) NULL,
	[Code] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CPResources]    Script Date: 3/17/2019 9:54:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CPResources](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LangID] [int] NULL,
	[Value] [nvarchar](50) NULL,
	[Code] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CPRoles]    Script Date: 3/17/2019 9:54:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CPRoles](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Code] [varchar](50) NULL,
	[Lock] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CPUsers]    Script Date: 3/17/2019 9:54:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CPUsers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NULL,
	[Name] [nvarchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Pass] [varchar](50) NULL,
	[BirthDay] [datetime2](7) NULL,
	[Skype] [varchar](100) NULL,
	[Phone] [varchar](20) NULL,
	[Activity] [bit] NULL,
	[Created] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebPage]    Script Date: 3/17/2019 9:54:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebPage](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CModule] [varchar](50) NULL,
	[CMethod] [varchar](50) NULL,
	[LangID] [int] NULL,
	[MenuID] [int] NULL,
	[Icon] [varchar](255) NULL,
	[Name] [nvarchar](250) NULL,
	[Code] [varchar](250) NULL,
	[Customs] [ntext] NULL,
	[Summary] [nvarchar](255) NULL,
	[Title] [nvarchar](255) NULL,
	[PageTitle] [nvarchar](255) NULL,
	[Content] [ntext] NULL,
	[Order] [int] NULL,
	[Activity] [bit] NULL,
	[Created] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebPageDetails]    Script Date: 3/17/2019 9:54:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebPageDetails](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CMethod] [varchar](50) NULL,
	[WebPageID] [int] NULL,
	[Icon] [varchar](255) NULL,
	[Name] [nvarchar](250) NULL,
	[Code] [varchar](250) NULL,
	[Customs] [ntext] NULL,
	[Summary] [nvarchar](255) NULL,
	[Title] [nvarchar](255) NULL,
	[PageTitle] [nvarchar](255) NULL,
	[Content] [ntext] NULL,
	[Order] [int] NULL,
	[Activity] [bit] NULL,
	[Created] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[CPLangs] ON 

INSERT [dbo].[CPLangs] ([ID], [Name], [Code], [Activity], [Created]) VALUES (1, N'Việt Nam', N'vn', 1, CAST(N'2019-03-16T11:36:35.1700000' AS DateTime2))
INSERT [dbo].[CPLangs] ([ID], [Name], [Code], [Activity], [Created]) VALUES (2, N'Tiếng Anh', N'en', 1, CAST(N'2019-03-16T11:36:35.2066667' AS DateTime2))
INSERT [dbo].[CPLangs] ([ID], [Name], [Code], [Activity], [Created]) VALUES (3, N'Tiếng Nhật', N'jn', 0, CAST(N'2019-03-16T21:52:00.8957744' AS DateTime2))
SET IDENTITY_INSERT [dbo].[CPLangs] OFF
SET IDENTITY_INSERT [dbo].[CPLoginLogs] ON 

INSERT [dbo].[CPLoginLogs] ([ID], [Email], [Token], [Activity], [Created], [IP]) VALUES (4, N'longthaihoang94@gmail.com', N'fb601d8d-0829-4535-97b2-c8d942a199b1', 1, CAST(N'2019-03-17T15:30:24.5208271' AS DateTime2), N'::1')
SET IDENTITY_INSERT [dbo].[CPLoginLogs] OFF
SET IDENTITY_INSERT [dbo].[CPMenus] ON 

INSERT [dbo].[CPMenus] ([ID], [ParentID], [LangID], [Type], [Summary], [Files], [Content], [Activity], [Created], [Name], [Code]) VALUES (1, 0, 1, N'test', N'ahihi', NULL, NULL, 1, CAST(N'2019-03-17T17:51:57.8608233' AS DateTime2), N'test', NULL)
INSERT [dbo].[CPMenus] ([ID], [ParentID], [LangID], [Type], [Summary], [Files], [Content], [Activity], [Created], [Name], [Code]) VALUES (4, 0, 1, N'new', N'danh mục chưa toàn bộ thông tin của trang tin tức
đây là danh mục gốc', NULL, NULL, 1, CAST(N'2019-03-17T19:46:14.5433993' AS DateTime2), N'Danh mục tin tức', NULL)
INSERT [dbo].[CPMenus] ([ID], [ParentID], [LangID], [Type], [Summary], [Files], [Content], [Activity], [Created], [Name], [Code]) VALUES (5, 4, 1, N'new', N'Tin tức mới cập nhật trong ngày ', NULL, NULL, 1, CAST(N'2019-03-17T18:23:56.6520607' AS DateTime2), N'Tin mới nhất', N'tin-moi-nhat')
INSERT [dbo].[CPMenus] ([ID], [ParentID], [LangID], [Type], [Summary], [Files], [Content], [Activity], [Created], [Name], [Code]) VALUES (6, 0, 1, N'CAdv', N'Danh mục lưu trữ hình ảnh ,, slider ...', NULL, NULL, 1, CAST(N'2019-03-17T19:56:36.4253938' AS DateTime2), N'Danh mục quảng cáo', N'danh-muc-quang-cao')
SET IDENTITY_INSERT [dbo].[CPMenus] OFF
SET IDENTITY_INSERT [dbo].[CPRoles] ON 

INSERT [dbo].[CPRoles] ([ID], [Name], [Code], [Lock]) VALUES (1, N'Administrator', N'administrator', 1)
INSERT [dbo].[CPRoles] ([ID], [Name], [Code], [Lock]) VALUES (2, N'Memember', N'memember', 1)
INSERT [dbo].[CPRoles] ([ID], [Name], [Code], [Lock]) VALUES (4, N'News', N'news', 0)
SET IDENTITY_INSERT [dbo].[CPRoles] OFF
SET IDENTITY_INSERT [dbo].[CPUsers] ON 

INSERT [dbo].[CPUsers] ([ID], [RoleID], [Name], [Email], [Pass], [BirthDay], [Skype], [Phone], [Activity], [Created]) VALUES (1, 1, N'Hoàng Thái Long', N'longthaihoang94@gmail.com', N'DOQOz63WtGs=', CAST(N'1994-04-09T00:00:00.0000000' AS DateTime2), N'live:breakingdawn1235', N'0976497928', 1, CAST(N'2019-03-16T11:35:16.5933333' AS DateTime2))
SET IDENTITY_INSERT [dbo].[CPUsers] OFF
ALTER TABLE [dbo].[CPLoginLogs] ADD  CONSTRAINT [DF_CPLoginLogs_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[CPMenus] ADD  CONSTRAINT [DF_CPMenus_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[CPRoles] ADD  CONSTRAINT [DF_CPRoles_Lock]  DEFAULT ((0)) FOR [Lock]
GO

USE [SkillShareFinal]
GO
/****** Object:  Table [dbo].[CPAccesses]    Script Date: 10/04/2019 6:51:20 PM ******/
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
/****** Object:  Table [dbo].[CPLangs]    Script Date: 10/04/2019 6:51:20 PM ******/
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
/****** Object:  Table [dbo].[CPLoginLogs]    Script Date: 10/04/2019 6:51:20 PM ******/
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
/****** Object:  Table [dbo].[CPMenus]    Script Date: 10/04/2019 6:51:20 PM ******/
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
/****** Object:  Table [dbo].[CPResources]    Script Date: 10/04/2019 6:51:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CPResources](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LangID] [int] NULL,
	[Value] [nvarchar](50) NULL,
	[Code] [varchar](50) NULL,
	[Name] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CPRoles]    Script Date: 10/04/2019 6:51:20 PM ******/
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
/****** Object:  Table [dbo].[CPUsers]    Script Date: 10/04/2019 6:51:20 PM ******/
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
/****** Object:  Table [dbo].[ModCAdvs]    Script Date: 10/04/2019 6:51:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ModCAdvs](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MenuID] [int] NULL,
	[Name] [nvarchar](100) NULL,
	[Code] [varchar](max) NULL,
	[File] [varchar](255) NULL,
	[Summary] [nvarchar](1000) NULL,
	[URL] [nvarchar](511) NULL,
	[Order] [int] NULL,
	[Activity] [bit] NULL,
	[Created] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ModNews]    Script Date: 10/04/2019 6:51:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ModNews](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LangID] [int] NULL,
	[MenuID] [int] NULL,
	[State] [int] NULL,
	[Name] [nvarchar](3000) NULL,
	[Code] [varchar](3000) NULL,
	[File] [nvarchar](3000) NULL,
	[View] [int] NULL,
	[Summary] [nvarchar](3000) NULL,
	[Content] [ntext] NULL,
	[Custom] [nvarchar](3000) NULL,
	[PageTitle] [nvarchar](3000) NULL,
	[PageDescription] [nvarchar](3000) NULL,
	[PageKeywords] [nvarchar](3000) NULL,
	[Created] [datetime2](7) NULL,
	[Updated] [datetime2](7) NULL,
	[Order] [int] NULL,
	[Activity] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ModTags]    Script Date: 10/04/2019 6:51:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ModTags](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RootID] [int] NULL,
	[Name] [nvarchar](3000) NULL,
	[Code] [varchar](3000) NULL,
	[Title] [nvarchar](3000) NULL,
	[Keywords] [nvarchar](3000) NULL,
	[Description] [nvarchar](3000) NULL,
	[Activity] [bit] NULL,
	[Created] [datetime2](7) NULL,
	[MenuID] [int] NULL,
	[Order] [int] NULL,
	[NewID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SysPages]    Script Date: 10/04/2019 6:51:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SysPages](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ParentID] [int] NULL,
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
	[TemplateID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SysProperties]    Script Date: 10/04/2019 6:51:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SysProperties](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TemplateDetailID] [int] NULL,
	[PartialID] [varchar](50) NULL,
	[Name] [varchar](50) NULL,
	[Value] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SysTemplateDetails]    Script Date: 10/04/2019 6:51:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SysTemplateDetails](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ParrentID] [varchar](50) NULL,
	[LayoutName] [nvarchar](50) NULL,
	[TemplateID] [int] NULL,
	[PartialID] [varchar](50) NULL,
	[PartialView] [varchar](50) NULL,
	[CModule] [varchar](50) NULL,
	[TypeView] [varchar](50) NULL,
	[IsBody] [bit] NULL,
	[IsDynamic] [bit] NULL,
	[Order] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SysTemplates]    Script Date: 10/04/2019 6:51:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SysTemplates](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LangID] [int] NULL,
	[Name] [nvarchar](50) NULL,
	[File] [varchar](50) NULL,
	[Html] [ntext] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebPage]    Script Date: 10/04/2019 6:51:20 PM ******/
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
	[TemplateID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebPageDetails]    Script Date: 10/04/2019 6:51:20 PM ******/
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
	[TemplateID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[CPAccesses] ON 

INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (1, 2, N'CPAccess', N'create', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (2, 2, N'CPLang', N'index', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (3, 2, N'CPLang', N'create', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (4, 2, N'CPLang', N'edit', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (5, 2, N'CPLang', N'delete', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (6, 2, N'CPLang', N'export', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (7, 2, N'CPLang', N'active', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (8, 2, N'CPLang', N'nonactive', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (9, 2, N'CPMenu', N'index', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (10, 2, N'CPMenu', N'create', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (11, 2, N'CPMenu', N'edit', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (12, 2, N'CPMenu', N'delete', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (13, 2, N'CPMenu', N'export', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (14, 2, N'CPMenu', N'active', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (15, 2, N'CPMenu', N'nonactive', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (16, 2, N'CPRole', N'index', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (17, 2, N'CPRole', N'create', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (18, 2, N'CPRole', N'edit', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (19, 2, N'CPRole', N'delete', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (20, 2, N'CPRole', N'export', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (21, 2, N'CPUser', N'index', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (22, 2, N'CPUser', N'create', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (23, 2, N'CPUser', N'edit', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (24, 2, N'CPUser', N'delete', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (25, 2, N'CPUser', N'export', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (26, 2, N'CPUser', N'active', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (27, 2, N'CPUser', N'nonactive', 0)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (28, 2, N'FileManager', N'index', 1)
INSERT [dbo].[CPAccesses] ([ID], [RoleID], [CModule], [CMethod], [Activity]) VALUES (29, 2, N'FileManager', N'create', 0)
SET IDENTITY_INSERT [dbo].[CPAccesses] OFF
SET IDENTITY_INSERT [dbo].[CPLangs] ON 

INSERT [dbo].[CPLangs] ([ID], [Name], [Code], [Activity], [Created]) VALUES (1, N'Việt Nam', N'vn', 1, CAST(N'2019-03-16T11:36:35.1700000' AS DateTime2))
INSERT [dbo].[CPLangs] ([ID], [Name], [Code], [Activity], [Created]) VALUES (2, N'Tiếng Anh', N'en', 1, CAST(N'2019-03-16T11:36:35.2066667' AS DateTime2))
INSERT [dbo].[CPLangs] ([ID], [Name], [Code], [Activity], [Created]) VALUES (3, N'Tiếng Nhật', N'jn', 0, CAST(N'2019-03-16T21:52:00.8957744' AS DateTime2))
SET IDENTITY_INSERT [dbo].[CPLangs] OFF
SET IDENTITY_INSERT [dbo].[CPLoginLogs] ON 

INSERT [dbo].[CPLoginLogs] ([ID], [Email], [Token], [Activity], [Created], [IP]) VALUES (4, N'longthaihoang94@gmail.com', N'73200aad-7b35-415f-81b3-631ed999f54f', 1, CAST(N'2019-03-25T15:47:08.5469586' AS DateTime2), N'::1')
SET IDENTITY_INSERT [dbo].[CPLoginLogs] OFF
SET IDENTITY_INSERT [dbo].[CPMenus] ON 

INSERT [dbo].[CPMenus] ([ID], [ParentID], [LangID], [Type], [Summary], [Files], [Content], [Activity], [Created], [Name], [Code]) VALUES (1, 0, 1, N'test', N'ahihi', NULL, NULL, 1, CAST(N'2019-03-17T17:51:57.8608233' AS DateTime2), N'test', NULL)
INSERT [dbo].[CPMenus] ([ID], [ParentID], [LangID], [Type], [Summary], [Files], [Content], [Activity], [Created], [Name], [Code]) VALUES (4, 0, 1, N'new', N'danh mục chưa toàn bộ thông tin của trang tin tức
đây là danh mục gốc', NULL, NULL, 1, CAST(N'2019-03-17T19:46:14.5433993' AS DateTime2), N'Danh mục tin tức', NULL)
INSERT [dbo].[CPMenus] ([ID], [ParentID], [LangID], [Type], [Summary], [Files], [Content], [Activity], [Created], [Name], [Code]) VALUES (5, 4, 1, N'new', N'Tin tức mới cập nhật trong ngày ', NULL, NULL, 1, CAST(N'2019-03-17T18:23:56.6520607' AS DateTime2), N'Tin mới nhất', N'tin-moi-nhat')
INSERT [dbo].[CPMenus] ([ID], [ParentID], [LangID], [Type], [Summary], [Files], [Content], [Activity], [Created], [Name], [Code]) VALUES (6, 0, 1, N'CAdv', N'Danh mục lưu trữ hình ảnh ,, slider ...', NULL, NULL, 1, CAST(N'2019-03-17T19:56:36.4253938' AS DateTime2), N'Danh mục quảng cáo', N'danh-muc-quang-cao')
SET IDENTITY_INSERT [dbo].[CPMenus] OFF
SET IDENTITY_INSERT [dbo].[CPResources] ON 

INSERT [dbo].[CPResources] ([ID], [LangID], [Value], [Code], [Name]) VALUES (1, 1, N'đây là test', N'test', N'test')
INSERT [dbo].[CPResources] ([ID], [LangID], [Value], [Code], [Name]) VALUES (2, 1, N'done', N'xong ', N'ok rồi ')
INSERT [dbo].[CPResources] ([ID], [LangID], [Value], [Code], [Name]) VALUES (3, 2, N'Tiếng anh', N'english', N'tiếng anh ')
SET IDENTITY_INSERT [dbo].[CPResources] OFF
SET IDENTITY_INSERT [dbo].[CPRoles] ON 

INSERT [dbo].[CPRoles] ([ID], [Name], [Code], [Lock]) VALUES (1, N'Administrator', N'administrator', 1)
INSERT [dbo].[CPRoles] ([ID], [Name], [Code], [Lock]) VALUES (2, N'Memember', N'memember', 1)
INSERT [dbo].[CPRoles] ([ID], [Name], [Code], [Lock]) VALUES (4, N'News', N'news', 0)
SET IDENTITY_INSERT [dbo].[CPRoles] OFF
SET IDENTITY_INSERT [dbo].[CPUsers] ON 

INSERT [dbo].[CPUsers] ([ID], [RoleID], [Name], [Email], [Pass], [BirthDay], [Skype], [Phone], [Activity], [Created]) VALUES (1, 1, N'Hoàng Thái Long', N'longthaihoang94@gmail.com', N'DOQOz63WtGs=', CAST(N'1994-04-09T00:00:00.0000000' AS DateTime2), N'live:breakingdawn1235', N'0976497928', 1, CAST(N'2019-03-16T11:35:16.5933333' AS DateTime2))
SET IDENTITY_INSERT [dbo].[CPUsers] OFF
SET IDENTITY_INSERT [dbo].[SysProperties] ON 

INSERT [dbo].[SysProperties] ([ID], [TemplateDetailID], [PartialID], [Name], [Value]) VALUES (1, 5, N'vswLogo', N'Title', N'Hoàng Thái Long')
INSERT [dbo].[SysProperties] ([ID], [TemplateDetailID], [PartialID], [Name], [Value]) VALUES (2, 5, N'vswLogo', N'MenuID', N'6')
INSERT [dbo].[SysProperties] ([ID], [TemplateDetailID], [PartialID], [Name], [Value]) VALUES (3, 2, N'd7a67caf-fb26-4ee8-b7b7-c5c8d5a26dd3', N'Title', N'Slider')
INSERT [dbo].[SysProperties] ([ID], [TemplateDetailID], [PartialID], [Name], [Value]) VALUES (4, 2, N'd7a67caf-fb26-4ee8-b7b7-c5c8d5a26dd3', N'MenuID', N'6')
INSERT [dbo].[SysProperties] ([ID], [TemplateDetailID], [PartialID], [Name], [Value]) VALUES (5, 4, N'09fc7eaa-1f6f-4f05-adff-f8c6bc086de5', N'Title', N'Silder23')
INSERT [dbo].[SysProperties] ([ID], [TemplateDetailID], [PartialID], [Name], [Value]) VALUES (6, 4, N'09fc7eaa-1f6f-4f05-adff-f8c6bc086de5', N'MenuID', N'6')
SET IDENTITY_INSERT [dbo].[SysProperties] OFF
SET IDENTITY_INSERT [dbo].[SysTemplateDetails] ON 

INSERT [dbo].[SysTemplateDetails] ([ID], [ParrentID], [LayoutName], [TemplateID], [PartialID], [PartialView], [CModule], [TypeView], [IsBody], [IsDynamic], [Order]) VALUES (1, N'vswMain', N'Body()', 1, N'31315365-565e-4832-9361-f726f587e442', NULL, N'RenderBody', NULL, 1, 1, 6)
INSERT [dbo].[SysTemplateDetails] ([ID], [ParrentID], [LayoutName], [TemplateID], [PartialID], [PartialView], [CModule], [TypeView], [IsBody], [IsDynamic], [Order]) VALUES (2, N'vswMain', N'Slider(ĐK : Quảng cáo/liên kết)', 1, N'd7a67caf-fb26-4ee8-b7b7-c5c8d5a26dd3', N'_Slider', N'MVCBase.ClientControllers.CAdvController', N'CAdv', 0, 1, 4)
INSERT [dbo].[SysTemplateDetails] ([ID], [ParrentID], [LayoutName], [TemplateID], [PartialID], [PartialView], [CModule], [TypeView], [IsBody], [IsDynamic], [Order]) VALUES (4, N'vswMain', N'Info image(ĐK : Quảng cáo/liên kết)', 1, N'09fc7eaa-1f6f-4f05-adff-f8c6bc086de5', N'_Slider', N'MVCBase.ClientControllers.CAdvController', N'CAdv', 0, 1, 0)
INSERT [dbo].[SysTemplateDetails] ([ID], [ParrentID], [LayoutName], [TemplateID], [PartialID], [PartialView], [CModule], [TypeView], [IsBody], [IsDynamic], [Order]) VALUES (5, N'', N'Logo(ĐK : Quảng cáo/liên kết)', 1, N'vswLogo', N'_Logo', N'MVCBase.ClientControllers.CAdvController', N'CAdv', 0, 0, 0)
INSERT [dbo].[SysTemplateDetails] ([ID], [ParrentID], [LayoutName], [TemplateID], [PartialID], [PartialView], [CModule], [TypeView], [IsBody], [IsDynamic], [Order]) VALUES (6, N'', N'Menu Top(No Exist)', 1, N'vswNav', N'_MenuTop', NULL, N'CMenu', 0, 0, 0)
INSERT [dbo].[SysTemplateDetails] ([ID], [ParrentID], [LayoutName], [TemplateID], [PartialID], [PartialView], [CModule], [TypeView], [IsBody], [IsDynamic], [Order]) VALUES (9, N'vswMain', N'new2(ĐK : Quảng cáo/liên kết)', 1, N'ffcc8933-cdd6-4f60-8e6a-6f87d7da3a77', NULL, N'MVCBase.ClientControllers.CAdvController', N'CAdv', 0, 1, 3)
SET IDENTITY_INSERT [dbo].[SysTemplateDetails] OFF
SET IDENTITY_INSERT [dbo].[SysTemplates] ON 

INSERT [dbo].[SysTemplates] ([ID], [LangID], [Name], [File], [Html]) VALUES (1, 1, N'Trang chủ', N'Layout.cshtml', N'<div id="render-layout">
					<div style="width:50%;float:left">
				       <static-layout code="CAdv" id="vswLogo" name="Logo"> Logo </static-layout></div><div style="width:50%;float:left"><static-layout code="CMenu" id="vswNav" name="Menu Top"> MenuTop </static-layout></div><div style="width:100%;float:left"><dynamic-layout code="CAdv" id="vswMain" name="Main"> MAin </dynamic-layout></div></div>')
SET IDENTITY_INSERT [dbo].[SysTemplates] OFF
ALTER TABLE [dbo].[CPLoginLogs] ADD  CONSTRAINT [DF_CPLoginLogs_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[CPMenus] ADD  CONSTRAINT [DF_CPMenus_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[CPRoles] ADD  CONSTRAINT [DF_CPRoles_Lock]  DEFAULT ((0)) FOR [Lock]
GO
ALTER TABLE [dbo].[ModCAdvs] ADD  CONSTRAINT [DF_ModCAdvs_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[ModNews] ADD  CONSTRAINT [DF_ModNews_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[ModNews] ADD  CONSTRAINT [DF_ModNews_Updated]  DEFAULT (getdate()) FOR [Updated]
GO
ALTER TABLE [dbo].[ModTags] ADD  CONSTRAINT [DF_ModTags_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[CPAccesses]  WITH CHECK ADD  CONSTRAINT [FK_CPAccesses_CPRoles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[CPRoles] ([ID])
GO
ALTER TABLE [dbo].[CPAccesses] CHECK CONSTRAINT [FK_CPAccesses_CPRoles]
GO
ALTER TABLE [dbo].[CPUsers]  WITH CHECK ADD  CONSTRAINT [FK_CPUsers_CPRoles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[CPRoles] ([ID])
GO
ALTER TABLE [dbo].[CPUsers] CHECK CONSTRAINT [FK_CPUsers_CPRoles]
GO
ALTER TABLE [dbo].[SysPages]  WITH CHECK ADD  CONSTRAINT [FK_SysPages_SysTemplates] FOREIGN KEY([TemplateID])
REFERENCES [dbo].[SysTemplates] ([ID])
GO
ALTER TABLE [dbo].[SysPages] CHECK CONSTRAINT [FK_SysPages_SysTemplates]
GO
ALTER TABLE [dbo].[WebPage]  WITH CHECK ADD  CONSTRAINT [FK_WebPage_SysTemplates] FOREIGN KEY([TemplateID])
REFERENCES [dbo].[SysTemplates] ([ID])
GO
ALTER TABLE [dbo].[WebPage] CHECK CONSTRAINT [FK_WebPage_SysTemplates]
GO
ALTER TABLE [dbo].[WebPageDetails]  WITH CHECK ADD  CONSTRAINT [FK_WebPageDetails_SysTemplates] FOREIGN KEY([TemplateID])
REFERENCES [dbo].[SysTemplates] ([ID])
GO
ALTER TABLE [dbo].[WebPageDetails] CHECK CONSTRAINT [FK_WebPageDetails_SysTemplates]
GO
ALTER TABLE [dbo].[WebPageDetails]  WITH CHECK ADD  CONSTRAINT [FK_WebPageDetails_WebPage] FOREIGN KEY([WebPageID])
REFERENCES [dbo].[WebPage] ([ID])
GO
ALTER TABLE [dbo].[WebPageDetails] CHECK CONSTRAINT [FK_WebPageDetails_WebPage]
GO

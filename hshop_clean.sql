/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[FullName] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[AvatarUrl] [nvarchar](max) NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[Gender] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CartItems]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CartItems](
	[CartItemId] [int] IDENTITY(1,1) NOT NULL,
	[CartId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_CartItems] PRIMARY KEY CLUSTERED 
(
	[CartItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Carts]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Carts](
	[CartId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NULL,
	[SessionId] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Carts] PRIMARY KEY CLUSTERED 
(
	[CartId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[Slug] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[ParentCategoryId] [int] NULL,
	[CategoryId1] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChatAttachments]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChatAttachments](
	[ChatAttachmentId] [uniqueidentifier] NOT NULL,
	[ChatMessageId] [uniqueidentifier] NOT NULL,
	[AttachmentType] [int] NOT NULL,
	[Url] [nvarchar](1000) NOT NULL,
	[FileName] [nvarchar](255) NULL,
	[ContentType] [nvarchar](100) NULL,
	[FileSize] [bigint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ChatAttachments] PRIMARY KEY CLUSTERED 
(
	[ChatAttachmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChatMessages]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChatMessages](
	[ChatMessageId] [uniqueidentifier] NOT NULL,
	[ConversationId] [uniqueidentifier] NOT NULL,
	[MessageType] [int] NOT NULL,
	[SenderType] [int] NOT NULL,
	[Content] [nvarchar](1000) NOT NULL,
	[Intent] [nvarchar](100) NULL,
	[Entitiesjson] [nvarchar](max) NULL,
	[RasaResponse] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ChatMessages] PRIMARY KEY CLUSTERED 
(
	[ChatMessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Conversations]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Conversations](
	[ConversationId] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[Title] [nvarchar](200) NULL,
	[IsClosed] [bit] NOT NULL,
	[EndedAt] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Conversations] PRIMARY KEY CLUSTERED 
(
	[ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Coupons]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Coupons](
	[CouponId] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[DiscountType] [nvarchar](20) NOT NULL,
	[DiscountValue] [decimal](18, 2) NOT NULL,
	[MinOrderAmount] [decimal](18, 2) NOT NULL,
	[MaxDiscountAmount] [decimal](18, 2) NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Coupons] PRIMARY KEY CLUSTERED 
(
	[CouponId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[OrderItemId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[ProductNameSnapshot] [nvarchar](200) NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[Quantity] [int] NOT NULL,
	[LineTotal] [decimal](18, 2) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED 
(
	[OrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[OrderCode] [nvarchar](50) NOT NULL,
	[CouponId] [int] NULL,
	[UserId] [nvarchar](450) NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[DiscountAmount] [decimal](18, 2) NOT NULL,
	[ShippingFee] [decimal](18, 2) NOT NULL,
	[FinalAmount] [decimal](18, 2) NOT NULL,
	[OrderStatus] [nvarchar](30) NOT NULL,
	[PaymentStatus] [nvarchar](30) NOT NULL,
	[PaymentMethod] [nvarchar](30) NOT NULL,
	[ReceiverName] [nvarchar](100) NOT NULL,
	[ReceiverPhone] [nvarchar](20) NOT NULL,
	[ShippingAddress] [nvarchar](255) NOT NULL,
	[Note] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[PaymentId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[PaymentMethod] [nvarchar](30) NOT NULL,
	[PaymentStatus] [nvarchar](30) NOT NULL,
	[TransactionCode] [nvarchar](100) NULL,
	[PaidAt] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductImages]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductImages](
	[ProductImageId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[ImageUrl] [nvarchar](500) NOT NULL,
	[IsPrimary] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ProductImages] PRIMARY KEY CLUSTERED 
(
	[ProductImageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[ProductName] [nvarchar](200) NOT NULL,
	[Slug] [nvarchar](250) NOT NULL,
	[ShortDescription] [nvarchar](500) NULL,
	[Description] [nvarchar](max) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[SalePrice] [decimal](18, 2) NULL,
	[StockQuantity] [int] NOT NULL,
	[ThumbnailUrl] [nvarchar](500) NULL,
	[IsFeatured] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reviews]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reviews](
	[ReviewId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[UserId] [nvarchar](450) NULL,
	[Rating] [int] NOT NULL,
	[Comment] [nvarchar](1000) NULL,
	[IsApproved] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Reviews] PRIMARY KEY CLUSTERED 
(
	[ReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAddresses]    Script Date: 9/3/2026 7:25:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAddresses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[FullName] [nvarchar](max) NOT NULL,
	[PhoneNumber] [nvarchar](max) NOT NULL,
	[SpecificAddress] [nvarchar](max) NOT NULL,
	[IsDefault] [bit] NOT NULL,
 CONSTRAINT [PK_UserAddresses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260711081423_InitialCreate', N'10.0.8')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260714051718_AddGenderToUser', N'10.0.8')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260715130159_AddBankAccountTable', N'10.0.8')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260716132212_RemoveBankAccountTable', N'10.0.8')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260716142114_UpdateUserAddressTable', N'10.0.8')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'050e27a5-5343-4f0c-a1f0-848004ae9e87', N'Customer', N'CUSTOMER', N'3acb8928-53cc-4e83-a043-aa8131827ca6')
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'b0ae710a-4e90-40bb-8859-1156c864c153', N'Admin', N'ADMIN', N'067577e9-ee32-43c7-994b-314ae7d21c39')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0f74b24f-b6a3-47b9-a746-f24d71f3ac34', N'050e27a5-5343-4f0c-a1f0-848004ae9e87')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'40800111-a0fe-43a6-a214-b4fd197ffc04', N'050e27a5-5343-4f0c-a1f0-848004ae9e87')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7aee3cc1-f94a-423b-bfb4-2622d58c4c12', N'b0ae710a-4e90-40bb-8859-1156c864c153')
GO
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [Address], [AvatarUrl], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Gender]) VALUES (N'0f74b24f-b6a3-47b9-a746-f24d71f3ac34', N'Kha Nguyen Huynh', N'151, Luong Chi 
', N'/Uploads/Avatars/495b08de-5699-44ba-b713-25486e953a1d_Watch.jpg', N'tnhuy807@gmail.com', N'TNHUY807@GMAIL.COM', N'tnhuy807@gmail.com', N'TNHUY807@GMAIL.COM', 1, N'AQAAAAIAAYagAAAAEDMvHkURnO9waEYFkvZsl3M8hi6NcD35HWqmPBIbRaqVnMPUrPpPOBDQA387pbQevg==', N'URXQEQ32A7MJQPHWSAJIYNAJVYKHGAC5', N'60c2e813-a785-43ce-8009-cd8a021c669a', N'0943265155', 0, 0, NULL, 1, 0, N'Nam')
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [Address], [AvatarUrl], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Gender]) VALUES (N'40800111-a0fe-43a6-a214-b4fd197ffc04', N'Kha Nguyen Huynh', N'1313131', NULL, N'khanguyen', N'KHANGUYEN', N'nhkhaktpm2411028@student.ctuet.edu.vn', N'NHKHAKTPM2411028@STUDENT.CTUET.EDU.VN', 0, N'AQAAAAIAAYagAAAAEH4Ou5lnqLUNQbL4wxqemBoRGnFe6b7OduSehs0ZYZ1WkGnvHQxpKv1hr0d/UBKtRQ==', N'2WGJGOOXVPXD677ISPFQLDQISJRO5AV4', N'1c2ba83b-700b-41a3-8b0f-24d4c268cf13', NULL, 0, 0, NULL, 1, 0, N'Nam')
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [Address], [AvatarUrl], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Gender]) VALUES (N'7aee3cc1-f94a-423b-bfb4-2622d58c4c12', N'Administrator', NULL, NULL, N'admin@gmail.com', N'ADMIN@GMAIL.COM', N'admin@gmail.com', N'ADMIN@GMAIL.COM', 1, N'AQAAAAIAAYagAAAAEHkC6T02MgRs+O39kTPWqjZVR41k79bYPnD/YBBpaVoZps9zjYFlBEbjMQdf8dNWLA==', N'KFBKYX67B2T5B4GQOOWMO2AKJJGTYRHR', N'394a4f82-721d-4ec6-9dee-41afb794f806', NULL, 0, 0, NULL, 1, 0, NULL)
GO
SET IDENTITY_INSERT [dbo].[CartItems] ON 

INSERT [dbo].[CartItems] ([CartItemId], [CartId], [ProductId], [Quantity], [UnitPrice], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (8, 2, 27, 1, CAST(204000.00 AS Decimal(18, 2)), CAST(N'2026-07-29T05:55:29.4088483' AS DateTime2), NULL, 1)
SET IDENTITY_INSERT [dbo].[CartItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Carts] ON 

INSERT [dbo].[Carts] ([CartId], [UserId], [SessionId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (1, N'0f74b24f-b6a3-47b9-a746-f24d71f3ac34', NULL, CAST(N'2026-07-22T07:46:40.9055832' AS DateTime2), NULL, 1)
INSERT [dbo].[Carts] ([CartId], [UserId], [SessionId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (2, N'40800111-a0fe-43a6-a214-b4fd197ffc04', NULL, CAST(N'2026-07-28T13:53:25.8061877' AS DateTime2), NULL, 1)
SET IDENTITY_INSERT [dbo].[Carts] OFF
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (1, N'Cần câu', N'can-cau', N'Danh mục cần câu cho nhiều kiểu câu khác nhau.', NULL, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (2, N'Mồi câu', N'moi-cau', N'Danh mục mồi câu giả, mồi mềm và mồi chuyên dụng.', NULL, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (3, N'Dây câu', N'day-cau', N'Danh mục dây câu PE, nylon, fluorocarbon và leader.', NULL, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (4, N'Phụ kiện', N'phu-kien', N'Danh mục phụ kiện và dụng cụ hỗ trợ câu cá.', NULL, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (5, N'Cần câu Shimano', N'can-cau-shimano', N'Các mẫu cần câu thương hiệu Shimano.', 1, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (6, N'Cần câu Daiwa', N'can-cau-daiwa', N'Các mẫu cần câu thương hiệu Daiwa.', 1, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (7, N'Cần câu Abu Garcia', N'can-cau-abu-garcia', N'Các mẫu cần câu thương hiệu Abu Garcia.', 1, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (8, N'Cần câu Lure', N'can-cau-lure', N'Cần câu chuyên dùng cho kỹ thuật lure.', 1, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (9, N'Cần câu Surf', N'can-cau-surf', N'Cần câu bờ biển và câu xa.', 1, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (10, N'Mồi giả Hard Bait', N'moi-gia-hard-bait', N'Mồi giả thân cứng dùng cho lure.', 2, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (11, N'Mồi mềm Soft Bait', N'moi-mem-soft-bait', N'Mồi mềm silicon nhiều màu và kích thước.', 2, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (12, N'Mồi kim loại', N'moi-kim-loai', N'Mồi thìa, jig và spinner kim loại.', 2, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (13, N'Mồi câu cá chép', N'moi-cau-ca-chep', N'Mồi chuyên dụng cho cá chép.', 2, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (14, N'Mồi câu cá rô phi', N'moi-cau-ca-ro-phi', N'Mồi chuyên dụng cho cá rô phi.', 2, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (15, N'Dây PE', N'day-pe', N'Dây bện PE chịu lực cao.', 3, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (16, N'Dây Nylon', N'day-nylon', N'Dây nylon dễ sử dụng và đa dụng.', 3, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (17, N'Dây Fluorocarbon', N'day-fluorocarbon', N'Dây fluorocarbon ít thấy dưới nước.', 3, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (18, N'Dây Leader', N'day-leader', N'Dây leader chống mài mòn.', 3, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (19, N'Dây thẻo', N'day-theo', N'Dây thẻo buộc sẵn và dây làm thẻo.', 3, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (20, N'Lưỡi câu', N'luoi-cau', N'Nhiều loại lưỡi câu cho từng loại cá.', 4, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (21, N'Phao câu', N'phao-cau', N'Phao câu hồ, câu đài và câu sông.', 4, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (22, N'Hộp và túi đựng', N'hop-va-tui-dung', N'Hộp mồi và túi đựng dụng cụ câu cá.', 4, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (23, N'Kìm và dụng cụ', N'kim-va-dung-cu', N'Kìm, kéo, gỡ lưỡi và dụng cụ hỗ trợ.', 4, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Slug], [Description], [ParentCategoryId], [CategoryId1], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (24, N'Giá đỡ và ghế câu', N'gia-do-va-ghe-cau', N'Giá đỡ cần, ghế và phụ kiện dã ngoại.', 4, NULL, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderItems] ON 

INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [ProductNameSnapshot], [UnitPrice], [Quantity], [LineTotal], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (1, 1, 1, N'Cần câu Abu Garcia Veritas 662M', CAST(2708000.00 AS Decimal(18, 2)), 1, CAST(2708000.00 AS Decimal(18, 2)), CAST(N'2026-07-28T13:57:52.6730185' AS DateTime2), NULL, 1)
INSERT [dbo].[OrderItems] ([OrderItemId], [OrderId], [ProductId], [ProductNameSnapshot], [UnitPrice], [Quantity], [LineTotal], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (2, 2, 1, N'Cần câu Abu Garcia Veritas 662M', CAST(2708000.00 AS Decimal(18, 2)), 3, CAST(8124000.00 AS Decimal(18, 2)), CAST(N'2026-07-29T05:54:41.3065611' AS DateTime2), NULL, 1)
SET IDENTITY_INSERT [dbo].[OrderItems] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 

INSERT [dbo].[Orders] ([OrderId], [OrderCode], [CouponId], [UserId], [TotalAmount], [DiscountAmount], [ShippingFee], [FinalAmount], [OrderStatus], [PaymentStatus], [PaymentMethod], [ReceiverName], [ReceiverPhone], [ShippingAddress], [Note], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (1, N'OD20260728205752409', NULL, N'40800111-a0fe-43a6-a214-b4fd197ffc04', CAST(2708000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(2708000.00 AS Decimal(18, 2)), N'Pending', N'Unpaid', N'COD', N'Kha Nguyen Huynh', N'0934010248', N'qqwqq', N'qưq', CAST(N'2026-07-28T13:57:52.4097132' AS DateTime2), NULL, 1)
INSERT [dbo].[Orders] ([OrderId], [OrderCode], [CouponId], [UserId], [TotalAmount], [DiscountAmount], [ShippingFee], [FinalAmount], [OrderStatus], [PaymentStatus], [PaymentMethod], [ReceiverName], [ReceiverPhone], [ShippingAddress], [Note], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (2, N'OD20260729125441063', NULL, N'40800111-a0fe-43a6-a214-b4fd197ffc04', CAST(8124000.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(8124000.00 AS Decimal(18, 2)), N'Pending', N'Unpaid', N'COD', N'Kha Nguyen Huynh', N'0934010248', N'151, Lương chí', N'', CAST(N'2026-07-29T05:54:41.0636192' AS DateTime2), NULL, 1)
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[Payments] ON 

INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentStatus], [TransactionCode], [PaidAt], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (1, 1, N'COD', N'Unpaid', NULL, NULL, CAST(N'2026-07-28T13:57:52.7000749' AS DateTime2), NULL, 1)
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentStatus], [TransactionCode], [PaidAt], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (2, 2, N'COD', N'Unpaid', NULL, NULL, CAST(N'2026-07-29T05:54:41.3372464' AS DateTime2), NULL, 1)
SET IDENTITY_INSERT [dbo].[Payments] OFF
GO
SET IDENTITY_INSERT [dbo].[ProductImages] ON 

INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (1, 1, N'/Uploads/products/0c59a15a-aab8-4df7-83a1-97ee87f468d6.jpg', 1, 1, CAST(N'2026-07-22T08:24:19.2310962' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (2, 2, N'/Uploads/products/d170104e-fcee-4e8e-a1a3-e4d2605bbcd1.webp', 1, 1, CAST(N'2026-07-28T13:27:25.3505289' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (3, 3, N'/Uploads/products/bed0fc06-9a68-47d0-baf5-a9c71ddcfa8e.webp', 1, 1, CAST(N'2026-07-28T13:27:52.0859551' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (4, 4, N'/Uploads/products/0cc04543-44de-4284-a51c-f2d426faf3a7.jpg', 1, 1, CAST(N'2026-07-28T13:28:20.8704274' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (5, 5, N'/Uploads/products/fee0e205-f58c-490b-b19a-d88f66f8c1cc.webp', 1, 1, CAST(N'2026-07-28T13:29:37.2122805' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (6, 6, N'/Uploads/products/5e8455ff-e993-4640-9089-93a6966805bd.jpg', 1, 1, CAST(N'2026-07-28T13:30:28.9716836' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (7, 7, N'/Uploads/products/0ce82732-05bd-4427-8a59-421e0e476a85.jpg', 1, 1, CAST(N'2026-07-28T13:31:05.1111725' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (8, 8, N'/Uploads/products/e0f35a4e-7ebd-4a43-8da0-e9c5a97828f3.webp', 1, 1, CAST(N'2026-07-28T13:31:41.0092942' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (9, 9, N'/Uploads/products/706cb181-256d-42ec-8198-9c17b5390a5a.jpg', 1, 1, CAST(N'2026-07-28T13:32:29.6958723' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (10, 10, N'/Uploads/products/1468b614-29e8-445e-869e-cbb069b8757b.webp', 1, 1, CAST(N'2026-07-28T13:32:54.9889329' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (11, 11, N'/Uploads/products/1ed1abef-7d14-4b1c-8969-ab04fd927aaa.jpg', 1, 1, CAST(N'2026-07-28T13:33:37.8566351' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (12, 12, N'/Uploads/products/bdb1ac66-d5e7-465c-860e-95ba8f73a27e.jpg', 1, 1, CAST(N'2026-07-28T13:34:16.4993252' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (13, 13, N'/Uploads/products/138ef562-30ee-4abb-bd99-a2b0ea192ef7.jpg', 1, 1, CAST(N'2026-07-28T13:34:44.5326457' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (14, 15, N'/Uploads/products/19b5e38f-20b1-47b6-bb0b-ce407b3cdae0.webp', 1, 1, CAST(N'2026-07-28T13:35:24.0500542' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (15, 14, N'/Uploads/products/a9913f54-2de9-4665-b33f-5d95b7729dd5.webp', 1, 1, CAST(N'2026-07-28T13:35:47.8353563' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (16, 16, N'/Uploads/products/4836e540-3fc0-48b1-a020-dd65b61eb4c6.jpg', 1, 1, CAST(N'2026-07-28T13:36:16.5120473' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (17, 17, N'/Uploads/products/0d05f244-2256-4dca-9f44-b0e4658e5dd4.jpg', 1, 1, CAST(N'2026-07-28T13:36:55.1513986' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (18, 18, N'/Uploads/products/ebfbd344-d94f-41f4-b592-1e797595eb48.jpg', 1, 1, CAST(N'2026-07-28T13:37:35.0343809' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (19, 19, N'/Uploads/products/30e8b0ca-4621-4231-94d8-f1581b817ec6.webp', 1, 1, CAST(N'2026-07-28T13:37:59.0139918' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (20, 20, N'/Uploads/products/fd2ccd07-4fa4-4ba4-8d6e-fcb6ff469a2e.jpg', 1, 1, CAST(N'2026-07-28T13:38:28.8142379' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (21, 21, N'/Uploads/products/a632163a-fbfa-4af3-b485-1592cdf7dd97.webp', 1, 1, CAST(N'2026-07-28T13:38:54.1885508' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (22, 22, N'/Uploads/products/50a7a70d-3cf6-4222-b0ee-d27815b8ba5a.jpg', 1, 1, CAST(N'2026-07-28T13:39:20.7757999' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (23, 23, N'/Uploads/products/7f396ef8-c48a-46cb-982d-5228539a3ddf.webp', 1, 1, CAST(N'2026-07-28T13:39:52.7181696' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (24, 24, N'/Uploads/products/f74bc0e6-983f-4318-9ebe-0c724fbe6c31.jpg', 1, 1, CAST(N'2026-07-28T13:40:20.1946755' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (25, 25, N'/Uploads/products/95519c2b-c3cd-4473-a9fb-e563cd2b0b14.webp', 1, 1, CAST(N'2026-07-28T13:40:52.4072961' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (26, 26, N'/Uploads/products/886dd753-a948-4e86-b13a-2d976f0e09f0.webp', 1, 1, CAST(N'2026-07-28T13:41:17.5284422' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (27, 28, N'/Uploads/products/b6674e02-7261-4119-b8e1-50ca946cc2e7.jpg', 1, 1, CAST(N'2026-07-28T13:42:01.3630687' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (28, 27, N'/Uploads/products/c3745c2c-b807-46c0-9c98-68800bbdd6a8.jpg', 1, 1, CAST(N'2026-07-28T13:42:29.8551318' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (29, 29, N'/Uploads/products/3e3d2dfe-d131-447b-a7f3-5b3b792b0589.webp', 1, 1, CAST(N'2026-07-28T13:43:04.1506547' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (30, 35, N'/Uploads/products/f78c7152-8a42-43c9-a8ff-768fb2497109.webp', 1, 1, CAST(N'2026-07-28T13:43:47.3814176' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (31, 40, N'/Uploads/products/03376216-9dad-4a12-9015-ead71eca90af.jpg', 1, 1, CAST(N'2026-07-28T13:44:18.9280996' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (32, 55, N'/Uploads/products/65960bca-47e1-4f46-8ed8-18731aac6a0c.jpeg', 1, 1, CAST(N'2026-07-28T13:44:58.1036948' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (33, 60, N'/Uploads/products/1472336f-dd5b-4d06-bf22-5bcf865bd059.webp', 1, 1, CAST(N'2026-07-28T13:45:29.9373059' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (34, 70, N'/Uploads/products/931ad425-1ae5-4c19-a721-e8b349d38e26.webp', 1, 1, CAST(N'2026-07-28T13:46:13.0036619' AS DateTime2), NULL, 1)
INSERT [dbo].[ProductImages] ([ProductImageId], [ProductId], [ImageUrl], [IsPrimary], [SortOrder], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (35, 80, N'/Uploads/products/d8178114-76fd-4dac-8052-945bb1178e65.webp', 1, 1, CAST(N'2026-07-28T13:47:07.7368056' AS DateTime2), NULL, 1)
SET IDENTITY_INSERT [dbo].[ProductImages] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (1, 7, N'Cần câu Abu Garcia Veritas 662M', N'can-cau-abu-garcia-veritas-662m', N'Cần câu Abu Garcia Veritas 662M - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Abu Garcia Veritas 662M phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(2850000.00 AS Decimal(18, 2)), CAST(2708000.00 AS Decimal(18, 2)), 87, N'/Uploads/products/0c59a15a-aab8-4df7-83a1-97ee87f468d6.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T15:24:19.2768388' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (2, 7, N'Cần câu Abu Garcia Vendetta 702MH', N'can-cau-abu-garcia-vendetta-702mh', N'Cần câu Abu Garcia Vendetta 702MH - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Abu Garcia Vendetta 702MH phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(2150000.00 AS Decimal(18, 2)), CAST(2150000.00 AS Decimal(18, 2)), 94, N'/Uploads/products/d170104e-fcee-4e8e-a1a3-e4d2605bbcd1.webp', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:27:25.4789914' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (3, 7, N'Cần câu Abu Garcia Vengeance 662ML', N'can-cau-abu-garcia-vengeance-662ml', N'Cần câu Abu Garcia Vengeance 662ML - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Abu Garcia Vengeance 662ML phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(1350000.00 AS Decimal(18, 2)), CAST(1242000.00 AS Decimal(18, 2)), 10, N'/Uploads/products/bed0fc06-9a68-47d0-baf5-a9c71ddcfa8e.webp', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:27:52.0880651' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (4, 7, N'Cần câu Abu Garcia Black Max 602M', N'can-cau-abu-garcia-black-max-602m', N'Cần câu Abu Garcia Black Max 602M - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Abu Garcia Black Max 602M phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(1180000.00 AS Decimal(18, 2)), CAST(1121000.00 AS Decimal(18, 2)), 17, N'/Uploads/products/0cc04543-44de-4284-a51c-f2d426faf3a7.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:28:20.8718463' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (5, 7, N'Cần câu Abu Garcia Beast Pro 702H', N'can-cau-abu-garcia-beast-pro-702h', N'Cần câu Abu Garcia Beast Pro 702H - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Abu Garcia Beast Pro 702H phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(3250000.00 AS Decimal(18, 2)), CAST(2925000.00 AS Decimal(18, 2)), 24, N'/Uploads/products/fee0e205-f58c-490b-b19a-d88f66f8c1cc.webp', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:29:37.2136073' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (6, 6, N'Cần câu Daiwa Tatula XT 662MFB', N'can-cau-daiwa-tatula-xt-662mfb', N'Cần câu Daiwa Tatula XT 662MFB - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Daiwa Tatula XT 662MFB phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(2950000.00 AS Decimal(18, 2)), CAST(2655000.00 AS Decimal(18, 2)), 52, N'/Uploads/products/5e8455ff-e993-4640-9089-93a6966805bd.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:30:28.9744545' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (7, 6, N'Cần câu Daiwa Crossfire 702MFS', N'can-cau-daiwa-crossfire-702mfs', N'Cần câu Daiwa Crossfire 702MFS - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Daiwa Crossfire 702MFS phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(1150000.00 AS Decimal(18, 2)), CAST(1058000.00 AS Decimal(18, 2)), 59, N'/Uploads/products/0ce82732-05bd-4427-8a59-421e0e476a85.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:31:05.1123187' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (8, 6, N'Cần câu Daiwa Laguna 662MLFS', N'can-cau-daiwa-laguna-662mlfs', N'Cần câu Daiwa Laguna 662MLFS - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Daiwa Laguna 662MLFS phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(1450000.00 AS Decimal(18, 2)), CAST(1450000.00 AS Decimal(18, 2)), 66, N'/Uploads/products/e0f35a4e-7ebd-4a43-8da0-e9c5a97828f3.webp', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:31:41.0095795' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (9, 6, N'Cần câu Daiwa Aird X 701MHFB', N'can-cau-daiwa-aird-x-701mhfb', N'Cần câu Daiwa Aird X 701MHFB - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Daiwa Aird X 701MHFB phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(2350000.00 AS Decimal(18, 2)), CAST(2115000.00 AS Decimal(18, 2)), 73, N'/Uploads/products/706cb181-256d-42ec-8198-9c17b5390a5a.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:32:29.6979946' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (10, 6, N'Cần câu Daiwa Sweepfire 270', N'can-cau-daiwa-sweepfire-270', N'Cần câu Daiwa Sweepfire 270 - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Daiwa Sweepfire 270 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(890000.00 AS Decimal(18, 2)), CAST(819000.00 AS Decimal(18, 2)), 80, N'/Uploads/products/1468b614-29e8-445e-869e-cbb069b8757b.webp', 1, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:32:54.9903815' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (11, 8, N'Cần lure Carbon Hunter 1m98 ML', N'can-lure-carbon-hunter-1m98-ml', N'Cần lure Carbon Hunter 1m98 ML - sản phẩm mẫu cho website FuuFishing.', N'Cần lure Carbon Hunter 1m98 ML phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(790000.00 AS Decimal(18, 2)), CAST(790000.00 AS Decimal(18, 2)), 31, N'/Uploads/products/1ed1abef-7d14-4b1c-8969-ab04fd927aaa.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:33:37.8583070' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (12, 8, N'Cần lure Stream Master 2m10 M', N'can-lure-stream-master-2m10-m', N'Cần lure Stream Master 2m10 M - sản phẩm mẫu cho website FuuFishing.', N'Cần lure Stream Master 2m10 M phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(950000.00 AS Decimal(18, 2)), CAST(902000.00 AS Decimal(18, 2)), 38, N'/Uploads/products/bdb1ac66-d5e7-465c-860e-95ba8f73a27e.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:34:16.5008839' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (13, 8, N'Cần lure Bass Force 1m80 UL', N'can-lure-bass-force-1m80-ul', N'Cần lure Bass Force 1m80 UL - sản phẩm mẫu cho website FuuFishing.', N'Cần lure Bass Force 1m80 UL phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(690000.00 AS Decimal(18, 2)), CAST(621000.00 AS Decimal(18, 2)), 45, N'/Uploads/products/138ef562-30ee-4abb-bd99-a2b0ea192ef7.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:34:44.5334546' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (14, 8, N'Cần lure Snakehead Pro 2m13 H', N'can-lure-snakehead-pro-2m13-h', N'Cần lure Snakehead Pro 2m13 H - sản phẩm mẫu cho website FuuFishing.', N'Cần lure Snakehead Pro 2m13 H phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(1290000.00 AS Decimal(18, 2)), CAST(1187000.00 AS Decimal(18, 2)), 52, N'/Uploads/products/a9913f54-2de9-4665-b33f-5d95b7729dd5.webp', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:35:47.8366183' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (15, 8, N'Cần lure Travel 5 khúc 2m10', N'can-lure-travel-5-khuc-2m10', N'Cần lure Travel 5 khúc 2m10 - sản phẩm mẫu cho website FuuFishing.', N'Cần lure Travel 5 khúc 2m10 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(1080000.00 AS Decimal(18, 2)), CAST(1080000.00 AS Decimal(18, 2)), 59, N'/Uploads/products/19b5e38f-20b1-47b6-bb0b-ce407b3cdae0.webp', 1, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:35:24.0506854' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (16, 5, N'Cần câu Shimano Zodias 166ML', N'can-cau-shimano-zodias-166ml', N'Cần câu Shimano Zodias 166ML - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Shimano Zodias 166ML phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(3850000.00 AS Decimal(18, 2)), CAST(3542000.00 AS Decimal(18, 2)), 17, N'/Uploads/products/4836e540-3fc0-48b1-a020-dd65b61eb4c6.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:36:16.5129401' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (17, 5, N'Cần câu Shimano Expride 1610M', N'can-cau-shimano-expride-1610m', N'Cần câu Shimano Expride 1610M - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Shimano Expride 1610M phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(5250000.00 AS Decimal(18, 2)), CAST(4988000.00 AS Decimal(18, 2)), 24, N'/Uploads/products/0d05f244-2256-4dca-9f44-b0e4658e5dd4.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:36:55.1521507' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (18, 5, N'Cần câu Shimano Catana FX 270M', N'can-cau-shimano-catana-fx-270m', N'Cần câu Shimano Catana FX 270M - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Shimano Catana FX 270M phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(1650000.00 AS Decimal(18, 2)), CAST(1485000.00 AS Decimal(18, 2)), 31, N'/Uploads/products/ebfbd344-d94f-41f4-b592-1e797595eb48.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:37:35.0350648' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (19, 5, N'Cần câu Shimano Solara 213MH', N'can-cau-shimano-solara-213mh', N'Cần câu Shimano Solara 213MH - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Shimano Solara 213MH phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(1280000.00 AS Decimal(18, 2)), CAST(1280000.00 AS Decimal(18, 2)), 38, N'/Uploads/products/30e8b0ca-4621-4231-94d8-f1581b817ec6.webp', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:37:59.0141594' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (20, 5, N'Cần câu Shimano Alivio DX 240', N'can-cau-shimano-alivio-dx-240', N'Cần câu Shimano Alivio DX 240 - sản phẩm mẫu cho website FuuFishing.', N'Cần câu Shimano Alivio DX 240 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(980000.00 AS Decimal(18, 2)), CAST(931000.00 AS Decimal(18, 2)), 45, N'/Uploads/products/fd2ccd07-4fa4-4ba4-8d6e-fcb6ff469a2e.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:38:28.8143708' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (21, 9, N'Cần Surf Ocean Cast 3m90', N'can-surf-ocean-cast-3m90', N'Cần Surf Ocean Cast 3m90 - sản phẩm mẫu cho website FuuFishing.', N'Cần Surf Ocean Cast 3m90 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(1850000.00 AS Decimal(18, 2)), CAST(1665000.00 AS Decimal(18, 2)), 66, N'/Uploads/products/a632163a-fbfa-4af3-b485-1592cdf7dd97.webp', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:38:54.1894764' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (22, 9, N'Cần Surf Long Beach 4m20', N'can-surf-long-beach-4m20', N'Cần Surf Long Beach 4m20 - sản phẩm mẫu cho website FuuFishing.', N'Cần Surf Long Beach 4m20 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(2450000.00 AS Decimal(18, 2)), CAST(2254000.00 AS Decimal(18, 2)), 73, N'/Uploads/products/50a7a70d-3cf6-4222-b0ee-d27815b8ba5a.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:39:20.7761782' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (23, 9, N'Cần Surf Carbon Distance 4m50', N'can-surf-carbon-distance-4m50', N'Cần Surf Carbon Distance 4m50 - sản phẩm mẫu cho website FuuFishing.', N'Cần Surf Carbon Distance 4m50 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(3150000.00 AS Decimal(18, 2)), CAST(2992000.00 AS Decimal(18, 2)), 80, N'/Uploads/products/7f396ef8-c48a-46cb-982d-5228539a3ddf.webp', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:39:52.7191697' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (24, 9, N'Cần Surf Shore Master 3m60', N'can-surf-shore-master-3m60', N'Cần Surf Shore Master 3m60 - sản phẩm mẫu cho website FuuFishing.', N'Cần Surf Shore Master 3m60 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(1680000.00 AS Decimal(18, 2)), CAST(1680000.00 AS Decimal(18, 2)), 87, N'/Uploads/products/f74bc0e6-983f-4318-9ebe-0c724fbe6c31.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:40:20.1955856' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (25, 9, N'Cần Surf Sea Hunter 4m20', N'can-surf-sea-hunter-4m20', N'Cần Surf Sea Hunter 4m20 - sản phẩm mẫu cho website FuuFishing.', N'Cần Surf Sea Hunter 4m20 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(2750000.00 AS Decimal(18, 2)), CAST(2530000.00 AS Decimal(18, 2)), 94, N'/Uploads/products/95519c2b-c3cd-4473-a9fb-e563cd2b0b14.webp', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:40:52.4074406' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (26, 17, N'Dây Fluorocarbon 50m 0.22mm', N'day-fluorocarbon-50m-0-22mm', N'Dây Fluorocarbon 50m 0.22mm - sản phẩm mẫu cho website FuuFishing.', N'Dây Fluorocarbon 50m 0.22mm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(185000.00 AS Decimal(18, 2)), CAST(170000.00 AS Decimal(18, 2)), 73, N'/Uploads/products/886dd753-a948-4e86-b13a-2d976f0e09f0.webp', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:41:17.5310293' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (27, 17, N'Dây Fluorocarbon 50m 0.28mm', N'day-fluorocarbon-50m-0-28mm', N'Dây Fluorocarbon 50m 0.28mm - sản phẩm mẫu cho website FuuFishing.', N'Dây Fluorocarbon 50m 0.28mm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(215000.00 AS Decimal(18, 2)), CAST(204000.00 AS Decimal(18, 2)), 80, N'/Uploads/products/c3745c2c-b807-46c0-9c98-68800bbdd6a8.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:42:29.8560901' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (28, 17, N'Dây Fluorocarbon 100m 0.33mm', N'day-fluorocarbon-100m-0-33mm', N'Dây Fluorocarbon 100m 0.33mm - sản phẩm mẫu cho website FuuFishing.', N'Dây Fluorocarbon 100m 0.33mm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(365000.00 AS Decimal(18, 2)), CAST(328000.00 AS Decimal(18, 2)), 87, N'/Uploads/products/b6674e02-7261-4119-b8e1-50ca946cc2e7.jpg', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:42:01.3643000' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (29, 17, N'Dây Fluorocarbon Invisible 50m', N'day-fluorocarbon-invisible-50m', N'Dây Fluorocarbon Invisible 50m - sản phẩm mẫu cho website FuuFishing.', N'Dây Fluorocarbon Invisible 50m phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(255000.00 AS Decimal(18, 2)), CAST(255000.00 AS Decimal(18, 2)), 94, N'/Uploads/products/3e3d2dfe-d131-447b-a7f3-5b3b792b0589.webp', 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:43:04.1521014' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (30, 17, N'Dây Fluorocarbon Premium 100m', N'day-fluorocarbon-premium-100m', N'Dây Fluorocarbon Premium 100m - sản phẩm mẫu cho website FuuFishing.', N'Dây Fluorocarbon Premium 100m phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(495000.00 AS Decimal(18, 2)), CAST(470000.00 AS Decimal(18, 2)), 10, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (31, 18, N'Dây Leader chống mài mòn 30lb', N'day-leader-chong-mai-mon-30lb', N'Dây Leader chống mài mòn 30lb - sản phẩm mẫu cho website FuuFishing.', N'Dây Leader chống mài mòn 30lb phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(145000.00 AS Decimal(18, 2)), CAST(130000.00 AS Decimal(18, 2)), 17, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (32, 18, N'Dây Leader chống mài mòn 50lb', N'day-leader-chong-mai-mon-50lb', N'Dây Leader chống mài mòn 50lb - sản phẩm mẫu cho website FuuFishing.', N'Dây Leader chống mài mòn 50lb phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(175000.00 AS Decimal(18, 2)), CAST(161000.00 AS Decimal(18, 2)), 24, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (33, 18, N'Dây Leader Shock 80lb', N'day-leader-shock-80lb', N'Dây Leader Shock 80lb - sản phẩm mẫu cho website FuuFishing.', N'Dây Leader Shock 80lb phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(225000.00 AS Decimal(18, 2)), CAST(225000.00 AS Decimal(18, 2)), 31, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (34, 18, N'Dây Leader Carbon 20m 25lb', N'day-leader-carbon-20m-25lb', N'Dây Leader Carbon 20m 25lb - sản phẩm mẫu cho website FuuFishing.', N'Dây Leader Carbon 20m 25lb phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(195000.00 AS Decimal(18, 2)), CAST(176000.00 AS Decimal(18, 2)), 38, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (35, 18, N'Dây Leader biển 100lb', N'day-leader-bien-100lb', N'Dây Leader biển 100lb - sản phẩm mẫu cho website FuuFishing.', N'Dây Leader biển 100lb phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(285000.00 AS Decimal(18, 2)), CAST(262000.00 AS Decimal(18, 2)), 45, N'/Uploads/products/f78c7152-8a42-43c9-a8ff-768fb2497109.webp', 1, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:43:47.3823078' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (36, 16, N'Dây Nylon Clear 100m 0.25mm', N'day-nylon-clear-100m-0-25mm', N'Dây Nylon Clear 100m 0.25mm - sản phẩm mẫu cho website FuuFishing.', N'Dây Nylon Clear 100m 0.25mm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(85000.00 AS Decimal(18, 2)), CAST(85000.00 AS Decimal(18, 2)), 38, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (37, 16, N'Dây Nylon Clear 100m 0.30mm', N'day-nylon-clear-100m-0-30mm', N'Dây Nylon Clear 100m 0.30mm - sản phẩm mẫu cho website FuuFishing.', N'Dây Nylon Clear 100m 0.30mm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(92000.00 AS Decimal(18, 2)), CAST(83000.00 AS Decimal(18, 2)), 45, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (38, 16, N'Dây Nylon Fluoro Coated 150m', N'day-nylon-fluoro-coated-150m', N'Dây Nylon Fluoro Coated 150m - sản phẩm mẫu cho website FuuFishing.', N'Dây Nylon Fluoro Coated 150m phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(135000.00 AS Decimal(18, 2)), CAST(124000.00 AS Decimal(18, 2)), 52, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (39, 16, N'Dây Nylon Soft Cast 300m', N'day-nylon-soft-cast-300m', N'Dây Nylon Soft Cast 300m - sản phẩm mẫu cho website FuuFishing.', N'Dây Nylon Soft Cast 300m phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(185000.00 AS Decimal(18, 2)), CAST(176000.00 AS Decimal(18, 2)), 59, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (40, 16, N'Dây Nylon Super Strong 500m', N'day-nylon-super-strong-500m', N'Dây Nylon Super Strong 500m - sản phẩm mẫu cho website FuuFishing.', N'Dây Nylon Super Strong 500m phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(245000.00 AS Decimal(18, 2)), CAST(245000.00 AS Decimal(18, 2)), 66, N'/Uploads/products/03376216-9dad-4a12-9015-ead71eca90af.jpg', 1, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:44:18.9313111' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (41, 15, N'Dây PE X8 150m size 1.0', N'day-pe-x8-150m-size-1-0', N'Dây PE X8 150m size 1.0 - sản phẩm mẫu cho website FuuFishing.', N'Dây PE X8 150m size 1.0 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(285000.00 AS Decimal(18, 2)), CAST(256000.00 AS Decimal(18, 2)), 94, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (42, 15, N'Dây PE X8 150m size 1.5', N'day-pe-x8-150m-size-1-5', N'Dây PE X8 150m size 1.5 - sản phẩm mẫu cho website FuuFishing.', N'Dây PE X8 150m size 1.5 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(295000.00 AS Decimal(18, 2)), CAST(295000.00 AS Decimal(18, 2)), 10, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (43, 15, N'Dây PE X8 200m size 2.0', N'day-pe-x8-200m-size-2-0', N'Dây PE X8 200m size 2.0 - sản phẩm mẫu cho website FuuFishing.', N'Dây PE X8 200m size 2.0 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(365000.00 AS Decimal(18, 2)), CAST(347000.00 AS Decimal(18, 2)), 17, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (44, 15, N'Dây PE X4 100m size 0.8', N'day-pe-x4-100m-size-0-8', N'Dây PE X4 100m size 0.8 - sản phẩm mẫu cho website FuuFishing.', N'Dây PE X4 100m size 0.8 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(165000.00 AS Decimal(18, 2)), CAST(148000.00 AS Decimal(18, 2)), 24, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (45, 15, N'Dây PE X9 300m size 3.0', N'day-pe-x9-300m-size-3-0', N'Dây PE X9 300m size 3.0 - sản phẩm mẫu cho website FuuFishing.', N'Dây PE X9 300m size 3.0 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(595000.00 AS Decimal(18, 2)), CAST(547000.00 AS Decimal(18, 2)), 31, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (46, 19, N'Dây thẻo câu đài 10 bộ', N'day-theo-cau-dai-10-bo', N'Dây thẻo câu đài 10 bộ - sản phẩm mẫu cho website FuuFishing.', N'Dây thẻo câu đài 10 bộ phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(75000.00 AS Decimal(18, 2)), CAST(71000.00 AS Decimal(18, 2)), 52, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (47, 19, N'Dây thẻo câu lục 5 bộ', N'day-theo-cau-luc-5-bo', N'Dây thẻo câu lục 5 bộ - sản phẩm mẫu cho website FuuFishing.', N'Dây thẻo câu lục 5 bộ phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(85000.00 AS Decimal(18, 2)), CAST(85000.00 AS Decimal(18, 2)), 59, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (48, 19, N'Dây thẻo thép chống cắn 20cm', N'day-theo-thep-chong-can-20cm', N'Dây thẻo thép chống cắn 20cm - sản phẩm mẫu cho website FuuFishing.', N'Dây thẻo thép chống cắn 20cm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(68000.00 AS Decimal(18, 2)), CAST(63000.00 AS Decimal(18, 2)), 66, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (49, 19, N'Dây thẻo đôi câu đáy 10 bộ', N'day-theo-doi-cau-day-10-bo', N'Dây thẻo đôi câu đáy 10 bộ - sản phẩm mẫu cho website FuuFishing.', N'Dây thẻo đôi câu đáy 10 bộ phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(92000.00 AS Decimal(18, 2)), CAST(87000.00 AS Decimal(18, 2)), 73, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (50, 19, N'Dây thẻo fluorocarbon buộc sẵn', N'day-theo-fluorocarbon-buoc-san', N'Dây thẻo fluorocarbon buộc sẵn - sản phẩm mẫu cho website FuuFishing.', N'Dây thẻo fluorocarbon buộc sẵn phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(125000.00 AS Decimal(18, 2)), CAST(112000.00 AS Decimal(18, 2)), 80, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (51, 24, N'Giá đỡ cần câu inox chữ V', N'gia-do-can-cau-inox-chu-v', N'Giá đỡ cần câu inox chữ V - sản phẩm mẫu cho website FuuFishing.', N'Giá đỡ cần câu inox chữ V phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(125000.00 AS Decimal(18, 2)), CAST(125000.00 AS Decimal(18, 2)), 45, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (52, 24, N'Giá đỡ cần câu 3 chân', N'gia-do-can-cau-3-chan', N'Giá đỡ cần câu 3 chân - sản phẩm mẫu cho website FuuFishing.', N'Giá đỡ cần câu 3 chân phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(285000.00 AS Decimal(18, 2)), CAST(262000.00 AS Decimal(18, 2)), 52, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (53, 24, N'Ghế xếp câu cá mini', N'ghe-xep-cau-ca-mini', N'Ghế xếp câu cá mini - sản phẩm mẫu cho website FuuFishing.', N'Ghế xếp câu cá mini phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(350000.00 AS Decimal(18, 2)), CAST(332000.00 AS Decimal(18, 2)), 59, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (54, 24, N'Ghế câu cá có tựa lưng', N'ghe-cau-ca-co-tua-lung', N'Ghế câu cá có tựa lưng - sản phẩm mẫu cho website FuuFishing.', N'Ghế câu cá có tựa lưng phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(690000.00 AS Decimal(18, 2)), CAST(621000.00 AS Decimal(18, 2)), 66, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (55, 24, N'Bộ giá đỡ cần câu 4 cần', N'bo-gia-do-can-cau-4-can', N'Bộ giá đỡ cần câu 4 cần - sản phẩm mẫu cho website FuuFishing.', N'Bộ giá đỡ cần câu 4 cần phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(495000.00 AS Decimal(18, 2)), CAST(495000.00 AS Decimal(18, 2)), 73, N'/Uploads/products/65960bca-47e1-4f46-8ed8-18731aac6a0c.jpeg', 1, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:44:58.1082656' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (56, 22, N'Hộp đựng mồi 2 mặt cỡ M', N'hop-dung-moi-2-mat-co-m', N'Hộp đựng mồi 2 mặt cỡ M - sản phẩm mẫu cho website FuuFishing.', N'Hộp đựng mồi 2 mặt cỡ M phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(145000.00 AS Decimal(18, 2)), CAST(138000.00 AS Decimal(18, 2)), 66, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (57, 22, N'Hộp đựng phụ kiện 4 ngăn', N'hop-dung-phu-kien-4-ngan', N'Hộp đựng phụ kiện 4 ngăn - sản phẩm mẫu cho website FuuFishing.', N'Hộp đựng phụ kiện 4 ngăn phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(95000.00 AS Decimal(18, 2)), CAST(86000.00 AS Decimal(18, 2)), 73, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (58, 22, N'Túi đeo chéo câu lure', N'tui-deo-cheo-cau-lure', N'Túi đeo chéo câu lure - sản phẩm mẫu cho website FuuFishing.', N'Túi đeo chéo câu lure phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(385000.00 AS Decimal(18, 2)), CAST(385000.00 AS Decimal(18, 2)), 80, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (59, 22, N'Túi đựng cần câu 1m2', N'tui-dung-can-cau-1m2', N'Túi đựng cần câu 1m2 - sản phẩm mẫu cho website FuuFishing.', N'Túi đựng cần câu 1m2 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(295000.00 AS Decimal(18, 2)), CAST(280000.00 AS Decimal(18, 2)), 87, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (60, 22, N'Thùng đựng đồ câu 36L', N'thung-dung-do-cau-36l', N'Thùng đựng đồ câu 36L - sản phẩm mẫu cho website FuuFishing.', N'Thùng đựng đồ câu 36L phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(680000.00 AS Decimal(18, 2)), CAST(612000.00 AS Decimal(18, 2)), 94, N'/Uploads/products/1472336f-dd5b-4d06-bf22-5bcf865bd059.webp', 1, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:45:29.9408471' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (61, 23, N'Kìm gỡ lưỡi câu inox 18cm', N'kim-go-luoi-cau-inox-18cm', N'Kìm gỡ lưỡi câu inox 18cm - sản phẩm mẫu cho website FuuFishing.', N'Kìm gỡ lưỡi câu inox 18cm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(165000.00 AS Decimal(18, 2)), CAST(152000.00 AS Decimal(18, 2)), 10, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (62, 23, N'Kìm cắt dây PE chuyên dụng', N'kim-cat-day-pe-chuyen-dung', N'Kìm cắt dây PE chuyên dụng - sản phẩm mẫu cho website FuuFishing.', N'Kìm cắt dây PE chuyên dụng phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(195000.00 AS Decimal(18, 2)), CAST(195000.00 AS Decimal(18, 2)), 17, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (63, 23, N'Kéo cắt mồi và dây câu', N'keo-cat-moi-va-day-cau', N'Kéo cắt mồi và dây câu - sản phẩm mẫu cho website FuuFishing.', N'Kéo cắt mồi và dây câu phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(125000.00 AS Decimal(18, 2)), CAST(112000.00 AS Decimal(18, 2)), 24, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (64, 23, N'Dụng cụ thắt nút câu cá', N'dung-cu-that-nut-cau-ca', N'Dụng cụ thắt nút câu cá - sản phẩm mẫu cho website FuuFishing.', N'Dụng cụ thắt nút câu cá phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(85000.00 AS Decimal(18, 2)), CAST(78000.00 AS Decimal(18, 2)), 31, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (65, 23, N'Bộ gỡ lưỡi câu 3 món', N'bo-go-luoi-cau-3-mon', N'Bộ gỡ lưỡi câu 3 món - sản phẩm mẫu cho website FuuFishing.', N'Bộ gỡ lưỡi câu 3 món phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(145000.00 AS Decimal(18, 2)), CAST(138000.00 AS Decimal(18, 2)), 38, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (66, 20, N'Lưỡi câu Chinu size 3 hộp 50', N'luoi-cau-chinu-size-3-hop-50', N'Lưỡi câu Chinu size 3 hộp 50 - sản phẩm mẫu cho website FuuFishing.', N'Lưỡi câu Chinu size 3 hộp 50 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(65000.00 AS Decimal(18, 2)), CAST(65000.00 AS Decimal(18, 2)), 87, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (67, 20, N'Lưỡi câu Iseama size 8 hộp 50', N'luoi-cau-iseama-size-8-hop-50', N'Lưỡi câu Iseama size 8 hộp 50 - sản phẩm mẫu cho website FuuFishing.', N'Lưỡi câu Iseama size 8 hộp 50 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(72000.00 AS Decimal(18, 2)), CAST(68000.00 AS Decimal(18, 2)), 94, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (68, 20, N'Lưỡi câu Circle Hook size 2/0', N'luoi-cau-circle-hook-size-2-0', N'Lưỡi câu Circle Hook size 2/0 - sản phẩm mẫu cho website FuuFishing.', N'Lưỡi câu Circle Hook size 2/0 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(85000.00 AS Decimal(18, 2)), CAST(76000.00 AS Decimal(18, 2)), 10, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (69, 20, N'Lưỡi câu Offset Worm size 3/0', N'luoi-cau-offset-worm-size-3-0', N'Lưỡi câu Offset Worm size 3/0 - sản phẩm mẫu cho website FuuFishing.', N'Lưỡi câu Offset Worm size 3/0 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(92000.00 AS Decimal(18, 2)), CAST(85000.00 AS Decimal(18, 2)), 17, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (70, 20, N'Lưỡi câu Treble Hook size 6', N'luoi-cau-treble-hook-size-6', N'Lưỡi câu Treble Hook size 6 - sản phẩm mẫu cho website FuuFishing.', N'Lưỡi câu Treble Hook size 6 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(98000.00 AS Decimal(18, 2)), CAST(98000.00 AS Decimal(18, 2)), 24, N'/Uploads/products/931ad425-1ae5-4c19-a721-e8b349d38e26.webp', 1, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:46:13.0052449' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (71, 13, N'Mồi cám cá chép hương bắp 500g', N'moi-cam-ca-chep-huong-bap-500g', N'Mồi cám cá chép hương bắp 500g - sản phẩm mẫu cho website FuuFishing.', N'Mồi cám cá chép hương bắp 500g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(55000.00 AS Decimal(18, 2)), CAST(52000.00 AS Decimal(18, 2)), 24, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (72, 13, N'Mồi cám cá chép hương sữa 500g', N'moi-cam-ca-chep-huong-sua-500g', N'Mồi cám cá chép hương sữa 500g - sản phẩm mẫu cho website FuuFishing.', N'Mồi cám cá chép hương sữa 500g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(58000.00 AS Decimal(18, 2)), CAST(52000.00 AS Decimal(18, 2)), 31, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (73, 13, N'Mồi viên cá chép tan chậm 300g', N'moi-vien-ca-chep-tan-cham-300g', N'Mồi viên cá chép tan chậm 300g - sản phẩm mẫu cho website FuuFishing.', N'Mồi viên cá chép tan chậm 300g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(65000.00 AS Decimal(18, 2)), CAST(60000.00 AS Decimal(18, 2)), 38, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (74, 13, N'Mồi bột cá chép tổng hợp 1kg', N'moi-bot-ca-chep-tong-hop-1kg', N'Mồi bột cá chép tổng hợp 1kg - sản phẩm mẫu cho website FuuFishing.', N'Mồi bột cá chép tổng hợp 1kg phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(89000.00 AS Decimal(18, 2)), CAST(89000.00 AS Decimal(18, 2)), 45, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (75, 13, N'Tinh chất dụ cá chép 100ml', N'tinh-chat-du-ca-chep-100ml', N'Tinh chất dụ cá chép 100ml - sản phẩm mẫu cho website FuuFishing.', N'Tinh chất dụ cá chép 100ml phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(72000.00 AS Decimal(18, 2)), CAST(65000.00 AS Decimal(18, 2)), 52, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (76, 14, N'Mồi cám rô phi vị tanh 500g', N'moi-cam-ro-phi-vi-tanh-500g', N'Mồi cám rô phi vị tanh 500g - sản phẩm mẫu cho website FuuFishing.', N'Mồi cám rô phi vị tanh 500g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(52000.00 AS Decimal(18, 2)), CAST(48000.00 AS Decimal(18, 2)), 59, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (77, 14, N'Mồi cám rô phi vị tôm 500g', N'moi-cam-ro-phi-vi-tom-500g', N'Mồi cám rô phi vị tôm 500g - sản phẩm mẫu cho website FuuFishing.', N'Mồi cám rô phi vị tôm 500g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(56000.00 AS Decimal(18, 2)), CAST(53000.00 AS Decimal(18, 2)), 66, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (78, 14, N'Mồi bột rô phi hồ dịch vụ 1kg', N'moi-bot-ro-phi-ho-dich-vu-1kg', N'Mồi bột rô phi hồ dịch vụ 1kg - sản phẩm mẫu cho website FuuFishing.', N'Mồi bột rô phi hồ dịch vụ 1kg phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(85000.00 AS Decimal(18, 2)), CAST(85000.00 AS Decimal(18, 2)), 73, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (79, 14, N'Mồi viên rô phi 300g', N'moi-vien-ro-phi-300g', N'Mồi viên rô phi 300g - sản phẩm mẫu cho website FuuFishing.', N'Mồi viên rô phi 300g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(62000.00 AS Decimal(18, 2)), CAST(57000.00 AS Decimal(18, 2)), 80, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (80, 14, N'Tinh chất dụ rô phi 100ml', N'tinh-chat-du-ro-phi-100ml', N'Tinh chất dụ rô phi 100ml - sản phẩm mẫu cho website FuuFishing.', N'Tinh chất dụ rô phi 100ml phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(68000.00 AS Decimal(18, 2)), CAST(65000.00 AS Decimal(18, 2)), 87, N'/Uploads/products/d8178114-76fd-4dac-8052-945bb1178e65.webp', 1, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-28T20:47:07.7375709' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (81, 10, N'Mồi Minnow Floating 90mm', N'moi-minnow-floating-90mm', N'Mồi Minnow Floating 90mm - sản phẩm mẫu cho website FuuFishing.', N'Mồi Minnow Floating 90mm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(125000.00 AS Decimal(18, 2)), CAST(119000.00 AS Decimal(18, 2)), 10, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (82, 10, N'Mồi Crankbait Deep Diver 70mm', N'moi-crankbait-deep-diver-70mm', N'Mồi Crankbait Deep Diver 70mm - sản phẩm mẫu cho website FuuFishing.', N'Mồi Crankbait Deep Diver 70mm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(145000.00 AS Decimal(18, 2)), CAST(130000.00 AS Decimal(18, 2)), 17, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (83, 10, N'Mồi Popper Topwater 80mm', N'moi-popper-topwater-80mm', N'Mồi Popper Topwater 80mm - sản phẩm mẫu cho website FuuFishing.', N'Mồi Popper Topwater 80mm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(135000.00 AS Decimal(18, 2)), CAST(135000.00 AS Decimal(18, 2)), 24, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (84, 10, N'Mồi Pencil Walking 95mm', N'moi-pencil-walking-95mm', N'Mồi Pencil Walking 95mm - sản phẩm mẫu cho website FuuFishing.', N'Mồi Pencil Walking 95mm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(155000.00 AS Decimal(18, 2)), CAST(147000.00 AS Decimal(18, 2)), 31, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (85, 10, N'Mồi Vibration Sinking 70mm', N'moi-vibration-sinking-70mm', N'Mồi Vibration Sinking 70mm - sản phẩm mẫu cho website FuuFishing.', N'Mồi Vibration Sinking 70mm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(165000.00 AS Decimal(18, 2)), CAST(148000.00 AS Decimal(18, 2)), 38, NULL, 1, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (86, 12, N'Mồi Jig kim loại 20g', N'moi-jig-kim-loai-20g', N'Mồi Jig kim loại 20g - sản phẩm mẫu cho website FuuFishing.', N'Mồi Jig kim loại 20g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(85000.00 AS Decimal(18, 2)), CAST(85000.00 AS Decimal(18, 2)), 80, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (87, 12, N'Mồi Jig kim loại 40g', N'moi-jig-kim-loai-40g', N'Mồi Jig kim loại 40g - sản phẩm mẫu cho website FuuFishing.', N'Mồi Jig kim loại 40g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(105000.00 AS Decimal(18, 2)), CAST(97000.00 AS Decimal(18, 2)), 87, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (88, 12, N'Mồi thìa Spoon 12g', N'moi-thia-spoon-12g', N'Mồi thìa Spoon 12g - sản phẩm mẫu cho website FuuFishing.', N'Mồi thìa Spoon 12g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(75000.00 AS Decimal(18, 2)), CAST(71000.00 AS Decimal(18, 2)), 94, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (89, 12, N'Mồi Spinner 15g', N'moi-spinner-15g', N'Mồi Spinner 15g - sản phẩm mẫu cho website FuuFishing.', N'Mồi Spinner 15g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(92000.00 AS Decimal(18, 2)), CAST(83000.00 AS Decimal(18, 2)), 10, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (90, 12, N'Mồi Blade Bait 18g', N'moi-blade-bait-18g', N'Mồi Blade Bait 18g - sản phẩm mẫu cho website FuuFishing.', N'Mồi Blade Bait 18g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(98000.00 AS Decimal(18, 2)), CAST(98000.00 AS Decimal(18, 2)), 17, NULL, 1, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (91, 11, N'Mồi mềm Paddle Tail 7cm', N'moi-mem-paddle-tail-7cm', N'Mồi mềm Paddle Tail 7cm - sản phẩm mẫu cho website FuuFishing.', N'Mồi mềm Paddle Tail 7cm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(65000.00 AS Decimal(18, 2)), CAST(60000.00 AS Decimal(18, 2)), 45, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (92, 11, N'Mồi mềm Worm 10cm', N'moi-mem-worm-10cm', N'Mồi mềm Worm 10cm - sản phẩm mẫu cho website FuuFishing.', N'Mồi mềm Worm 10cm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(72000.00 AS Decimal(18, 2)), CAST(72000.00 AS Decimal(18, 2)), 52, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (93, 11, N'Mồi mềm Craw 8cm', N'moi-mem-craw-8cm', N'Mồi mềm Craw 8cm - sản phẩm mẫu cho website FuuFishing.', N'Mồi mềm Craw 8cm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(78000.00 AS Decimal(18, 2)), CAST(70000.00 AS Decimal(18, 2)), 59, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (94, 11, N'Mồi mềm Frog 6cm', N'moi-mem-frog-6cm', N'Mồi mềm Frog 6cm - sản phẩm mẫu cho website FuuFishing.', N'Mồi mềm Frog 6cm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(85000.00 AS Decimal(18, 2)), CAST(78000.00 AS Decimal(18, 2)), 66, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (95, 11, N'Mồi mềm Shrimp 5cm', N'moi-mem-shrimp-5cm', N'Mồi mềm Shrimp 5cm - sản phẩm mẫu cho website FuuFishing.', N'Mồi mềm Shrimp 5cm phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(69000.00 AS Decimal(18, 2)), CAST(66000.00 AS Decimal(18, 2)), 73, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (96, 21, N'Phao câu đài Nano số 1', N'phao-cau-dai-nano-so-1', N'Phao câu đài Nano số 1 - sản phẩm mẫu cho website FuuFishing.', N'Phao câu đài Nano số 1 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(95000.00 AS Decimal(18, 2)), CAST(86000.00 AS Decimal(18, 2)), 31, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (97, 21, N'Phao câu đài Nano số 2', N'phao-cau-dai-nano-so-2', N'Phao câu đài Nano số 2 - sản phẩm mẫu cho website FuuFishing.', N'Phao câu đài Nano số 2 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(105000.00 AS Decimal(18, 2)), CAST(97000.00 AS Decimal(18, 2)), 38, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (98, 21, N'Phao điện câu đêm CR425', N'phao-dien-cau-dem-cr425', N'Phao điện câu đêm CR425 - sản phẩm mẫu cho website FuuFishing.', N'Phao điện câu đêm CR425 phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(185000.00 AS Decimal(18, 2)), CAST(176000.00 AS Decimal(18, 2)), 45, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (99, 21, N'Phao lục xa bờ 50g', N'phao-luc-xa-bo-50g', N'Phao lục xa bờ 50g - sản phẩm mẫu cho website FuuFishing.', N'Phao lục xa bờ 50g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(125000.00 AS Decimal(18, 2)), CAST(125000.00 AS Decimal(18, 2)), 52, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
GO
INSERT [dbo].[Products] ([ProductId], [CategoryId], [ProductName], [Slug], [ShortDescription], [Description], [Price], [SalePrice], [StockQuantity], [ThumbnailUrl], [IsFeatured], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (100, 21, N'Phao trượt câu sông 20g', N'phao-truot-cau-song-20g', N'Phao trượt câu sông 20g - sản phẩm mẫu cho website FuuFishing.', N'Phao trượt câu sông 20g phù hợp cho nhu cầu câu cá giải trí và bán chuyên. Sản phẩm seed phục vụ demo và kiểm thử hệ thống.', CAST(85000.00 AS Decimal(18, 2)), CAST(78000.00 AS Decimal(18, 2)), 59, NULL, 0, CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), CAST(N'2026-07-22T14:44:15.0149365' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
SET IDENTITY_INSERT [dbo].[UserAddresses] ON 

INSERT [dbo].[UserAddresses] ([Id], [UserId], [FullName], [PhoneNumber], [SpecificAddress], [IsDefault]) VALUES (1, N'0f74b24f-b6a3-47b9-a746-f24d71f3ac34', N'Kha Nguyen Huynh', N'0943265155', N'151, Luong Chi 
', 1)
INSERT [dbo].[UserAddresses] ([Id], [UserId], [FullName], [PhoneNumber], [SpecificAddress], [IsDefault]) VALUES (2, N'40800111-a0fe-43a6-a214-b4fd197ffc04', N'Kha Nguyen Huynh', N'0943265155', N'151, Lương chí', 0)
SET IDENTITY_INSERT [dbo].[UserAddresses] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CartItems_CartId_ProductId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_CartItems_CartId_ProductId] ON [dbo].[CartItems]
(
	[CartId] ASC,
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CartItems_ProductId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_CartItems_ProductId] ON [dbo].[CartItems]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Carts_UserId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_Carts_UserId] ON [dbo].[Carts]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Categories_CategoryId1]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_Categories_CategoryId1] ON [dbo].[Categories]
(
	[CategoryId1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Categories_ParentCategoryId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_Categories_ParentCategoryId] ON [dbo].[Categories]
(
	[ParentCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Categories_Slug]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Categories_Slug] ON [dbo].[Categories]
(
	[Slug] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChatAttachments_ChatMessageId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_ChatAttachments_ChatMessageId] ON [dbo].[ChatAttachments]
(
	[ChatMessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChatMessages_ConversationId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_ChatMessages_ConversationId] ON [dbo].[ChatMessages]
(
	[ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ChatMessages_Intent]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_ChatMessages_Intent] ON [dbo].[ChatMessages]
(
	[Intent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Conversations_UserId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_Conversations_UserId] ON [dbo].[Conversations]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Coupons_Code]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Coupons_Code] ON [dbo].[Coupons]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_OrderId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderId] ON [dbo].[OrderItems]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_ProductId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_ProductId] ON [dbo].[OrderItems]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_CouponId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_Orders_CouponId] ON [dbo].[Orders]
(
	[CouponId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Orders_UserId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_Orders_UserId] ON [dbo].[Orders]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_OrderId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Payments_OrderId] ON [dbo].[Payments]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ProductImages_ProductId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_ProductImages_ProductId] ON [dbo].[ProductImages]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Products_CategoryId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId] ON [dbo].[Products]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Products_Slug]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Products_Slug] ON [dbo].[Products]
(
	[Slug] ASC
)
WHERE ([Slug] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reviews_ProductId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_Reviews_ProductId] ON [dbo].[Reviews]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Reviews_UserId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_Reviews_UserId] ON [dbo].[Reviews]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserAddresses_UserId]    Script Date: 9/3/2026 7:25:30 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserAddresses_UserId] ON [dbo].[UserAddresses]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD  CONSTRAINT [FK_CartItems_Carts_CartId] FOREIGN KEY([CartId])
REFERENCES [dbo].[Carts] ([CartId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CartItems] CHECK CONSTRAINT [FK_CartItems_Carts_CartId]
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD  CONSTRAINT [FK_CartItems_Products_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[CartItems] CHECK CONSTRAINT [FK_CartItems_Products_ProductId]
GO
ALTER TABLE [dbo].[Carts]  WITH CHECK ADD  CONSTRAINT [FK_Carts_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Carts] CHECK CONSTRAINT [FK_Carts_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Categories_CategoryId1] FOREIGN KEY([CategoryId1])
REFERENCES [dbo].[Categories] ([CategoryId])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_Categories_CategoryId1]
GO
ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Categories_ParentCategoryId] FOREIGN KEY([ParentCategoryId])
REFERENCES [dbo].[Categories] ([CategoryId])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_Categories_ParentCategoryId]
GO
ALTER TABLE [dbo].[ChatAttachments]  WITH CHECK ADD  CONSTRAINT [FK_ChatAttachments_ChatMessages_ChatMessageId] FOREIGN KEY([ChatMessageId])
REFERENCES [dbo].[ChatMessages] ([ChatMessageId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ChatAttachments] CHECK CONSTRAINT [FK_ChatAttachments_ChatMessages_ChatMessageId]
GO
ALTER TABLE [dbo].[ChatMessages]  WITH CHECK ADD  CONSTRAINT [FK_ChatMessages_Conversations_ConversationId] FOREIGN KEY([ConversationId])
REFERENCES [dbo].[Conversations] ([ConversationId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ChatMessages] CHECK CONSTRAINT [FK_ChatMessages_Conversations_ConversationId]
GO
ALTER TABLE [dbo].[Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Conversations_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Conversations] CHECK CONSTRAINT [FK_Conversations_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItems_Orders_OrderId]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItems_Products_ProductId]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Coupons_CouponId] FOREIGN KEY([CouponId])
REFERENCES [dbo].[Coupons] ([CouponId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Coupons_CouponId]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Orders_OrderId] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Orders_OrderId]
GO
ALTER TABLE [dbo].[ProductImages]  WITH CHECK ADD  CONSTRAINT [FK_ProductImages_Products_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductImages] CHECK CONSTRAINT [FK_ProductImages_Products_ProductId]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([CategoryId])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Categories_CategoryId]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD  CONSTRAINT [FK_Reviews_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Reviews] CHECK CONSTRAINT [FK_Reviews_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD  CONSTRAINT [FK_Reviews_Products_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[Reviews] CHECK CONSTRAINT [FK_Reviews_Products_ProductId]
GO
ALTER TABLE [dbo].[UserAddresses]  WITH CHECK ADD  CONSTRAINT [FK_UserAddresses_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserAddresses] CHECK CONSTRAINT [FK_UserAddresses_AspNetUsers_UserId]
GO

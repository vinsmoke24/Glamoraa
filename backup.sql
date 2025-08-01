USE [master]
GO
/****** Object:  Database [Glamoraa]    Script Date: 05-06-2025 16:22:32 ******/
CREATE DATABASE [Glamoraa]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Glamoraa', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\Glamoraa.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Glamoraa_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\Glamoraa_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [Glamoraa] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Glamoraa].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Glamoraa] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Glamoraa] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Glamoraa] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Glamoraa] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Glamoraa] SET ARITHABORT OFF 
GO
ALTER DATABASE [Glamoraa] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [Glamoraa] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Glamoraa] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Glamoraa] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Glamoraa] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Glamoraa] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Glamoraa] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Glamoraa] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Glamoraa] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Glamoraa] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Glamoraa] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Glamoraa] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Glamoraa] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Glamoraa] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Glamoraa] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Glamoraa] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Glamoraa] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Glamoraa] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Glamoraa] SET  MULTI_USER 
GO
ALTER DATABASE [Glamoraa] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Glamoraa] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Glamoraa] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Glamoraa] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Glamoraa] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Glamoraa] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [Glamoraa] SET QUERY_STORE = ON
GO
ALTER DATABASE [Glamoraa] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [Glamoraa]
GO
/****** Object:  Table [dbo].[ActivityLogs]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivityLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[Action] [varchar](100) NULL,
	[Description] [nvarchar](255) NULL,
	[Timestamp] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Appointments]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Appointments](
	[AppointmentId] [uniqueidentifier] NOT NULL,
	[SalonId] [uniqueidentifier] NOT NULL,
	[BranchId] [uniqueidentifier] NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[ServiceId] [uniqueidentifier] NOT NULL,
	[SubServiceId] [int] NOT NULL,
	[StaffId] [uniqueidentifier] NULL,
	[PreferredDate] [date] NULL,
	[PreferredTime] [time](7) NULL,
	[SpecialRequests] [text] NULL,
	[FullAmount] [decimal](10, 2) NULL,
	[AdvancePaid] [decimal](10, 2) NULL,
	[CouponCode] [varchar](50) NULL,
	[BookingDate] [datetime] NULL,
	[Status] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[AppointmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppointmentServiceProducts]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppointmentServiceProducts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AppointmentServiceId] [int] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Quantity] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppointmentServices]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppointmentServices](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AppointmentId] [uniqueidentifier] NOT NULL,
	[ServiceId] [uniqueidentifier] NOT NULL,
	[AssignedStaffId] [uniqueidentifier] NULL,
	[PriceAtBooking] [decimal](10, 2) NULL,
	[EstimatedDurationMins] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Branches]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Branches](
	[BranchId] [uniqueidentifier] NOT NULL,
	[SalonId] [uniqueidentifier] NOT NULL,
	[BranchName] [nvarchar](150) NOT NULL,
	[Address] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BranchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Coupons]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Coupons](
	[CouponId] [int] IDENTITY(1,1) NOT NULL,
	[SalonId] [uniqueidentifier] NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Description] [text] NULL,
	[DiscountAmount] [decimal](10, 2) NULL,
	[IsPercentage] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CouponId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerId] [uniqueidentifier] NOT NULL,
	[SalonId] [uniqueidentifier] NOT NULL,
	[BranchId] [uniqueidentifier] NULL,
	[FullName] [nvarchar](255) NULL,
	[PhoneNumber] [varchar](20) NULL,
	[Email] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
	[Gender] [nvarchar](10) NULL,
	[IsMember] [bit] NULL,
	[LastVisitDate] [datetime] NULL,
	[AmountSpent] [decimal](10, 2) NULL,
	[Status] [nvarchar](50) NULL,
	[PasswordHash] [varchar](255) NULL,
	[LoyaltyPoints] [int] NULL,
	[MembershipId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[PhoneNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DailyCashLogs]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DailyCashLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SalonId] [uniqueidentifier] NOT NULL,
	[BranchId] [uniqueidentifier] NULL,
	[Date] [date] NOT NULL,
	[OpeningCash] [decimal](10, 2) NULL,
	[ClosingCash] [decimal](10, 2) NULL,
	[Notes] [nvarchar](255) NULL,
	[RecordedBy] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[FeedbackId] [int] IDENTITY(1,1) NOT NULL,
	[AppointmentId] [uniqueidentifier] NULL,
	[StaffId] [uniqueidentifier] NULL,
	[CustomerId] [uniqueidentifier] NULL,
	[Rating] [int] NULL,
	[Comments] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[FeedbackId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoices]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoices](
	[InvoiceId] [uniqueidentifier] NOT NULL,
	[SalonId] [uniqueidentifier] NOT NULL,
	[BranchId] [uniqueidentifier] NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[AppointmentId] [uniqueidentifier] NULL,
	[InvoiceDate] [datetime] NULL,
	[Subtotal] [decimal](10, 2) NULL,
	[Discount] [decimal](10, 2) NULL,
	[TaxAmount] [decimal](10, 2) NULL,
	[TotalAmount] [decimal](10, 2) NULL,
	[PaymentStatus] [varchar](20) NULL,
	[PaymentMethod] [varchar](20) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Memberships]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Memberships](
	[MembershipId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[DiscountPercent] [decimal](5, 2) NULL,
	[ValidFrom] [date] NULL,
	[ValidTo] [date] NULL,
	[Price] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[MembershipId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[PaymentId] [int] IDENTITY(1,1) NOT NULL,
	[SalonId] [uniqueidentifier] NOT NULL,
	[BranchId] [uniqueidentifier] NULL,
	[AppointmentId] [uniqueidentifier] NULL,
	[AmountPaid] [decimal](10, 2) NULL,
	[PaymentMode] [varchar](20) NULL,
	[TokenAdvance] [bit] NULL,
	[PaidOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [uniqueidentifier] NOT NULL,
	[SalonId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Category] [nvarchar](100) NULL,
	[Type] [varchar](50) NULL,
	[Unit] [nvarchar](50) NULL,
	[Price] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductStock]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductStock](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[Operation] [varchar](50) NULL,
	[ReferenceId] [uniqueidentifier] NULL,
	[Notes] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalonOwners]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalonOwners](
	[OwnerId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](150) NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[PhoneNumber] [varchar](20) NOT NULL,
	[Gender] [varchar](10) NOT NULL,
	[DateOfBirth] [date] NOT NULL,
	[ProfilePictureUrl] [nvarchar](250) NULL,
	[GSTNumber] [varchar](50) NULL,
	[OpeningTime] [time](7) NOT NULL,
	[ClosingTime] [time](7) NOT NULL,
	[PasswordHash] [nvarchar](250) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[TermsAccepted] [bit] NOT NULL,
	[SubscribeToUpdates] [bit] NOT NULL,
	[SalonName] [varchar](200) NULL,
	[SalonType] [varchar](50) NULL,
	[ServicesOffered] [varchar](500) NULL,
	[StreetAddress] [varchar](300) NULL,
	[City] [varchar](100) NULL,
	[Country] [varchar](100) NULL,
	[State] [varchar](100) NULL,
	[PinCode] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[OwnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Salons]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Salons](
	[SalonId] [uniqueidentifier] NOT NULL,
	[OwnerId] [uniqueidentifier] NULL,
	[SalonName] [nvarchar](150) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SalonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Services]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Services](
	[ServiceId] [uniqueidentifier] NOT NULL,
	[SalonId] [uniqueidentifier] NOT NULL,
	[ServiceName] [varchar](100) NOT NULL,
	[Description] [text] NULL,
	[Status] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ServiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Staff]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Staff](
	[StaffId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[SalonId] [uniqueidentifier] NOT NULL,
	[BranchId] [uniqueidentifier] NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[PhoneNumber] [varchar](15) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[ProfilePictureUrl] [nvarchar](255) NULL,
	[Specializations] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[DateOfJoining] [date] NULL,
	[PasswordHash] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[StaffId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[PhoneNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StaffAvailabilityOverrides]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StaffAvailabilityOverrides](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StaffId] [uniqueidentifier] NULL,
	[Date] [date] NOT NULL,
	[IsAvailable] [bit] NOT NULL,
	[Notes] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StaffServices]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StaffServices](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StaffId] [uniqueidentifier] NULL,
	[ServiceId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StaffWorkingHours]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StaffWorkingHours](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StaffId] [uniqueidentifier] NOT NULL,
	[DayOfWeek] [varchar](10) NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubServices]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubServices](
	[SubServiceId] [int] IDENTITY(1,1) NOT NULL,
	[ServiceId] [uniqueidentifier] NOT NULL,
	[SubServiceName] [varchar](100) NOT NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[DurationMinutes] [int] NULL,
	[Description] [text] NULL,
	[Status] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[SubServiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 05-06-2025 16:22:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [uniqueidentifier] NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[PasswordHash] [varchar](255) NOT NULL,
	[RoleId] [int] NOT NULL,
	[SalonId] [uniqueidentifier] NULL,
	[BranchId] [uniqueidentifier] NULL,
	[LastLogin] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ActivityLogs] ADD  DEFAULT (getdate()) FOR [Timestamp]
GO
ALTER TABLE [dbo].[Appointments] ADD  DEFAULT (newid()) FOR [AppointmentId]
GO
ALTER TABLE [dbo].[Appointments] ADD  DEFAULT (getdate()) FOR [BookingDate]
GO
ALTER TABLE [dbo].[Appointments] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[Branches] ADD  DEFAULT (newid()) FOR [BranchId]
GO
ALTER TABLE [dbo].[Branches] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Branches] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Coupons] ADD  DEFAULT ((0)) FOR [IsPercentage]
GO
ALTER TABLE [dbo].[Coupons] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Customers] ADD  DEFAULT (newid()) FOR [CustomerId]
GO
ALTER TABLE [dbo].[Feedback] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT (newid()) FOR [InvoiceId]
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT (getdate()) FOR [InvoiceDate]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT (getdate()) FOR [PaidOn]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (newid()) FOR [ProductId]
GO
ALTER TABLE [dbo].[ProductStock] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[SalonOwners] ADD  DEFAULT (newid()) FOR [OwnerId]
GO
ALTER TABLE [dbo].[SalonOwners] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SalonOwners] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SalonOwners] ADD  DEFAULT ((0)) FOR [SubscribeToUpdates]
GO
ALTER TABLE [dbo].[Salons] ADD  DEFAULT (newid()) FOR [SalonId]
GO
ALTER TABLE [dbo].[Salons] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Salons] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Services] ADD  DEFAULT (newid()) FOR [ServiceId]
GO
ALTER TABLE [dbo].[Services] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Staff] ADD  DEFAULT (newid()) FOR [StaffId]
GO
ALTER TABLE [dbo].[Staff] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SubServices] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (newid()) FOR [UserId]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ActivityLogs]  WITH CHECK ADD  CONSTRAINT [FK_ActivityLogs_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ActivityLogs] CHECK CONSTRAINT [FK_ActivityLogs_Users]
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [FK_Appointments_Branches] FOREIGN KEY([BranchId])
REFERENCES [dbo].[Branches] ([BranchId])
GO
ALTER TABLE [dbo].[Appointments] CHECK CONSTRAINT [FK_Appointments_Branches]
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [FK_Appointments_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[Appointments] CHECK CONSTRAINT [FK_Appointments_Customers]
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [FK_Appointments_Salons] FOREIGN KEY([SalonId])
REFERENCES [dbo].[Salons] ([SalonId])
GO
ALTER TABLE [dbo].[Appointments] CHECK CONSTRAINT [FK_Appointments_Salons]
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [FK_Appointments_Services] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Services] ([ServiceId])
GO
ALTER TABLE [dbo].[Appointments] CHECK CONSTRAINT [FK_Appointments_Services]
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [FK_Appointments_Staff] FOREIGN KEY([StaffId])
REFERENCES [dbo].[Staff] ([StaffId])
GO
ALTER TABLE [dbo].[Appointments] CHECK CONSTRAINT [FK_Appointments_Staff]
GO
ALTER TABLE [dbo].[Appointments]  WITH CHECK ADD  CONSTRAINT [FK_Appointments_SubServices] FOREIGN KEY([SubServiceId])
REFERENCES [dbo].[SubServices] ([SubServiceId])
GO
ALTER TABLE [dbo].[Appointments] CHECK CONSTRAINT [FK_Appointments_SubServices]
GO
ALTER TABLE [dbo].[AppointmentServiceProducts]  WITH CHECK ADD  CONSTRAINT [FK_AppointmentServiceProducts_AppointmentServices] FOREIGN KEY([AppointmentServiceId])
REFERENCES [dbo].[AppointmentServices] ([Id])
GO
ALTER TABLE [dbo].[AppointmentServiceProducts] CHECK CONSTRAINT [FK_AppointmentServiceProducts_AppointmentServices]
GO
ALTER TABLE [dbo].[AppointmentServiceProducts]  WITH CHECK ADD  CONSTRAINT [FK_AppointmentServiceProducts_Products] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[AppointmentServiceProducts] CHECK CONSTRAINT [FK_AppointmentServiceProducts_Products]
GO
ALTER TABLE [dbo].[AppointmentServices]  WITH CHECK ADD  CONSTRAINT [FK_AppointmentServices_Appointments] FOREIGN KEY([AppointmentId])
REFERENCES [dbo].[Appointments] ([AppointmentId])
GO
ALTER TABLE [dbo].[AppointmentServices] CHECK CONSTRAINT [FK_AppointmentServices_Appointments]
GO
ALTER TABLE [dbo].[AppointmentServices]  WITH CHECK ADD  CONSTRAINT [FK_AppointmentServices_Services] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Services] ([ServiceId])
GO
ALTER TABLE [dbo].[AppointmentServices] CHECK CONSTRAINT [FK_AppointmentServices_Services]
GO
ALTER TABLE [dbo].[AppointmentServices]  WITH CHECK ADD  CONSTRAINT [FK_AppointmentServices_Staff] FOREIGN KEY([AssignedStaffId])
REFERENCES [dbo].[Staff] ([StaffId])
GO
ALTER TABLE [dbo].[AppointmentServices] CHECK CONSTRAINT [FK_AppointmentServices_Staff]
GO
ALTER TABLE [dbo].[Branches]  WITH CHECK ADD  CONSTRAINT [FK_Branches_Salons] FOREIGN KEY([SalonId])
REFERENCES [dbo].[Salons] ([SalonId])
GO
ALTER TABLE [dbo].[Branches] CHECK CONSTRAINT [FK_Branches_Salons]
GO
ALTER TABLE [dbo].[Coupons]  WITH CHECK ADD  CONSTRAINT [FK_Coupons_Salons] FOREIGN KEY([SalonId])
REFERENCES [dbo].[Salons] ([SalonId])
GO
ALTER TABLE [dbo].[Coupons] CHECK CONSTRAINT [FK_Coupons_Salons]
GO
ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_Branches] FOREIGN KEY([BranchId])
REFERENCES [dbo].[Branches] ([BranchId])
GO
ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_Branches]
GO
ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_Memberships] FOREIGN KEY([MembershipId])
REFERENCES [dbo].[Memberships] ([MembershipId])
GO
ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_Memberships]
GO
ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_Salons] FOREIGN KEY([SalonId])
REFERENCES [dbo].[Salons] ([SalonId])
GO
ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_Salons]
GO
ALTER TABLE [dbo].[DailyCashLogs]  WITH CHECK ADD  CONSTRAINT [FK_DailyCashLogs_Branches] FOREIGN KEY([BranchId])
REFERENCES [dbo].[Branches] ([BranchId])
GO
ALTER TABLE [dbo].[DailyCashLogs] CHECK CONSTRAINT [FK_DailyCashLogs_Branches]
GO
ALTER TABLE [dbo].[DailyCashLogs]  WITH CHECK ADD  CONSTRAINT [FK_DailyCashLogs_Salons] FOREIGN KEY([SalonId])
REFERENCES [dbo].[Salons] ([SalonId])
GO
ALTER TABLE [dbo].[DailyCashLogs] CHECK CONSTRAINT [FK_DailyCashLogs_Salons]
GO
ALTER TABLE [dbo].[DailyCashLogs]  WITH CHECK ADD  CONSTRAINT [FK_DailyCashLogs_Users] FOREIGN KEY([RecordedBy])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DailyCashLogs] CHECK CONSTRAINT [FK_DailyCashLogs_Users]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Appointments] FOREIGN KEY([AppointmentId])
REFERENCES [dbo].[Appointments] ([AppointmentId])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK_Feedback_Appointments]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK_Feedback_Customers]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Staff] FOREIGN KEY([StaffId])
REFERENCES [dbo].[Staff] ([StaffId])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK_Feedback_Staff]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_Appointments] FOREIGN KEY([AppointmentId])
REFERENCES [dbo].[Appointments] ([AppointmentId])
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_Appointments]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_Branches] FOREIGN KEY([BranchId])
REFERENCES [dbo].[Branches] ([BranchId])
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_Branches]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_Customers]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_Salons] FOREIGN KEY([SalonId])
REFERENCES [dbo].[Salons] ([SalonId])
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_Salons]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_Users] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_Users]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Appointments] FOREIGN KEY([AppointmentId])
REFERENCES [dbo].[Appointments] ([AppointmentId])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Appointments]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Branches] FOREIGN KEY([BranchId])
REFERENCES [dbo].[Branches] ([BranchId])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Branches]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Salons] FOREIGN KEY([SalonId])
REFERENCES [dbo].[Salons] ([SalonId])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Salons]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Salons] FOREIGN KEY([SalonId])
REFERENCES [dbo].[Salons] ([SalonId])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Salons]
GO
ALTER TABLE [dbo].[ProductStock]  WITH CHECK ADD  CONSTRAINT [FK_ProductStock_Products] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[ProductStock] CHECK CONSTRAINT [FK_ProductStock_Products]
GO
ALTER TABLE [dbo].[SalonOwners]  WITH CHECK ADD  CONSTRAINT [FK_SalonOwners_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[SalonOwners] CHECK CONSTRAINT [FK_SalonOwners_Users]
GO
ALTER TABLE [dbo].[Salons]  WITH CHECK ADD  CONSTRAINT [FK_Salons_SalonOwners] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[SalonOwners] ([OwnerId])
GO
ALTER TABLE [dbo].[Salons] CHECK CONSTRAINT [FK_Salons_SalonOwners]
GO
ALTER TABLE [dbo].[Services]  WITH CHECK ADD  CONSTRAINT [FK_Services_Salons] FOREIGN KEY([SalonId])
REFERENCES [dbo].[Salons] ([SalonId])
GO
ALTER TABLE [dbo].[Services] CHECK CONSTRAINT [FK_Services_Salons]
GO
ALTER TABLE [dbo].[Staff]  WITH CHECK ADD  CONSTRAINT [FK_Staff_Branches] FOREIGN KEY([BranchId])
REFERENCES [dbo].[Branches] ([BranchId])
GO
ALTER TABLE [dbo].[Staff] CHECK CONSTRAINT [FK_Staff_Branches]
GO
ALTER TABLE [dbo].[Staff]  WITH CHECK ADD  CONSTRAINT [FK_Staff_Salons] FOREIGN KEY([SalonId])
REFERENCES [dbo].[Salons] ([SalonId])
GO
ALTER TABLE [dbo].[Staff] CHECK CONSTRAINT [FK_Staff_Salons]
GO
ALTER TABLE [dbo].[Staff]  WITH CHECK ADD  CONSTRAINT [FK_Staff_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Staff] CHECK CONSTRAINT [FK_Staff_Users]
GO
ALTER TABLE [dbo].[StaffAvailabilityOverrides]  WITH CHECK ADD  CONSTRAINT [FK_StaffAvailabilityOverrides_Staff] FOREIGN KEY([StaffId])
REFERENCES [dbo].[Staff] ([StaffId])
GO
ALTER TABLE [dbo].[StaffAvailabilityOverrides] CHECK CONSTRAINT [FK_StaffAvailabilityOverrides_Staff]
GO
ALTER TABLE [dbo].[StaffServices]  WITH CHECK ADD  CONSTRAINT [FK_StaffServices_Services] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Services] ([ServiceId])
GO
ALTER TABLE [dbo].[StaffServices] CHECK CONSTRAINT [FK_StaffServices_Services]
GO
ALTER TABLE [dbo].[StaffServices]  WITH CHECK ADD  CONSTRAINT [FK_StaffServices_Staff] FOREIGN KEY([StaffId])
REFERENCES [dbo].[Staff] ([StaffId])
GO
ALTER TABLE [dbo].[StaffServices] CHECK CONSTRAINT [FK_StaffServices_Staff]
GO
ALTER TABLE [dbo].[StaffWorkingHours]  WITH CHECK ADD  CONSTRAINT [FK_StaffWorkingHours_Staff] FOREIGN KEY([StaffId])
REFERENCES [dbo].[Staff] ([StaffId])
GO
ALTER TABLE [dbo].[StaffWorkingHours] CHECK CONSTRAINT [FK_StaffWorkingHours_Staff]
GO
ALTER TABLE [dbo].[SubServices]  WITH CHECK ADD  CONSTRAINT [FK_SubServices_Services] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Services] ([ServiceId])
GO
ALTER TABLE [dbo].[SubServices] CHECK CONSTRAINT [FK_SubServices_Services]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Branches] FOREIGN KEY([BranchId])
REFERENCES [dbo].[Branches] ([BranchId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Branches]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Salons] FOREIGN KEY([SalonId])
REFERENCES [dbo].[Salons] ([SalonId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Salons]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD CHECK  (([Rating]>=(1) AND [Rating]<=(5)))
GO
USE [master]
GO
ALTER DATABASE [Glamoraa] SET  READ_WRITE 
GO

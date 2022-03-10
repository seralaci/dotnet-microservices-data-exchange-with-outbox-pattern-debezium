USE [master]
GO

/* Create TrueAtomicMicroservices-Db database */

CREATE DATABASE [TrueAtomicMicroservices-Db]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TrueAtomicMicroservices-Db', FILENAME = N'/var/opt/mssql/data/TrueAtomicMicroservices-Db.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TrueAtomicMicroservices-Db_log', FILENAME = N'/var/opt/mssql/data/TrueAtomicMicroservices-Db_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET COMPATIBILITY_LEVEL = 150
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET ARITHABORT OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET RECOVERY FULL 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET  MULTI_USER 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [TrueAtomicMicroservices-Db] SET QUERY_STORE = OFF
GO

USE [TrueAtomicMicroservices-Db]
GO

/* Create Orders table */

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](max) NOT NULL,
	[LastName] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[OrderDate] [datetime2](7) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/* Create OutboxEvents table */

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OutboxEvents](
	[Id] [uniqueidentifier] NOT NULL,
	[AggregateType] [nvarchar](max) NOT NULL,
	[AggregateId] [uniqueidentifier] NOT NULL,
	[Type] [nvarchar](max) NOT NULL,
	[Payload] [nvarchar](max) NOT NULL,
	[DateOccurred] [datetime2](7) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[OutboxEvents] ADD  CONSTRAINT [PK_OutboxEvents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/* Enables change data capture */

EXEC sys.sp_cdc_enable_db
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'dbo',
    @source_name   = N'OutboxEvents',
    @role_name     = N'Admin',
    @supports_net_changes = 1
GO


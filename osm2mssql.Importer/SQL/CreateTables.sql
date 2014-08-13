use [OSM];

CREATE TABLE [tWayCreation]	(
	wayId bigint NOT NULL,
	nodeId bigint NOT NULL,
	sort int NOT NULL
);

CREATE TABLE [tWay] (
	[Id] bigint NOT NULL,
	[line] [geography] NULL
);

CREATE TABLE [tWayTag] (
	[WayId] bigint NOT NULL,
	[Typ] [int] NOT NULL,
	[Info] [nvarchar](max) NOT NULL
);

CREATE TABLE [tNode](
	[Id] bigint NOT NULL,
	[location] [geography] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
);

CREATE TABLE [tNodeTag](
	[NodeId] bigint NOT NULL,
	[Typ] [int] NOT NULL,
	[Info] [nvarchar](1000) NOT NULL
);

create table [tTagType] (
	[Typ] [int] not null,
	[Name] nvarchar(255)		
)

CREATE TABLE [tRelationCreation]	(
	RelationId bigint NOT NULL,
	[ref] bigint NOT NULL,
	[type] int not null,
	[role] int not null,
	sort int NOT NULL,
);

CREATE TABLE [tRelation] (
	[id] bigint NOT NULL,
	[geo] [geography] NULL	 ,	
	[role] int not null,
);

CREATE TABLE [tRelationTag] (
	[RelationId] bigint NOT NULL,
	[Typ] [int] NOT NULL,
	[Info] [nvarchar](max) NOT NULL
);

create table [tMemberType] (
	[id] [int] not null constraint PK_tMemberType_Id primary key clustered,
	[Name] nvarchar(255)		
)

create table [tMemberRole] (
	[id] [int] not null constraint PK_tMemberRole_Id primary key clustered,
	[Name] nvarchar(255)		
)

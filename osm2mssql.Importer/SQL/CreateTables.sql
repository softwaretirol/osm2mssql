use [OSM];

CREATE TABLE [WayCreation]	(
	wayId bigint NOT NULL,
	nodeId bigint NOT NULL,
	sort int NOT NULL
);

CREATE TABLE [Way] (
	[Id] bigint NOT NULL,
	[line] [geography] NULL
);

CREATE TABLE [WayTag] (
	[WayId] bigint NOT NULL,
	[Typ] [int] NOT NULL,
	[Info] [nvarchar](max) NOT NULL
);

CREATE TABLE [Node](
	[Id] bigint NOT NULL,
	[location] [geography] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
);

CREATE TABLE [NodeTag](
	[NodeId] bigint NOT NULL,
	[Typ] [int] NOT NULL,
	[Info] [nvarchar](1000) NOT NULL
);

create table [TagType] (
	[Typ] [int] not null,
	[Name] nvarchar(255)		
)

CREATE TABLE [RelationCreation]	(
	RelationId bigint NOT NULL,
	[ref] bigint NOT NULL,
	[type] int not null,
	[role] int not null,
	sort int NOT NULL,
);

CREATE TABLE [Relation] (
	[id] bigint NOT NULL,
	[geo] [geography] NULL	 ,	
	[role] int not null,
);

CREATE TABLE [RelationTag] (
	[RelationId] bigint NOT NULL,
	[Typ] [int] NOT NULL,
	[Info] [nvarchar](max) NOT NULL
);

create table [MemberType] (
	[id] [int] not null constraint PK_MemberType_Id primary key clustered,
	[Name] nvarchar(255)		
)

create table [MemberRole] (
	[id] [int] not null constraint PK_MemberRole_Id primary key clustered,
	[Name] nvarchar(255)		
)

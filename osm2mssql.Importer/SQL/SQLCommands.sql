
create schema info;
GO
CREATE NONCLUSTERED INDEX idxTagType ON [TagType] ([Name])
CREATE NONCLUSTERED INDEX idxWayTagTyp ON [WayTag] ([Typ]) INCLUDE ([WayId],[Info])
CREATE NONCLUSTERED INDEX idxRelationTagTyp ON [RelationTag] ([Typ]) INCLUDE ([RelationId],[Info])
CREATE NONCLUSTERED INDEX idxNodeTagTyp ON [dbo].[NodeTag] ([Typ]) INCLUDE ([NodeId],[Info])
GO

---- INFO ADMINLEVELS CREATION
--SELECT * INTO info.AdminLevels FROM
--(
--SELECT       Relation.id as RelationId, AdminLevel = CAST(RelationTag.Info as int), Geo, RelationTag1.Info as Name, RelationTag2.Info as Place, RelationTag3.Info as PostalCode
--FROM            Relation LEFT JOIN
--				RelationTag ON Relation.id = RelationTag.RelationId INNER JOIN
--				TagType ON RelationTag.Typ = TagType.Typ and RelationTag.Typ = (SELECT TOP(1) Typ FROM TagType WHERE name like 'admin_level') LEFT JOIN
--				RelationTag AS RelationTag1 ON Relation.Id = RelationTag1.RelationId and RelationTag1.Typ =  (SELECT TOP(1) Typ FROM TagType WHERE name like 'name') LEFT JOIN
--				RelationTag AS RelationTag2 ON Relation.Id = RelationTag2.RelationId and RelationTag2.Typ = (SELECT TOP(1) Typ FROM TagType WHERE name like 'place')LEFT JOIN
--				RelationTag AS RelationTag3 ON Relation.Id = RelationTag3.RelationId and RelationTag3.Typ = (SELECT TOP(1) Typ FROM TagType WHERE name like 'postal_code')
--where Relation.geo is not null
--) x

--GO
--ALTER TABLE info.AdminLevels ADD CONSTRAINT
--PK_AdminLevels PRIMARY KEY CLUSTERED (RelationId)
--GO
--CREATE NONCLUSTERED INDEX idx_AdminLevels ON [info].[AdminLevels]
--(AdminLevel ASC)
--GO

--IF(@@VERSION like '%Server 2008%')
--	CREATE SPATIAL INDEX [idx_AdminLevelsSpatial] ON [info].[AdminLevels] ([Geo])
--else
--	CREATE SPATIAL INDEX [idx_AdminLevelsSpatial] ON [info].[AdminLevels] ([Geo]) USING  GEOGRAPHY_AUTO_GRID 

--GO

---- INFO ROAD CREATION
--SELECT * INTO info.Roads FROM
--(SELECT       Way.Id, Way.Line as Street,WayTag.Info as HighWayType, WayTag1.Info as Name, WayTag2.Info as MaxSpeed
--FROM            Way LEFT JOIN
--				WayTag ON WayTag.WayId = Way.Id INNER JOIN
--				TagType ON WayTag.Typ = TagType.Typ and WayTag.Typ = (SELECT TOP(1) Typ FROM TagType WHERE name like 'highway') LEFT JOIN
--				WayTag AS WayTag1 ON Way.Id = WayTag1.WayId and WayTag1.Typ =  (SELECT TOP(1) Typ FROM TagType WHERE name like 'name') LEFT JOIN
--				WayTag AS WayTag2 ON Way.Id = WayTag2.WayId and WayTag2.Typ = (SELECT TOP(1) Typ FROM TagType WHERE name like 'maxspeed')
--where Way.line is not null) x
--GO

--ALTER TABLE info.Roads ADD CONSTRAINT PK_Roads PRIMARY KEY CLUSTERED (Id)

--IF(@@VERSION like '%Server 2008%')
--	CREATE SPATIAL INDEX [idxInfoRoad] ON [info].[Roads] ([Street])
--ELSE
--	CREATE SPATIAL INDEX [idxInfoRoad] ON [info].[Roads] ([Street]) USING  GEOGRAPHY_AUTO_GRID 

---- INFO CITIES CREATION
--SELECT * INTO info.Cities FROM (
--SELECT      Node.Id, Latitude, Longitude, Node.location,  NodeTag.Info as Name, NodeTag2.Info as Place
--FROM            Node LEFT JOIN
--				NodeTag ON Node.id = NodeTag.NodeId INNER JOIN
--				TagType ON NodeTag.Typ = TagType.Typ and NodeTag.Typ = (SELECT TOP(1) Typ FROM TagType WHERE name = 'name') JOIN
--				NodeTag AS NodeTag2 ON Node.Id = NodeTag2.NodeId and NodeTag2.Typ = (SELECT TOP(1) Typ FROM TagType WHERE name like 'place') 
--where Node.location is not null) x


--ALTER TABLE info.Cities ADD CONSTRAINT PK_Cities PRIMARY KEY CLUSTERED (Id)

--IF(@@VERSION like '%Server 2008%')
--	CREATE SPATIAL INDEX [idxCities] ON [info].Cities (location)
--ELSE
--	CREATE SPATIAL INDEX [idxCities] ON [info].Cities (location) USING  GEOGRAPHY_AUTO_GRID 


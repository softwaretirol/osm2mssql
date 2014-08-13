
create schema info;
GO
CREATE NONCLUSTERED INDEX idxTagType ON [tTagType] ([Name])
CREATE NONCLUSTERED INDEX idxtWayTagTyp ON [tWayTag] ([Typ]) INCLUDE ([WayId],[Info])
CREATE NONCLUSTERED INDEX idxtRelationTagTyp ON [tRelationTag] ([Typ]) INCLUDE ([RelationId],[Info])
CREATE NONCLUSTERED INDEX idxtNodeTagTyp ON [dbo].[tNodeTag] ([Typ]) INCLUDE ([NodeId],[Info])
GO

---- INFO ADMINLEVELS CREATION
--SELECT * INTO info.AdminLevels FROM
--(
--SELECT       tRelation.id as RelationId, AdminLevel = CAST(tRelationTag.Info as int), Geo, tRelationTag1.Info as Name, tRelationTag2.Info as Place, tRelationTag3.Info as PostalCode
--FROM            tRelation LEFT JOIN
--				tRelationTag ON tRelation.id = tRelationTag.RelationId INNER JOIN
--				tTagType ON tRelationTag.Typ = tTagType.Typ and tRelationTag.Typ = (SELECT TOP(1) Typ FROM tTagType WHERE name like 'admin_level') LEFT JOIN
--				tRelationTag AS tRelationTag1 ON tRelation.Id = tRelationTag1.RelationId and tRelationTag1.Typ =  (SELECT TOP(1) Typ FROM tTagType WHERE name like 'name') LEFT JOIN
--				tRelationTag AS tRelationTag2 ON tRelation.Id = tRelationTag2.RelationId and tRelationTag2.Typ = (SELECT TOP(1) Typ FROM tTagType WHERE name like 'place')LEFT JOIN
--				tRelationTag AS tRelationTag3 ON tRelation.Id = tRelationTag3.RelationId and tRelationTag3.Typ = (SELECT TOP(1) Typ FROM tTagType WHERE name like 'postal_code')
--where tRelation.geo is not null
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
--(SELECT       tWay.Id, tWay.Line as Street,tWayTag.Info as HighWayType, tWayTag1.Info as Name, tWayTag2.Info as MaxSpeed
--FROM            tWay LEFT JOIN
--				tWayTag ON tWayTag.WayId = tWay.Id INNER JOIN
--				tTagType ON tWayTag.Typ = tTagType.Typ and tWayTag.Typ = (SELECT TOP(1) Typ FROM tTagType WHERE name like 'highway') LEFT JOIN
--				tWayTag AS tWayTag1 ON tWay.Id = tWayTag1.WayId and tWayTag1.Typ =  (SELECT TOP(1) Typ FROM tTagType WHERE name like 'name') LEFT JOIN
--				tWayTag AS tWayTag2 ON tWay.Id = tWayTag2.WayId and tWayTag2.Typ = (SELECT TOP(1) Typ FROM tTagType WHERE name like 'maxspeed')
--where tWay.line is not null) x
--GO

--ALTER TABLE info.Roads ADD CONSTRAINT PK_Roads PRIMARY KEY CLUSTERED (Id)

--IF(@@VERSION like '%Server 2008%')
--	CREATE SPATIAL INDEX [idxInfoRoad] ON [info].[Roads] ([Street])
--ELSE
--	CREATE SPATIAL INDEX [idxInfoRoad] ON [info].[Roads] ([Street]) USING  GEOGRAPHY_AUTO_GRID 

---- INFO CITIES CREATION
--SELECT * INTO info.Cities FROM (
--SELECT      tNode.Id, Latitude, Longitude, tNode.location,  tNodeTag.Info as Name, tNodeTag2.Info as Place
--FROM            tNode LEFT JOIN
--				tNodeTag ON tNode.id = tNodeTag.NodeId INNER JOIN
--				tTagType ON tNodeTag.Typ = tTagType.Typ and tNodeTag.Typ = (SELECT TOP(1) Typ FROM tTagType WHERE name = 'name') JOIN
--				tNodeTag AS tNodeTag2 ON tNode.Id = tNodeTag2.NodeId and tNodeTag2.Typ = (SELECT TOP(1) Typ FROM tTagType WHERE name like 'place') 
--where tNode.location is not null) x


--ALTER TABLE info.Cities ADD CONSTRAINT PK_Cities PRIMARY KEY CLUSTERED (Id)

--IF(@@VERSION like '%Server 2008%')
--	CREATE SPATIAL INDEX [idxCities] ON [info].Cities (location)
--ELSE
--	CREATE SPATIAL INDEX [idxCities] ON [info].Cities (location) USING  GEOGRAPHY_AUTO_GRID 


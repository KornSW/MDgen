# FooAPI
this is a summary with [Hyperlink](https://github.com/KornSW/MDgen)

### Methods:



## .Foooo
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|a|String|**IN**-Param (required)|
|b|Int32|**OUT**-Param |
**return value:** Boolean



## .Kkkkkk
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|optParamA|Int32? *(nullable)*|**IN**-Param (optional)|
|optParamB|String|**IN**-Param (optional)|
**return value:** [TestModel](#TestModel)



## .AVoid
Meth
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|errorCode|[TestModel](#TestModel)|**IN**-Param (required): Bbbbbb|
no return value (void)



## .TestNullableDt
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|dt|DateTime? *(nullable)*|**IN**-Param (required)|
**return value:** Boolean



## .GetOriginIdentity
Returns an string, representing the "Identity" of the current origin.
This can be used to discriminate multiple source repos.
(usually it should be related to a SCOPE like {DbServer}+{DbName/Schema}+{EntityName})
NOTE: This is an technical disciminator and it is not required, that it is an human-readable
"frieldly-name". It can just be an Hash or Uid, so its NOT RECOMMENDED to use it as display label!
no parameters
**return value:** String



## .GetCapabilities
Returns an property bag which holds information about the implemented/supported capabilities of this IRepository.
no parameters
**return value:** [RepositoryCapabilities](#RepositoryCapabilities)



## .GetEntityRefs
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|filter|[ExpressionTree](#ExpressionTree)|**IN**-Param (required): (from 'FUSE-fx.RepositoryContract')|
|sortedBy|String[] *(array)*|**IN**-Param (required): An array of field names to be used for sorting the results (before 'limit' and 'skip' is processed). Use the character "^" as prefix for DESC sorting. Sample: ['^Age','Lastname']|
|limit|Int32? *(nullable)*|**IN**-Param (optional)|
|skip|Int32? *(nullable)*|**IN**-Param (optional)|
**return value:** [EntityRef_String](#EntityRef_String)[] *(array)*



## .GetEntityRefsBySearchExpression
NOTE: this method can only be used, if the 'SupportsStringBaseSearchExpressions'-Capability is given for this repository!
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|searchExpression|String|**IN**-Param (required)|
|sortedBy|String[] *(array)*|**IN**-Param (required): An array of field names to be used for sorting the results (before 'limit' and 'skip' is processed). Use the character "^" as prefix for DESC sorting. Sample: ['^Age','Lastname']|
|limit|Int32? *(nullable)*|**IN**-Param (optional)|
|skip|Int32? *(nullable)*|**IN**-Param (optional)|
**return value:** [EntityRef_String](#EntityRef_String)[] *(array)*



## .GetEntityRefsByKey
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|keysToLoad|String[] *(array)*|**IN**-Param (required)|
**return value:** [EntityRef_String](#EntityRef_String)[] *(array)*



## .GetEntities
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|filter|[ExpressionTree](#ExpressionTree)|**IN**-Param (required): (from 'FUSE-fx.RepositoryContract')|
|sortedBy|String[] *(array)*|**IN**-Param (required): An array of field names to be used for sorting the results (before 'limit' and 'skip' is processed). Use the character "^" as prefix for DESC sorting. Sample: ['^Age','Lastname']|
|limit|Int32? *(nullable)*|**IN**-Param (optional)|
|skip|Int32? *(nullable)*|**IN**-Param (optional)|
**return value:** [TestModel](#TestModel)[] *(array)*



## .GetEntitiesBySearchExpression
NOTE: this method can only be used, if the 'SupportsStringBaseSearchExpressions'-Capability is given for this repository!
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|searchExpression|String|**IN**-Param (required)|
|sortedBy|String[] *(array)*|**IN**-Param (required): An array of field names to be used for sorting the results (before 'limit' and 'skip' is processed). Use the character "^" as prefix for DESC sorting. Sample: ['^Age','Lastname']|
|limit|Int32? *(nullable)*|**IN**-Param (optional)|
|skip|Int32? *(nullable)*|**IN**-Param (optional)|
**return value:** [TestModel](#TestModel)[] *(array)*



## .GetEntitiesByKey
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|keysToLoad|String[] *(array)*|**IN**-Param (required)|
**return value:** [TestModel](#TestModel)[] *(array)*



## .GetEntityFields
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|filter|[ExpressionTree](#ExpressionTree)|**IN**-Param (required): (from 'FUSE-fx.RepositoryContract')|
|includedFieldNames|String[] *(array)*|**IN**-Param (required): An array of field names to be loaded|
|sortedBy|String[] *(array)*|**IN**-Param (required): An array of field names to be used for sorting the results (before 'limit' and 'skip' is processed). Use the character "^" as prefix for DESC sorting. Sample: ['^Age','Lastname']|
|limit|Int32? *(nullable)*|**IN**-Param (optional)|
|skip|Int32? *(nullable)*|**IN**-Param (optional)|
**return value:** Dictionary_String_Object[] *(array)*



## .GetEntityFieldsBySearchExpression
NOTE: this method can only be used, if the 'SupportsStringBaseSearchExpressions'-Capability is given for this repository!
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|searchExpression|String|**IN**-Param (required)|
|includedFieldNames|String[] *(array)*|**IN**-Param (required): An array of field names to be loaded|
|sortedBy|String[] *(array)*|**IN**-Param (required): An array of field names to be used for sorting the results (before 'limit' and 'skip' is processed). Use the character "^" as prefix for DESC sorting. Sample: ['^Age','Lastname']|
|limit|Int32? *(nullable)*|**IN**-Param (optional)|
|skip|Int32? *(nullable)*|**IN**-Param (optional)|
**return value:** Dictionary_String_Object[] *(array)*



## .GetEntityFieldsByKey
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|keysToLoad|String[] *(array)*|**IN**-Param (required)|
|includedFieldNames|String[] *(array)*|**IN**-Param (required)|
**return value:** Dictionary_String_Object[] *(array)*



## .CountAll
no parameters
**return value:** Int32



## .Count
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|filter|[ExpressionTree](#ExpressionTree)|**IN**-Param (required): (from 'FUSE-fx.RepositoryContract')|
**return value:** Int32



## .CountBySearchExpression
NOTE: this method can only be used, if the 'SupportsStringBaseSearchExpressions'-Capability is given for this repository!
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|searchExpression|String|**IN**-Param (required)|
**return value:** Int32



## .ContainsKey
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|key|String|**IN**-Param (required)|
**return value:** Boolean



## .AddOrUpdateEntityFields
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|fields|Dictionary_String_Object|**IN**-Param (required)|
**return value:** Dictionary_String_Object



## .AddOrUpdateEntity
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|entity|[TestModel](#TestModel)|**IN**-Param (required): MMMMMMMMMMMMMMMMMMM|
**return value:** [TestModel](#TestModel)



## .TryUpdateEntityFields
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|fields|Dictionary_String_Object|**IN**-Param (required)|
**return value:** Dictionary_String_Object



## .TryUpdateEntity
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|entity|[TestModel](#TestModel)|**IN**-Param (required): MMMMMMMMMMMMMMMMMMM|
**return value:** [TestModel](#TestModel)



## .TryAddEntity
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|entity|[TestModel](#TestModel)|**IN**-Param (required): MMMMMMMMMMMMMMMMMMM|
**return value:** String



## .MassupdateByKeys
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|keysToUpdate|String[] *(array)*|**IN**-Param (required)|
|fields|Dictionary_String_Object|**IN**-Param (required)|
**return value:** String[] *(array)*



## .Massupdate
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|filter|[ExpressionTree](#ExpressionTree)|**IN**-Param (required): (from 'FUSE-fx.RepositoryContract')|
|fields|Dictionary_String_Object|**IN**-Param (required)|
**return value:** String[] *(array)*



## .MassupdateBySearchExpression
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|searchExpression|String|**IN**-Param (required)|
|fields|Dictionary_String_Object|**IN**-Param (required)|
**return value:** String[] *(array)*



## .TryDeleteEntities
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|keysToDelete|String[] *(array)*|**IN**-Param (required)|
**return value:** String[] *(array)*



## .TryUpdateKey
#### Parameters:
|Name|Type|Description|
|----|----|-----------|
|currentKey|String|**IN**-Param (required)|
|newKey|String|**IN**-Param (required)|
**return value:** Boolean
# ApiBase

### Methods:



## .BaseIsUsable
descibes if the base is usable
no parameters
**return value:** Boolean



# Models:



## TestModel
MMMMMMMMMMMMMMMMMMM
#### Fields:
|Name|Type|Description|
|----|----|-----------|
|FooBar|String|(required): jfjfj|
|RootNode|[TestNode](#TestNode)|(required): fsdfsf|
|fff|String[] *(array)*|(required)|



## TestNode
fsdfsf
#### Fields:
|Name|Type|Description|
|----|----|-----------|
|ChildNode|[TestNode](#TestNode)|(required): fsdfsf|



## EntityRef
(from 'FUSE-fx.RepositoryContract')
EntityRef (UNTYPED)
#### Fields:
|Name|Type|Description|
|----|----|-----------|
|Key|Object|(optional)|
|Label|String|(optional)|



## EntityRef_String
(from 'FUSE-fx.RepositoryContract')
EntityRef with Typed Key (generic)
#### Fields:
|Name|Type|Description|
|----|----|-----------|
|Key|String|(optional)|
|Key|Object|(optional)|
|Label|String|(optional)|



## ExpressionTree
(from 'FUSE-fx.RepositoryContract')
#### Fields:
|Name|Type|Description|
|----|----|-----------|
|MatchAll|Boolean? *(nullable)*|(optional): true: AND-Relation | false: OR-Relation|
|Negate|Boolean? *(nullable)*|(optional): Negates the result|
|Predicates|List_FieldPredicate|(optional): Can contain ATOMIC predicates (FieldName~Value). NOTE: If there is more than one predicate with the same FieldName in combination with MatchAll=true, then this will lead to an subordinated OR-Expression dedicated to this field.|
|SubTree|List_ExpressionTree|(optional)|



## RepositoryCapabilities
(from 'FUSE-fx.RepositoryContract')
An property bag which holds information about the implemented/supported
capabilities of an IRepository.
#### Fields:
|Name|Type|Description|
|----|----|-----------|
|CanReadContent|Boolean? *(nullable)*|(optional): Indicates, that this repository offers access to load entities(classes) or some their entity fields (if this is false, then only EntityRefs are accessable)|
|CanUpdateContent|Boolean? *(nullable)*|(optional)|
|CanAddNewEntities|Boolean? *(nullable)*|(optional)|
|CanDeleteEntities|Boolean? *(nullable)*|(optional)|
|SupportsMassupdate|Boolean? *(nullable)*|(optional)|
|SupportsKeyUpdate|Boolean? *(nullable)*|(optional)|
|SupportsStringBasedSearchExpressions|Boolean? *(nullable)*|(optional)|
|RequiresExternalKeys|Boolean? *(nullable)*|(optional): Indicates, that entities can only be added to this repository, if ther key fields are pre-initialized by the caller. If false, then the persistence-technology behind the repository implementation will auto-generate a new key by its own.|


![](https://github.com/DynamoDS/NodePropertyPalette/workflows/NodePropertyPalette-Build/badge.svg)

# NodePropertyPalette

NodePropertyPalette is in `beta`.

NodePropertyPalette is a view extension for getting all nodes within Dynamo graph and enable bulk operation.

## Building
### Recommended Build Environment
- VisualStudio 2019
- .Net Framework 4.7 Developer Pack
- Dynamo repository cloned and built on the same level of NodePropertyPalette repository which means your Dynamo repo and NodePropertyPalette repo should exist under the same parent folder.

### Result Binaries
- After a `Debug` build of NodePropertyPalette one can expect:
    - Under `NodePropertyPalette\dist\NodePropertyPalette`, there is a sample package wrapped up ready for publishing and adoption. This would be the un-optimized version.
    - Un-optimized package installed locally for [DynamoVersion] defined in NodePropertyPalette/NodePropertyPalette.csproj, under DynamoCore and DynamoRevit
- After a `Release` build of NodePropertyPalette one can expect:
Under `NodePropertyPalette\dist\NodePropertyPalette`, there is a sample package wrapped up ready for publishing and adoption. This would be the optimized version.

## Known issues
- NodePropertyPalette binaries are not semantically versioned and are not intended to be built on top of as an API. Do not treat these binaries like DynamoCore.
- NodePropertyPalette requires Dynamo 2.5 or higher for access to new extension APIs.

## Testing

### Setup

### Running NodePropertyPalette Unit Tests
- Not supported uet

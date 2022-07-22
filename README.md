# Inventory Search Bar Plugin

A Final Fantasy XIV Dalamud Plugin that adds a search bar to the inventory windows to filter items.

## Usage

Use search bar located at the top of the inventory window to filter your items.

![example](https://github.com/Tischel/InventorySearchBar/blob/master/Images/example_1.gif)

## Filters

### Name

The item will be highlighted if the name contains the search term.\
Default tags: `name` and `n`.\
Example: `name:Materia`.

### Type

The item will be highlighted if the item type contains the search term.\
Default tags: `type` and `t`.\
Example: `type:Ingredient`.

### Job

The item will be highlighted if the search term matches a job abbreviation and the item can be equipped by that job.\
Default tags: `job` and `j`.\
Example: `job:BLM`.

### Level

The item will be highlighted if the search term matches a job abbreviation and the item can be equipped by that job.\
Default tags: `level` and `l`.\
Example: `level:70` would find all items that require level 70 or lower.\
Example: `level:>80` would find all items that require level 81 or higher.\
Example: `level:=90` would find all items that require level 90.\
Example: `level:>=80` would find all items that require level 80 or higher.

## Combining Filters

Multiple filters can be used in a single search.\
Example: `j:blm t:neck` would find all items equippable by a Black Mage on the Necklace slot.\
Example: `t:ingredient n:tomato` would find all the ingredients that contain "tomato" in their name.\
\
Filters can be configured to be used without a tag as well.\
Example: `j:blm Augmented` would find all the items equippable by a Black Mage that contain "Augmented" in their name.

## Supported Inventories

- Character Inventory
- Chocobo Saddle
- Retainer Inventory
- Armoury

## Known Issues

- Premium Cocobo Saddle not yet supported

# Unity-Projects
![](https://img.shields.io/badge/Code-CSharp-informational?style=plastic&logo=csharp&logoColor=white&color=283443)
![](https://img.shields.io/badge/Engine-Unity-informational?style=plastic&logo=unity&logoColor=white&color=283443)
![](https://img.shields.io/badge/Software-Visual_Studio_Code-informational?style=plastic&logo=visualstudiocode&logoColor=white&color=283443)
![](https://img.shields.io/badge/Software-Tiled-informational?style=plastic&logo=&logoColor=white&color=283443)
<br>
Collection of small Unity test projects I created to learn the engine between 2017 and 2018.

## Table of Contents
  - [General Information](#general-information)
  - [Technologies Used](#technologies-used)
  - [How To Run](#how-to-run)
  - [Illustrations](#illustrations)
  - [Features](#features)
  - [Sources](#sources)

## General Information
Collection of small Unity test projects I created to learn the engine between 2017 and 2018.
This includes: 
- *Ocean's Solitude*: Controlling a small fish in a 2D level created with [Tiled](https://www.mapeditor.org/) where the player needs to flee from an enemy fish using abilities.
- *RTS Prototype*: Controlling multiple units via a unit statemachine and simple UI with a few enemy units to fight.
- *ZDungeon*: Zelda-like dungeons created with [Tiled](https://www.mapeditor.org/). Simple 2D character movement, sword fighting, keys, switches and doors.

## Technologies Used

## How To Run
1. Requirements:

2. Exported Builds:
Exported builds can be found in the `builds` directory. It includes windows exports for each projects. 

## Illustrations
### Ocean's Solitude

### RTS Prototype

### ZDungeon

## Features
### Ocean's Solitude
- Main menu.
- Adventure style game, where the player controls a fish with a few abilities and an enemy to fight.
- Level created with Tiled.
- Movement Logic for Player fish with abilities.
- Enemy Fish Logic based on sight.

### RTS Prototype
- Unit control with state machine that allows for commands like 'hold position', 'patrol' or 'attack'.
- Unit selection for single units and multiple units and simple movement.
- Enemy units you can attack.

### ZDungeon
- Create simple 2D dungeons with tiled.
- Using editor scripts to generate logical connections between entities in the level. For example: connecting a switch to a door.
- Character controller, that allows movement and sword fighting.

## Sources
- [Tiled](https://www.mapeditor.org/): Used to create 2D tilemaps.
- [Unity Engine](https://unity.com/): Used to create test projects.
- [Tiled2Unity](https://seanba.com/supertiled2unity.html): To automatically import maps created with Tiled.

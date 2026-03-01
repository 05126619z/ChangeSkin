# ChangeSkin - Character Skin Replacement
## Be sure to leave a 🌟
Mod support: https://discord.gg/aCBrFPYYjs

Skins workshop website: https://skin.cat-bot.de/
## Description
This mod allows you to easialy replace skin in Scav without tampering with game files.
## Features
![Usage Demonstration](https://github.com/05126619z/ChangeSkin/blob/main/readme/demo.gif)
<!-- - Changing look of your character in WoundView menu -->
- Changing skin of your character in main gameplay
- Hotswapping skins without relaunching the game
- Remembers which skin you last selected
- Easy to use
- Works in multiplayer

## Installation
1. Download and install BepInEx From official repository: https://github.com/BepInEx/BepInEx/releases/latest
2. Launch game once
3. Unpack mod into `CasualtiesUnknownDemo/BepInEx/plugins` folder
4. Enjoy!

## Usage
1. Open in-game console after starting the run
2. Type `skin load local robot` to load local robot skin(or any other if you made some) or `skin load remote https://skin.cat-bot.de/d/59` to load them from the portal
3. Type `skin enable`
4. Enjoy!

## Creating skins
To use your own skin you have to duplicate and rename the `robot` folder, and replace all the textures in it with the new ones

## Usable commands
1. Type `skin init` to initialize the mod
2. Type `skin load local {skinName}` to load skin which you have locally
3. Type `skin load remote {skinURL}` to load skin from url
4. Type `skin rule set skinuploading true/false` to allow or disallow your local skin uploading
5. Type `skin rule set skindownloading true/false` to allow or disallow skin downloading from links
6. Type `skin ban/unban {playername}` to ban player skins locally
7. Type `skin reload` to reload all player's skins
8. Type `skin clearcache` to clear cache of the mod
9. Type `skin verbose true/false` to enable verbosity in the console
10. Type `skin unload` to unload your local skin
11. Type `skin enable/disable` to enable or disable the mod

## Special thanks
Special thanks to @speed_buump for skin textures and @garythecat for skin portal

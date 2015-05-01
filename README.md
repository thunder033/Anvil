# Anvil
Anvil is an C#/XAML asset management tool tailored to XNA/Monogame projects, augmenting the XNA content pipeline. Anvil was
originally developed as a part of a game project and still maintains a few references. This project is currently on hiatus,
and is only about 60 - 70% of what I want it to be, but I wanted to upload here in case someone found it useful.

Features include the ability to import spritesheets, generate animation properties files, and automatically converting files 
to XNB format. This simplifies the XNA content pipeline that is often complex for newer users.

Anvil takes advantage of the XNA Content Compiler <br>
(https://xnacontentcompiler.codeplex.com/)

Also essential to the application's functionality is Dr. WPF's Observable Dictionary <br>
(http://drwpf.com/blog/2007/09/16/can-i-bind-my-itemscontrol-to-a-dictionary/)

# Setup
Anvil provides a dialog to define your project directory and structure:

- "Project Directory" is the root directory of your project.
- "Textures Directory" is the folder XNB files should be written to.
- "Textures Index File" contains a list of all the textures that can be read by your program.

- "Animation Definitions Directory" is a the directory containing animation properties files
- "Animation Definitions Index" contains a list of all the animation properties files
- "Animation File Type" is the file extension animation properties files should be created with

- "Texture Preview Directory" is the folder containing standard image files for Anvil to use

- "Animation Fields" is a comma-separated list of custom animation properties

#Animation Definitions
Anvil currently outputs ASCII text files that contain basic animation properties to be used for a given sprite sheet.
Default fields include animation name, texture name, number of frames, the frame duration (in seconds), frame dimensions,
offset of the animation in the sprite sheet, and dimensions of the object in the game world. Any number of additional fields
can be added.

Also included in the Animation Definitions is a collision polygon composed of multiple rectangles that can be created for
each frame via a drag and drop interface. Different types can be given to these polygons to allow different types of
interactions in the game engine.

Logic exists that is tailored to leverage these Animation Definitions, but it is somewhat heavily tied into the original game
project.

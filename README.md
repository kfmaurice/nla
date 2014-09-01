# nla

An extension for Visual Studio extension to enable autocompletion at uncommon places like for instance in comments, by variable naming or string variable initialization.

## Table of contents

- [Usages](#usages)
- [Configuration](#configuration)
- [Installation](#installation)
- [Miscellaneous](#miscellaneous)

## Usages

Here are in pictures some cases in which the plug-in can be useful.

#### Variable declaration and string initialization
<img src="http://makefoo.net/wp-content/uploads/2014/08/nla1.png" alt="variable declaration" />
<img src="http://makefoo.net/wp-content/uploads/2014/08/nla2.png" alt="string initialization" style="margin-left: 30px"/>
#### Comments
<img src="http://makefoo.net/wp-content/uploads/2014/08/nla3.png" alt="comments" />

## Configuration

There is an option dialog in which you could configure the plugin to support Camel, Pascal or Snake Case as well as disable 
autocompletion in string variables.

<img src="http://makefoo.net/wp-content/uploads/2014/08/vsoptions.png" alt="plug-in options" />

## Installation

Installation is actually not possible since the plug-in hasn't been released yet. A link will be published here as soon as possible. In the meantime, feel free to download the source files, compile and install locally by using the vsix file in the bin\release directory. Remember that you need at least a pro version of Visual Studio to open the project.

This project has been initially created with VS 2010 pro and then migrated to VS 2013 premium. Note that after the migration, following changes were made in order to make the project debuggable:

- the VS SDK 2013 had to be installed
- the references to the VS SDK 2013 assemblies needed to be manually updated (deleted and readded) in the StartUp project
- "Start Action" in the StartUp project needed to point to the new devenv.exe executable in order for a debug session to start

<img src="http://makefoo.net/wp-content/uploads/2014/08/nla4.png" alt="project properties" />

## Miscellaneous

The code from this [sample](http://msdn.microsoft.com/en-us/library/ee372314(v=vs.100).aspx) were used in this plug-in. A short documentation of the changes made is documented [here](http://makefoo.net/?p=230)


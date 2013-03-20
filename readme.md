# XSExtra

XSExtra is a little addin for [Xamarin Studio](http://xamarin.com/studio) that adds a couple of handy commands:

## Features

### Duplicate File Command

Duplicates any file in your project, optionally doing a search and replace at the same time.

eg: Say you have `MyClass.cs` with something like:

	using System;

	class MyClass
	{
	}

Right click on the file in Solution explorer, and choose Duplicate.  Enter a new name, say "MyNewClass" and press OK.  You'll get a new file `MyNewClass.cs` with the same project settings containing this:

	using System;

	class MyNewClass
	{
	}

The renaming within the file is a simple case-sensitive search and replace.  You can disable renaming within the file by turning off the `Update Name in File Contents` option in the dialog
that appears.  This is off by default for non-text files.


### Edit as Text Command

Right click on a project or solution file and choose "Edit as Text" to open the file in the source code editor.

NB: There's a small issue with using this on .sln files where Xamarin Studio will remove the file from it's recently used solution list. Oh well.


### Synchronize with File System Command

Right click on a project folder and choose "Synchronize with File System" to update the project to match files found in the file system.  New files will be added, missing files will be removed,  linked files will be left alone.

Caveats:

* Only available on sub-folders of the project - not the project itself, that seemed just a little to risky.
* Newly added files will have a default build action as determined by Xamarin Studio/MonoDevelop
* Renaming a file externally will work, but if it has a non-default build action that may need to be reset. (Since renaming a file is treated as remove/add file).
* Files that are present both in the project and in the file system are not affected and therefore build action and other project properties are retained.


## Download and Installation

Download the Addin here:

* [https://github.com/toptensoftware/XSExtra/raw/master/out/XSExtra.dll](https://github.com/toptensoftware/XSExtra/raw/master/out/XSExtra.dll).

Install on Windows by copying to:

* C:\Users\yourusername\AppData\Local\XamarinStudio-4.0\LocalInstall\Addins

Install on OSX/Linux like this:

	mkdir -p ~/Library/Application\ Support/XamarinStudio-4.0/LocalInstall/Addins
	cd ~/Library/Application\ Support/XamarinStudio-4.0/LocalInstall/Addins
	curl -OL https://github.com/toptensoftware/XSExtra/raw/master/out/XSExtra.dll

Should also work in MonoDevelop, but not tested.


## Build

Building is straight forward except the project's output folder is set to my user profiles AppData path to simplify debugging - you should change that directory before building it yourself.

The release mode build has a post build script that copies the file back to the solutionDir/out directory so it can be included in git for easy download from github.

## That's All

Enjoy!

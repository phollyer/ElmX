# ElmX

ElmX is a command line tool for helping with Elm development.

Currently it can be used to find all unused modules in an Elm project, which is useful for cleaning up said project.

However, I intend to continue developing this further, and add many more features, so please see the TODO list below for more information on this. The end goal is to add a GUI wrapper around it, so that it can also be used by folks who prefer not to use the command line.

## Installation

### OSX

Head over to the [Releases](https://github.com/phollyer/elmx/releases) page and grab the binary for your Mac - both x64 and arm64 are supported. Place the binary somewhere on your PATH, and you're good to go - except for Apples security settings. You'll need to enable the binary to run by making it executable. To do this, open a terminal and run the following command:

``` shell
chmod 755 /path/to/elmx
```

The last step is to navigate to the directory where you've placed the binary, in `Finder`, right click and select Open, this will allow you to run `elmx` from that point on.

### Linux

Head over to the [Releases](https://github.com/phollyer/elmx/releases) page and grab the binary for your Linux distro. Place the binary somewhere on your PATH, and you should be good to go. 

If you're not sure where to place the binary, you can place it in `/usr/local/bin`, which is a good place for user installed binaries.

You'll need to make the binary executable, so open a terminal and run the following command:

``` shell
chmod 755 /path/to/elmx
```

### Windows

At the moment I don't have a Windows machine so can't compile to Windows OS. So if you work on Windows, and want to use this tool, you'll have to compile it yourself. Sorry about that. If you do compile it, please consider submitting a PR to add the binary to the Releases page so that others can benefit from your work. Place the binary somewhere on your PATH, and you're good to go.

### Any other OS

If you require a binary for an OS that isn't listed above, please open an issue and I'll see what I can do. If I'm not able to compile to your OS, then you'll have to compile it yourself. Sorry about that. If you do compile it, please consider submitting a PR to add the binary to the Releases page so that others can benefit from your work. However, I may be able to help you compile it if you're not sure how to do it yourself, so please ask.

### Compiling from source

If you want to compile from source, you'll need to have [dotnet](https://dotnet.microsoft.com/download) installed. Once you have that, clone this repo, and run the following command from the root of the repo:

``` shell
dotnet publish -r <runtime-target> -c Publish --self-contained
```

You can then find the binary in the `bin/Publish/net<dotnet-version>/<runtime-target>/publish` folder.

### Any Questions

Please start a discussion.

## Usage

### Find unused modules

This the only feature available at this time.

The simplest way is to `cd` to the root of your Elm project (where your `elm.json` resides), and run:

``` shell
elmx unused-modules [options]
```

This will find all unused modules in your project, and perform the required task based on the options you provide.

``` shell
[unused-modules-options]

-D, --dir <dir>                         Specify the directory to search. If no directory is specified, the current directory is used.
-d, --delete                            Delete the unused modules.
-e, --exclude <dir> <dir> <dir>...      Exclude the specified directories from the search.
-p, --pause                             Pause before each deletion, requesting confirmation before deleting a module.
-r, --rename                            Rename the unused modules. This will add a tilde (~) to the front of the file name.
-s, --show                              Show the unused modules.
```

By default the following assumptions are made:

- Your entry module is `Main.elm`, this can be changed by adding an `elmx.json` file. More on this below.
- You don't want to search any of the `elm-stuff`, `node_modules`, `review` or `tests` folders, so these are excluded, and currently can't be changed. To exclude more folders, use the `-e` or `--exclude` option.

## ElmX Json

ElmX can be configured by adding an `elmx.json` file to the root of your Elm project. This file is optional, and if it doesn't exist, ElmX will use the default values.

The following options are available:

``` json
{
  "entry": "SomeFileNameOtherThanMain.elm"
}
```

Other options will become available as needed and as more features are added.

---

## TODO

(In no particular order)

- [ ] Find and remove unused imports
- [ ] Find and remove unused functions
- [ ] Find and remove unused types
- [ ] Find and remove unused fields
- [ ] Provide quick access to the Elm documentation for a given module
- [ ] Search for and install Elm packages
- [ ] Include the other great tools created by the community, enabling you to use them all from one place
- [ ] Add a templates feature, so that you can create a new Elm project from a predefined template
- [ ] Add a GUI wrapper around the command line tool

---

If you have any suggestions for features, please start a discussion.

If you want to contribute, please do so. I'm happy to accept PRs.

If you use this tool, and find it useful, please consider [donating](https://github.com/sponsors/phollyer) to the project. I'm not asking for much, just enough to buy me a beer every now and then. If you don't want to donate, that's fine too. I'm happy to provide this tool for free, and I'll continue to do so for as long as I can.

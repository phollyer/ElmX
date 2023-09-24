# ElmX

ElmX is a command line tool for helping with Elm development.

Currently it can be used to find all unused modules in an Elm project, which is useful for cleaning up said project. At the moment, ElmX supports projects of type `application`, as defined in your projects' `elm.json` file, `package`s are next on the list.

For details on features that are planned once `packages` are supported, see the [TODO](#todo) section below.

## Installation

### OSX

Head over to the [Releases](https://github.com/phollyer/elmx/releases) page and grab the binary for your Mac - both x64 and arm64 are supported. Place the binary somewhere on your PATH, and you should be good to go.

If you need to make the binary executable, open a terminal and run the following command:

``` shell
chmod 755 /path/to/elmx
```

If you have any problems with Apple's security settings refusing to allow the binary to run, you'll need to navigate to the directory where you've placed the binary, (you must do this in `Finder`), then right click the binary and select Open, this will allow you to run `elmx` from that point on.

### Linux

Head over to the [Releases](https://github.com/phollyer/elmx/releases) page and grab the binary for your Linux distro. Place the binary somewhere on your PATH, and you should be good to go.

If you're not sure where to place the binary, you can place it in `/usr/local/bin`, which is a good place for user installed binaries.

If you need to make the binary executable, open a terminal and run the following command:

``` shell
chmod 755 /path/to/elmx
```

### Windows

At the moment I don't have a Windows machine and therefore I can't compile to Windows OS. So if you work on Windows, and want to use this tool, you'll have to compile it yourself. Sorry about that. If you do compile it, please consider submitting a PR to add the binary to the Releases page so that others can benefit from your work.

I do intend to get a Windows machine as soon as I am financially able to do so, so that I can compile to Windows OS, but I don't know when that will be.

### Any other OS

If you require a binary for an OS that isn't listed above, please open an issue and I'll see what I can do. If I'm not able to compile to your OS, then you'll have to compile it yourself. Sorry about that. If you do compile it, please consider submitting a PR to add the binary to the Releases page so that others can benefit from your work. However, I may be able to help you compile it if you're not sure how to do it yourself, so please ask.

### Compiling from source

If you want to compile from source, you'll need to have [dotnet](https://dotnet.microsoft.com/download) installed. Once you have that, clone this repo, and run the following command from the root of the repo:

``` shell
dotnet publish -r <runtime-target> -c Publish --self-contained
```

Where `<runtime-target>` is the runtime you want to target. For example, if you want to target OSX, you would use `osx-x64`. For a full list of runtime targets, see [this page](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).

You can then find the binary in the `bin/Publish/net<dotnet-version>/<runtime-target>/publish` folder.

### Any Questions

Please start a discussion.

## Usage

### Initial setup

Before you can use ElmX, you'll need to create a config file (`elmx.json`). This is a simple JSON file that tells ElmX the following information:

- "entryFile": The name of your entry module. This defaults to `src/Main.elm`, but if you use a different name, you can specify it here.
- "excludedDirs": A list of directories to exclude from the search. This defaults to `["elm-stuff", "node_modules", "review", "tests"]`, but you can add to this list if you want to exclude more directories.
- "excludedFiles": A list of files to exclude from the search. This is useful if you have one or more modules that you are working on but are not yet `import`ed into any of your project files.

Once this has been created, you can edit it to suit your needs at any time.

``` shell
# First
cd /path/to/your/elm/project # this is the root of your Elm project, where your elm.json file resides

# Then create the config file with the default values
elmx init

# or, if you want to specify the values when creating the config file the following options are available for the init command
-e, --entry-file <file>                         Specify the entry file of the Elm project.
-d, --exclude-dirs <dir> <dir> <dir>...         Exclude the specified directories from the search.
-f, --exclude-files <file> <file> <file>...     Exclude the specified files from the search.
```

Once your `elmx.json` file has been created you can then use the following commands.

### Find unused modules

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

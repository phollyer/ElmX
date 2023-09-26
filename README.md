# ElmX

ElmX is a command line tool for helping with Elm development.

Currently it can be used to find all unused modules in an Elm project, which is useful for cleaning up said project. At the moment, ElmX supports projects of type `application`, as defined in your projects' `elm.json` file - `package`s are next on the list.

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

If there is not a binary for your Linux distro, you'll either need to compile it yourself - see [Compiling from source](#compiling-from-source), or open an issue and I should be able to compile it for you.

### Windows

At the moment I don't have a Windows machine and therefore I can't compile to Windows OS. So if you work on Windows, and want to use this tool, you'll have to compile it yourself - see [Compiling from source](#compiling-from-source). Sorry about that. If you do compile it, please consider submitting a PR to add the binary to the Releases page so that others can benefit from your work.

I do intend to get a Windows machine as soon as I am financially able to do so, so that I can compile to Windows OS, but I don't know when that will be.

### Any other OS

If you require a binary for an OS that isn't listed above, please open an issue and I'll see what I can do. If I'm not able to compile to your OS, then you'll have to compile it yourself - see [Compiling from source](#compiling-from-source). Sorry about that. If you do compile it, please consider submitting a PR to add the binary to the Releases page so that others can benefit from your work. However, I may be able to help you compile it if you're not sure how to do it yourself, so please ask.

### Compiling from source

If you want to compile from source, you'll need to have [dotnet](https://dotnet.microsoft.com/download) installed. Once you have that, clone this repo, and run the following command from the root of the repo:

``` shell
dotnet publish -r <runtime-target> -c Publish --self-contained
```

Where `<runtime-target>` is the runtime you want to target. For example, if you want to target OSX, you could use `osx-x64` or `osx-arm64`. For a full list of runtime targets, see [this page](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).

You can then find the binary in the `bin/Publish/net<dotnet-version>/<runtime-target>/publish` folder.

## Usage

### Initial setup

Before you can use ElmX, you'll need to create a config file (`elmx.json`). This is a simple JSON file that tells ElmX the following information:

- "entry-file": The name of, and path to, your entry module from your `elm.json` file (this is not applicable to packages). This defaults to `src/Main.elm`, but if you use a different name or path, you can specify it here.
- "exclude-dirs": A list of directories to exclude from the search. This defaults to `["elm-stuff", "node_modules", "review", "tests"]`, but you can add to this list if you want to exclude more directories.
- "exclude-files": A list of files to exclude from the search. This is useful if you have one or more modules that you are working on but that are not yet ready to `import` into any of your project files.

You can create this file manually, or you can use the `init` command to create it for you. Once this has been created, you can edit it to suit your needs at any time.

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

Once your `elmx.json` file has been created you can then use `elmx` from the same directory as your `elm.json` file.

### Find unused modules

From the root of your Elm project (where your `elm.json` resides), run:

``` shell
elmx unused-modules [options]
```

This will find all unused modules in your project, and perform the required task based on the options you provide.

``` shell
[unused-modules-options]

-d, --delete                            Delete the unused modules.
-e, --exclude <dir> <dir> <dir>...      Exclude the specified directories from the search.
-p, --pause                             Pause before each deletion, requesting confirmation before deleting a module.
-r, --rename                            Rename the unused modules. This will add a tilde (~) to the front of the file name.
-s, --show                              Show the unused modules.
```

---

### Any Questions, Comments, Suggestions

Please start a discussion.

### Found a bug or edge case that isn't handled

Please open an issue.

## TODO

(In no particular order at the moment, and not an exhaustive list)

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

### My current situation

I am currently, and have been for the last three years, full-time carer for my mother, who has Alzheimers. As a result I am forced to work close to home (in case I'm needed at home quickly), in a part-time job paying minimum wage, and I have very little time to work on my own projects. I am hoping that I can get enough support from the community to allow me to work from home on this project full-time, which will allow me to provide more features, faster, and to provide top-notch support.

If you do use this tool, and find it useful, or you think it is a worthwhile project you would like to see flourish in the Elm community, please consider [donating](https://github.com/sponsors/phollyer) to the project.

It would make a world of difference, both to me and my mother - who would benefit immensely from me being able to work from home, and to the Elm community - who, dare I say it, would have a great tool to help them with their Elm development.

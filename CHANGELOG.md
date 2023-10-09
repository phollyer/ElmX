# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [unreleased]

### Fixed

- Handle nested comments correctly.

    ```elm
    {- 
        import Module1 {- some comments 
        {- followed by -} 
        {- some more {- nested comments -} -}     
        -} exposing (foo, bar) 
    -}
    ```

## [1.1.1] - 2023-09-26

### Fixed

- The "exclude-files" property of `elmx.json` is now correctly read and applied.

### Added

- Support for Elm Packages.

## [1.1.0] - 2023-09-25

### Fixed

- The `source-directories` property from the `elm.json` file is now read by ElmX to ensure unused modules are found correctly. However, only directories on the same level as the `elm.json` file are searched. If you have a directory structure like this:

    ``` shell
    elm-project
    ├── elm.json
    ├── src
    │   ├── Main.elm
    │   ├── SubDir
    │   │   ├── SubModule.elm
    |   src2
    |   ├── Something.elm
    ```

    And `Something.elm` is not imported by any other module, then this will be found as unused providing `src2` is in the `source-directories` property of the `elm.json` file.

    If your `elm.json` file looks like this:

    ``` json
    {
        "type": "application",
        "source-directories": [
            "src",
            "src2",
            "../some-other-dir"
        ],
        "elm-version": "0.19.1",
        "dependencies": {
            "direct": {
                ...
            },
            "indirect": {
                ...
            }
        },
        "test-dependencies": {
            "direct": {},
            "indirect": {}
        }
    }
    ```

    Then `../some-other-dir` will not be searched, but `src` & `src2` will. In my opinion, the path `../some-other-dir` may be to a shared library across multiple projects, and therefore should not be searched. If you have a valid use case where this directory path should be searched, please start a discussion initially so that I can fully understand your requirements. If we are in agreement that your use case is a worthwhile endeavour to support, we can then raise an issue and I'll get to work on resolving it.

- Import statements that span multiple lines are now supported.

    ```elm

    import
        Module1
        exposing
            ( foo
            , bar
            )
    ```

- Import statements that have a trailing single line comment are now supported.

    ```elm
    import Module1 exposing (foo, bar) -- this is a comment
    ```

- Import statements that are commented out with a single line comment are now supported, provided the import statement is directly preceded by the comment symbol (`--`).

    ```elm
    -- import Module1 exposing (foo, bar) may or may not have trailing text
    ```

- Import statements that include a multiline comment are now supported, whether the multiline comment is on a single line or spanning multiple lines.

    ```elm
    import Module1 exposing (foo, bar) {- this is a multiline comment -}

    import Module1 {- this is a multiline comment -} exposing (foo, bar)

    import Module1 exposing (foo, bar) {- this is 
    a multiline
    comment -}

    import Module1 {- this is
    a multiline
    comment -} 
    exposing 
    (foo, bar)

    ```

- Import statements that are commented out with a multline comment are now supported when the comment is spanning multiple lines.

    ```elm
    {- import 
        Module1 
        exposing (foo, bar) 

        may or may not have
        trailing text
    -}
    ```

- Import statements that are commented out with a multiline comment are now supported when the comment is a single line provided the import statement is directly preceeded by the opening comment symbol (`{-`).

    ```elm
    {- import Module1 exposing (foo, bar) may or may not have trailing text -}
    ```

- Import statements inside a multiline string are now supported.

    ```elm
    myVar = """
    import Module1 exposing (foo, bar) may or may not have trailing text
    """
    ```

- Cyclical imports are now supported.

    ```elm
    -- Module1.elm
    import Module2 exposing (foo, bar)

    -- Module2.elm
    import Module1 exposing (foo, bar)
    ```

    If neither of these modules are imported into any other module that _is used_, then both will be found as unused.

Many thanks to [lydell](https://github.com/lydell/) for pointing out the edge cases [here](https://discourse.elm-lang.org/t/elmx-find-and-delete-unused-modules/9309/7).

### Added

- `init` command to initialize the `elmx.json` file.

- `-h`, `--help` flags to show help for the `unused-modules` command.

## [1.0.2] - 2023-09-06

### Improved

- Finding unused modules is now much faster. On an Elm project with 286 unique imports across 223 files, the processing time is reduced from an average of 15 seconds to 0.5 seconds.

## [1.0.1] - 2023-09-06

### Fixed

- -v, --version flags now work correctly

## [1.0.0] - 2023-09-05

### Added

- Initial Commit.

[1.1.1]: https://github.com/phollyer/elmx/compare/v1.1.0...v1.1.1
[1.1.0]: https://github.com/phollyer/elmx/compare/v1.0.2...v1.1.0
[1.0.2]: https://github.com/phollyer/elmx/compare/v1.0.1...v1.0.2
[1.0.1]: https://github.com/phollyer/elmx/compare/1.0.0...v1.0.1
[1.0.0]: https://github.com/phollyer/elmx/releases/tag/1.0.0

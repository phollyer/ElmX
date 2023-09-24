# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [unreleased]

### Fixed

- Now using the `source-directories` property from the `elm.json` file to ensure unused modules are found correctly.

- Import statements that span multiple lines are now handled correctly.
- Import statements that have a trailing single line comment are now handled correctly.
- Imports that are commented out with a single line comment are now handled correctly, provided the import statement is directly preceded by the comment.
- Import statements that include a multiline comment are now handled correctly, whether the multiline comment is on a single line or spanning multiple lines.
- Import statements that are commented out with a multline comment are now handled correctly when the comment is spanning multiple lines.
- Import statements that are commented out with a multiline comment are now handled correctly when the comment is a single line provided the import statement is directly preceeded by the opening comment.
- Import statements inside a multiline string are now handled correctly.
- Cyclical imports are now handled correctly.

Many thanks to [lydell](https://github.com/lydell/) for pointing out the edge cases [here](https://discourse.elm-lang.org/t/elmx-find-and-delete-unused-modules/9309/7).

## [1.0.2] - 2023-09-06

### Improved

- Finding unused modules is now much faster. On an Elm project with 286 unique imports across 223 files, the processing time is reduced from an average of 15 seconds to 0.5 seconds.

## [1.0.1] - 2023-09-06

### Fixed

- -v, --version flags now work correctly

## [1.0.0] - 2023-09-05

### Added

- Initial Commit.

[1.0.2]: https://github.com/phollyer/elmx/compare/v1.0.1...v1.0.2
[1.0.1]: https://github.com/phollyer/elmx/compare/1.0.0...v1.0.1
[1.0.0]: https://github.com/phollyer/elmx/releases/tag/1.0.0

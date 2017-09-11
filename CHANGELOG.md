# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

- Removed GitReleaseManager from the build script
- Adopted keepachangelog.com
- The glob pattern implementation now relies on pure regex instead of relying on external NuGet packages

## [0.4.1] - 2017-08-24

### Fixed

- The 0.4.0 packages were unlisted because they were missing a library

### Changed

- [#19](https://github.com/fwinkelbauer/Bumpy/issues/19) **Breaking Change** Glob pattern: The re-introduction of glob patterns might break existing `.bumpyconfig` files. Please read the updated documentation!
- [#17](https://github.com/fwinkelbauer/Bumpy/issues/17) Automate Chocolatey verification

## [0.3.0] - 2017-07-17

### Fixed

- [#18](https://github.com/fwinkelbauer/Bumpy/issues/18) Bug when parsing configuration files with the '=' character in a regular expression

### Added

- [#14](https://github.com/fwinkelbauer/Bumpy/issues/14) Automate GitHub releases

## [0.2.1] - 2017-06-28

### Added

- [#10](https://github.com/fwinkelbauer/Bumpy/issues/10) Add missing information to the help command
- [#12](https://github.com/fwinkelbauer/Bumpy/issues/12) Add automation steps to publish packages

## [0.2.0] - 2017-06-27

### Added

- [#12](https://github.com/fwinkelbauer/Bumpy/issues/2) Support other version formats
- [#16](https://github.com/fwinkelbauer/Bumpy/issues/6) Add encoding configuration 

## [0.1.0] - 2017-06-19

### Added

- Initial release

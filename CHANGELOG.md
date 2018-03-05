# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## Unreleased

### Changed

- **Breaking Change:** The default configuration file to `bumpy.yaml`. The old format `.bumpyconfig` is still supported, but a warning is printed

## 0.10.0 - 2018-02-26

### Added

- [#36](https://github.com/fwinkelbauer/Bumpy/issues/36) Ensure command
- [#35](https://github.com/fwinkelbauer/Bumpy/issues/35) Templates for `.bumpyconfig` files
- [#34](https://github.com/fwinkelbauer/Bumpy/issues/34) Regex capture group "marker"
- [#31](https://github.com/fwinkelbauer/Bumpy/issues/31) "No-operation" flag which prevents commands such as `bumpy increment` to persist changes
- [#28](https://github.com/fwinkelbauer/Bumpy/issues/28) Support for versions which use comma as a delimiter (e.g. Visual Studio C++ resource files contain versions such as "1,0,0,0")

### Changed

- [#32](https://github.com/fwinkelbauer/Bumpy/issues/32) Bumpy now highlights files with no version information

## 0.9.0 - 2018-02-09

### Added

- [#29](https://github.com/fwinkelbauer/Bumpy/issues/29) Support to change the postfix text of a version (e.g. "-beta001")

## 0.8.0 - 2018-02-06

### Added

- [#26](https://github.com/fwinkelbauer/Bumpy/issues/26) Support for version formats with leading zeros (e.g. increment "2018.01.01" to "2018.01.02" instead of "2018.1.2")

### Changed

- The glob implementation. My current approach relies on regex and is not fully compliant to any "glob standard". I might switch to an open source glob library if users start to have issues

### Fixed

- A off-by-one bug when printing the lines of found versions
- Some bugs which caused unnecessary regex and file write operations

## 0.7.0 - 2017-12-23

### Added

- [#23](https://github.com/fwinkelbauer/Bumpy/issues/23) Support for code pages in `.bumpyconfig`
- [#24](https://github.com/fwinkelbauer/Bumpy/issues/24) New command incrementonly. Thank you [@euronay](https://github.com/euronay) for your contribution!

### Fixed

- [#22](https://github.com/fwinkelbauer/Bumpy/issues/22) A case in which `bumpy write` would perform in an inconsistent manner

## 0.6.0 - 2017-10-18

### Changed

- **Breaking Change:** The command line interface to be similar to tools such as git or chocolatey. This change also introduces new options. See the updated `README` file for more information

### Removed

- **Breaking Change:** The command to list profiles. Profiles are now shown when calling `bumpy list`

## 0.5.0 - 2017-09-20

### Removed

- GitReleaseManager from the build script

### Changed

- Adopted keepachangelog.com
- The glob pattern implementation now relies on pure regex instead of relying on external NuGet packages
- The pattern "\*\*\Foo.txt" will:
  - match files named "Foo.txt"
  - not match other files such as "SomeFoo.txt"

## 0.4.1 - 2017-08-24

### Fixed

- The 0.4.0 packages were unlisted because they were missing a library

### Changed

- [#19](https://github.com/fwinkelbauer/Bumpy/issues/19) **Breaking Change** Glob pattern: The re-introduction of glob patterns might break existing `.bumpyconfig` files. Please read the updated documentation!
- [#17](https://github.com/fwinkelbauer/Bumpy/issues/17) Automation process to perform Chocolatey verification

## 0.3.0 - 2017-07-17

### Fixed

- [#18](https://github.com/fwinkelbauer/Bumpy/issues/18) A bug when parsing configuration files with the '=' character in a regular expression

### Added

- [#14](https://github.com/fwinkelbauer/Bumpy/issues/14) GitHub releases automation

## 0.2.1 - 2017-06-28

### Added

- [#10](https://github.com/fwinkelbauer/Bumpy/issues/10) Missing information to the help command
- [#12](https://github.com/fwinkelbauer/Bumpy/issues/12) Automation steps to publish packages

## 0.2.0 - 2017-06-27

### Added

- [#12](https://github.com/fwinkelbauer/Bumpy/issues/2) Support other version formats
- [#16](https://github.com/fwinkelbauer/Bumpy/issues/6) Encoding configuration 

## 0.1.0 - 2017-06-19

### Added

- Initial release

# Bumpy

Bumpy is a tool to manipulate version information across multiple files found in the current working directory using a configuration file which consists of [glob patterns](https://en.wikipedia.org/wiki/Glob_(programming)) and regular expressions.

NuGet and Chocolatey packages can be found [here](https://www.nuget.org/packages/Bumpy/) and [here](https://chocolatey.org/packages/bumpy.portable).

**Note:** As Bumpy's behaviour is heavily influenced by the provided configuration (see below), make sure that your files are kept under version control so that you can easily verify Bumpy's results.

## Why?

Most of the build systems that I have seen or worked with in the past create a workflow in which they utilize a single source
(e.g. a file called `version.txt`, or a tool such as [GitVersion](https://github.com/GitTools/GitVersion)) to inject version information into a set of files (`AssemblyInfo.cs`, `*.csproj`, `*.nuspec`, `*.xml`, ...).
These files  might contain "blank versions" (`0.0.0`, `0.0.0.0`) which are only ever changed in memory while a build is active. In my opinion such processes require too much magic, even though they work for a lot of people.
Bumpy was born because I wanted a simple tool that I can use in combination with existing source control management systems to handle all my versioning requirements in a project.

## Usage & Examples

Bumpy is a command line tool:

```
bumpy <command> <arguments> <options>
```

Check out the `.bumpyconfig` file [here](https://github.com/fwinkelbauer/Bumpy/blob/master/.bumpyconfig) to see how the following examples were created directly using this repository:

### List

```
bumpy list
```

Lists all versions and their profiles (see below to learn more about profiles).

**Example:** `bumpy list`

```
[assembly]
Bumpy\Properties\AssemblyInfo.cs (16): 0.8.0.0
Bumpy\Properties\AssemblyInfo.cs (17): 0.8.0.0
[semver]
NuSpec\Chocolatey\Bumpy.Portable.nuspec (6): 0.8.0
NuSpec\NuGet\Bumpy.nuspec (6): 0.8.0
```

### Create New Configuration

```
bumpy new
```

Creates an empty `.bumpyconfig` file.

### Increment

```
bumpy increment <one-based index number>
```

Increments the specified component of each version.

**Example:** `bumpy increment 3`

```
Bumpy\Properties\AssemblyInfo.cs (16): 0.8.0.0 -> 0.8.1.0
Bumpy\Properties\AssemblyInfo.cs (17): 0.8.0.0 -> 0.8.1.0
NuSpec\Chocolatey\Bumpy.Portable.nuspec (6): 0.8.0 -> 0.8.1
NuSpec\NuGet\Bumpy.nuspec (6): 0.8.0 -> 0.8.1
```

### Incrementonly

```
bumpy incrementonly <one-based index number>
```

Increments the specified component of each version, without updating following components.

**Example:** `bumpy incrementonly 2`

```
Bumpy\Properties\AssemblyInfo.cs (16): 0.8.1.0 -> 0.9.1.0
Bumpy\Properties\AssemblyInfo.cs (17): 0.8.1.0 -> 0.9.1.0
NuSpec\Chocolatey\Bumpy.Portable.nuspec (6): 0.8.1 -> 0.9.1
NuSpec\NuGet\Bumpy.nuspec (6): 0.8.1 -> 0.9.1
```

### Write

```
bumpy write <version string>
```

Overwrites a version with another version.

This command could be used to e.g:

- Unify the version information of projects and files in a solution
- Change the version information of a newly created project to be in line with other projects in a solution

**Example:** `bumpy write 1.0.0.0 -p assembly`

```
Bumpy\Properties\AssemblyInfo.cs (16): 0.9.1.0 -> 1.0.0.0
Bumpy\Properties\AssemblyInfo.cs (17): 0.9.1.0 -> 1.0.0.0
```

**Example:** `bumpy write 1.0.0 -p semver`

```
NuSpec\Chocolatey\Bumpy.Portable.nuspec (6): 0.9.1 -> 1.0.0
NuSpec\NuGet\Bumpy.nuspec (6): 0.9.1 -> 1.0.0
```

### Assign

```
bumpy assign <one-based index number> <version number>
```

Replaces the specified component of a version with a new number. This command could for example be used by a CI server to add the current build number.

**Example:** `bumpy assign 3 42`

```
Bumpy\Properties\AssemblyInfo.cs (16): 1.0.0.0 -> 1.0.42.0
Bumpy\Properties\AssemblyInfo.cs (17): 1.0.0.0 -> 1.0.42.0
NuSpec\Chocolatey\Bumpy.Portable.nuspec (6): 1.0.0 -> 1.0.42
NuSpec\NuGet\Bumpy.nuspec (6): 1.0.0 -> 1.0.42
```

### Help

```
bumpy help
```

Shows all available commands and options.

### Options

The following options are available for the commands `list`, `increment`, `incrementonly`, `write` and `assign`:

- `-p <profile name>`
- `-d <directory in which Bumpy should operate>`
- `-c <path to a configuration file Bumpy should use>`

**Examples:**

```
bumpy write 1.15.0-beta -p semver

# In this example Bumpy will still expect a configuration file to be present in the current working directory
bumpy list -d ..\some_other_project

# Using this command Bumpy will only run in the specified folder (configuration loading + execution)
bumpy list -d D:\my_project -c D:\my_project\.bumpyconfig
```

## Configuration

Bumpy's configuration is based on the presence of a `.bumpyconfig` file in the current working directory. This file dictates the behaviour of Bumpy using a pair of glob patterns and regular expressions, e.g:

```
# Usage: <file glob pattern> = <regular expression>

# Example: Search for all .nuspec files in the NuSpec directory
NuSpec\**\*.nuspec = <version>(?<version>\d+(\.\d+)+)

# Example: The default read/write encoding is UTF-8 without BOM, but you can change this behaviour (e.g. UTF-8 with BOM)
AssemblyInfo.cs | UTF-8 = (?<version>\d+\.\d+\.\d+\.\d+)
```

For each line of a specific file (found through the glob pattern) Bumpy uses the provided regular expression to extract the named regex group `?<version>`.
These regex groups can contain versions in different formats. Bumpy can currently handle formats such as:

- `\d+(\.\d+)*` (meaning versions such as 1, 1.0, 1.0.0, 1.0.0.0, ...)
- [SemVer](http://semver.org/) (1.8.0-beta01, 1.0.0-alpha+001, ...)

Type `bumpy new` to create a new configuration file. This file contains additional information about configuration possibilities (e.g. how to change the read/write encoding).

### Profiles

Lines in a `.bumpyconfig` file can be organized using profiles ("groups"):

```
[my_profile]
*.txt = ...
```

Most of Bumpy's commands can be applied to a certain profile by specifing the profile name, e.g. `bumpy list -p my_profile`. This feature can be useful if you need to target a specific set of files in isolation (e.g. a `AssemblyInfo.cs` file in C# can only deal with versions of the format `1.0.0.0`, while a `.nuspec` file could contain textual elements such as `1.0.0-beta`).

## Trivia

- The name Bumpy is loosely based on the phrase "to bump something up" instead of the original meaning of the word (e.g. "a bumpy road")
- Inspiration taken from [Zero29](https://github.com/ploeh/ZeroToNine)

## License

[MIT](http://opensource.org/licenses/MIT)

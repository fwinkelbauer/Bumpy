# Bumpy

Bumpy is a tool to manipulate version information across multiple files found in the current working directory using a `.bumpyconfig` file which consists of [glob patterns](https://en.wikipedia.org/wiki/Glob_(programming)) and regular expressions.
I am using Bumpy to handle C# projects, but the tool can be configured for any set of files.

NuGet and Chocolatey packages can be found [here](https://www.nuget.org/packages/Bumpy/) and [here](https://chocolatey.org/packages/bumpy.portable). A Cake Addin is provided [here](https://www.nuget.org/packages/Cake.Bumpy/).

**Note:** As Bumpy's behaviour is heavily influenced by your `.bumpyconfig` file, make sure that your files are kept under version control so that you can easily verify Bumpy's results.

## Why?

Most of the build systems that I have seen or worked with in the past create a workflow in which they utilize a single source
(e.g. a file called `version.txt`, or a tool such as [GitVersion](https://github.com/GitTools/GitVersion)) to inject version information into a set of files (`AssemblyInfo.cs`, `*.csproj`, `*.nuspec`, `*.xml`, ...).
These files  might contain "blank versions" (`0.0.0`, `0.0.0.0`) which are only ever changed in memory while a build is active. I prefer to persist all file changes so that I can track the history in any source control management system.
Bumpy was born because I wanted a simple tool that I can use to update version information across several files in one operation.

## Getting Started

Using Bumpy in a .NET project is rather easy:

- Download the Bumpy NuGet package (or install it via Chocolatey - `choco install bumpy.portable`)
- Make sure that the `<version>` XML element exists in the `*.csproj` file (.NET Standard or .NET Core)
- Type `bumpy new` in the Package Manager Console in Visual Studio

Afterwards you will find a `.bumpyconfig` file in your solution. Type `bumpy list` to see an output similar to this:

```
[assembly]
ConsoleApp1\Properties\AssemblyInfo.cs (AssemblyVersion): 1.0.0.0
ConsoleApp1\Properties\AssemblyInfo.cs (AssemblyFileVersion): 1.0.0.0
UnitTestProject1\Properties\AssemblyInfo.cs (AssemblyVersion): 1.0.0.0
UnitTestProject1\Properties\AssemblyInfo.cs (AssemblyFileVersion): 1.0.0.0
```

Now you can use Bumpy to change all versions in one operation, e.g. `bumpy increment 4`:

```
[assembly]
ConsoleApp1\Properties\AssemblyInfo.cs (AssemblyVersion): 1.0.0.0 -> 1.0.0.1
ConsoleApp1\Properties\AssemblyInfo.cs (AssemblyFileVersion): 1.0.0.0 -> 1.0.0.1
UnitTestProject1\Properties\AssemblyInfo.cs (AssemblyVersion): 1.0.0.0 -> 1.0.0.1
UnitTestProject1\Properties\AssemblyInfo.cs (AssemblyFileVersion): 1.0.0.0 -> 1.0.0.1
```

Check out the documentation below to learn more about Bumpy's commands and how you can configure Bumpy according to your needs.

## Usage & Examples

Bumpy is a command line tool:

```
bumpy <command> <arguments> <options>
```

Have a look at the file `Source\.bumpyconfig` to see how the following examples were created using this repository:

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
[nuspec]
NuSpec\Chocolatey\Bumpy.Portable.nuspec (6): 0.8.0
NuSpec\NuGet\Bumpy.nuspec (6): 0.8.0
```

### Create New Configuration

```
bumpy new
```

Creates a `.bumpyconfig` file if it does not exist.

### Increment

```
bumpy increment <one-based index number>
```

Increments the specified component of each version.

**Example:** `bumpy increment 3`

```
[assembly]
Bumpy\Properties\AssemblyInfo.cs (16): 0.8.0.5 -> 0.8.1.0
Bumpy\Properties\AssemblyInfo.cs (17): 0.8.0.5 -> 0.8.1.0
[nuspec]
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
[assembly]
Bumpy\Properties\AssemblyInfo.cs (16): 0.8.1.0 -> 0.9.1.0
Bumpy\Properties\AssemblyInfo.cs (17): 0.8.1.0 -> 0.9.1.0
[nuspec]
NuSpec\Chocolatey\Bumpy.Portable.nuspec (6): 0.8.1 -> 0.9.1
NuSpec\NuGet\Bumpy.nuspec (6): 0.8.1 -> 0.9.1
```

### Write

```
bumpy write <version string>
```

Overwrites a version with another version.

This command could be used to:

- Unify the version information of projects and files in a solution
- Change the version information of a newly created project to be in line with other projects in a solution

**Example:** `bumpy write 1.0.0.0 -p assembly`

```
[assembly]
Bumpy\Properties\AssemblyInfo.cs (16): 0.9.1.0 -> 1.0.0.0
Bumpy\Properties\AssemblyInfo.cs (17): 0.9.1.0 -> 1.0.0.0
```

**Example:** `bumpy write 1.0.0 -p nuspec`

```
[nuspec]
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
[assembly]
Bumpy\Properties\AssemblyInfo.cs (16): 1.0.0.0 -> 1.0.42.0
Bumpy\Properties\AssemblyInfo.cs (17): 1.0.0.0 -> 1.0.42.0
[nuspec]
NuSpec\Chocolatey\Bumpy.Portable.nuspec (6): 1.0.0 -> 1.0.42
NuSpec\NuGet\Bumpy.nuspec (6): 1.0.0 -> 1.0.42
```

### Label

```
bumpy label <suffix version text>
```

Replaces the suffix text of a version.

**Example:** `bumpy label "-beta" -p nuspec`

```
[nuspec]
NuSpec\Chocolatey\Bumpy.Portable.nuspec (6): 0.8.0 -> 0.8.0-beta
NuSpec\NuGet\Bumpy.nuspec (6): 0.8.0 -> 0.8.0-beta
```

Or:

```
[nuspec]
NuSpec\Chocolatey\Bumpy.Portable.nuspec (6): 0.8.0-alpha -> 0.8.0-beta
NuSpec\NuGet\Bumpy.nuspec (6): 0.8.0-alpha -> 0.8.0-beta
```

**Example:** `bumpy label "" -p nuspec`

```
[nuspec]
NuSpec\Chocolatey\Bumpy.Portable.nuspec (6): 0.8.0-beta -> 0.8.0
NuSpec\NuGet\Bumpy.nuspec (6): 0.8.0-beta -> 0.8.0
```

### Ensure

```
bumpy ensure
```

Checks that all versions in a profile are equal. This command can be used in a build pipeline to enforce consistency of version numbers.

**Eample:** `bumpy list`

```
[assembly]
Bumpy\Properties\AssemblyInfo.cs (16): 0.8.0.0
Bumpy\Properties\AssemblyInfo.cs (17): 0.8.0.0
[nuspec]
NuSpec\Chocolatey\Bumpy.Portable.nuspec (6): 0.8.0
NuSpec\NuGet\Bumpy.nuspec (6): 0.8.0
```

Followed by:

`bumpy ensure`

Gives us:

```
assembly: 0.8.0.0
nuspec: 0.8.0
```

If we change one version in `AssemblyInfo.cs` to `0.9.0.0`:

`bumpy ensure`

Gives us:

```
Error: Found different versions in profile 'assembly': 0.8.0.0, 0.9.0.0.
```

### Help

```
bumpy help
```

Shows all available commands and options.

### Options

The following options are available for the commands `list`, `increment`, `incrementonly`, `write`, `assign` and `label`:

- Use a profile: `-p <profile name>`
- Change Bumpy's working directory: `-d <directory in which Bumpy should operate>`
- Specify a configuration file: `-c <path to a configuration file Bumpy should use>`
- Use "no-operation" (preview) mode: `-n`

**Examples:**

```
bumpy write 1.15.0-beta -p nuspec

# In this example Bumpy will still expect a configuration file to be present in the current working directory
bumpy list -d ..\some_other_project

# Using this command Bumpy will only run in the specified folder (configuration loading + execution)
bumpy list -d D:\my_project -c D:\my_project\.bumpyconfig

# Bumpy will show a preview of the increment command (without changing a file on disk)
bumpy increment 1 -n
```

## Configuration

Bumpy's configuration is based on the presence of a `.bumpyconfig` file in the current working directory. This file dictates the behaviour of Bumpy using a pair of glob patterns and regular expressions, e.g:

```
# Example: Search for all .nuspec files in the NuSpec directory
[NuSpec\**\*.nuspec]
regex = <version>(?<version>\d+(\.\d+)+)

# Example: The default read/write encoding is UTF-8 without BOM, but you can change this behaviour (e.g. UTF-8 with BOM)
[AssemblyInfo.cs]
encoding = UTF-8
regex = (?<version>\d+\.\d+\.\d+\.\d+)

# Example: Encodings can also be defined through code pages. This can be handy for Visual Studio C++ projects
[MyProject.rc]
encoding = 1200
regex = \d+\.\d+\.\d+\.\d+
```

For each line of a specific file (found through a glob pattern) Bumpy uses the provided regular expression to extract the named capture group `?<version>`.
These groups can contain versions in different formats. Bumpy can currently handle formats such as:

- `\d+(\.\d+)*` (meaning versions such as `1`, `1.0`, `1.0.0`, `1.0.0.0`, ...)
- `\d+(,\d+)*` (`1,0,0,0`)
- [SemVer](http://semver.org/) (`1.8.0-beta`, `1.0.0-alpha+001`, ...)

### Profiles

Projects might contain different categories of versions. In a typical C# project you might find two categories:

- Assembly versions (e.g. `1.0.0.0`, found in `AssemblyInfo.cs` files)
- Semantic versions (e.g. `1.0.0-beta` found in `*.nuspec` files)

These categories can be organized in profiles:

```
[*.my_glob | my_profile]
```

Most of Bumpy's commands can be applied to a certain profile by specifing the profile name, e.g. `bumpy list -p my_profile`.

### Marker

Bumpy will per default print the line number for each version found in a file:

```
MyProject.nuspec (6): 1.0.0
```

This behaviour can be changed using the named capture group `?<marker>`. A `.bumpyconfig` like this:

```
[*.nuspec]
regex = <(?<marker>version)>(?<version>\d+(\.\d+)+)
```

Would change the output of `bumpy list` to something like this:

```
MyProject.nuspec (version): 1.0.0
```

### Templates

Templates can be used to simplify a `.bumpyconfig` file for known file types. Currently `.nuspec`, `AssemblyInfo.cs` and `.csproj` files are supported.

```
# These glob patterns will inherit predefined encodings and basic regular expressions:
[*.nuspec | nuspec]

[MyDotNetCoreProject/MyDotNetCoreProject.csproj | nuspec]

[MyDotNetFrameworkProject/**/AssemblyInfo.cs | assembly]
```

### Best Practices

- Commit your `.bumpyconfig` file in whatever source code management system that you are using
- Use one profile per version type so that you can use `bumpy ensure` to enforce version consistency between files. Don't put a file containing `1.0.0.0` in the same profile as a file containing `1.0.0` or `1.0.0-rc2`

## Trivia

- The name Bumpy is loosely based on the phrase "to bump something up" instead of the original meaning of the word (e.g. "a bumpy road")
- Inspiration taken from [Zero29](https://github.com/ploeh/ZeroToNine)

## License

[MIT](http://opensource.org/licenses/MIT)

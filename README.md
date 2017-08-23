# Bumpy

Bumpy is a tool to manipulate version information across multiple files found in the current working directory using a configuration file which consists of [glob patterns](https://github.com/kthompson/glob/) and regular expressions.

NuGet and Chocolatey packages can be found [here](https://www.nuget.org/packages/Bumpy/) and [here](https://chocolatey.org/packages/bumpy.portable).

**Note:** As Bumpy's behaviour is heavily influenced by the provided configuration (see below), make sure that your files are kept under version control so that you can easily verify Bumpy's results.

## Why?

Most of the build systems that I have seen in the past create a workflow in which they utilize a single source (e.g. a file called `version.txt`) to inject version information into a set of files (`AssemblyInfo.cs`, `*.csproj`, `*.nuspec`, `*.xml`, `*.yaml`, ...).
This would also mean that some files would contain a "blank version" (such as 0.0.0.0) which would remind developers that they should not touch that file manually. Even though this is a valid solution, it just wasn't for me. That's why Bumpy was born.

## Usage & Examples

Bumpy is a command line tool:

```
bumpy <command> <arguments>
```

Check out the `.bumpyconfig` file [here](https://github.com/fwinkelbauer/Bumpy/blob/master/.bumpyconfig) to see how the following examples were created directly using this repository:

### List

```
bumpy -l
```

Lists all versions.

**Example:** `bumpy -l`

```
\Source\Bumpy\Properties\AssemblyInfo.cs (35): 0.2.1.0
\Source\Bumpy\Properties\AssemblyInfo.cs (36): 0.2.1.0
\NuSpec\Chocolatey\Bumpy.Portable.nuspec (5): 0.2.1
\NuSpec\NuGet\Bumpy.nuspec (5): 0.2.1
```

### Profiles

```
bumpy -p
```

Shows all profiles defined in the configuration (see below to learn more about profiles).

**Example:** `bumpy -p`

```
assembly
semver
```

### Create Configuration

```
bumpy -c
```

Creates an empty `.bumpyconfig` file.

### Increment

```
bumpy -i <one-based index number>
```

Increments the specified component of each version.

**Example:** `bumpy -i 2`

```
\Source\Bumpy\Properties\AssemblyInfo.cs (35): 0.2.1.0 -> 0.3.0.0
\Source\Bumpy\Properties\AssemblyInfo.cs (36): 0.2.1.0 -> 0.3.0.0
\NuSpec\Chocolatey\Bumpy.Portable.nuspec (5): 0.2.1 -> 0.3.0
\NuSpec\NuGet\Bumpy.nuspec (5): 0.2.1 -> 0.3.0
```

### Write

```
bumpy -w <version string>
```

Overwrites a version with another version.

This command could be used to e.g:

- Unify the version information of projects and files in a solution
- Change the version information of a newly created project to be in line with other projects in a solution

**Example:** `bumpy -w 1.2.5.0`

```
\Source\Bumpy\Properties\AssemblyInfo.cs (35): 0.3.0.0 -> 1.2.5.0
\Source\Bumpy\Properties\AssemblyInfo.cs (36): 0.3.0.0 -> 1.2.5.0
\NuSpec\Chocolatey\Bumpy.Portable.nuspec (5): 0.3.0 -> 1.2.5.0
\NuSpec\NuGet\Bumpy.nuspec (5): 0.3.0 -> 1.2.5.0
```

**Example:** `bumpy nuspec -w 1.2.5`

```
\NuSpec\Chocolatey\Bumpy.Portable.nuspec (5): 1.2.5.0 -> 1.2.5
\NuSpec\NuGet\Bumpy.nuspec (5): 1.2.5.0 -> 1.2.5
```

### Assign

```
bumpy -a <one-based index number> <version number>
```

Replaces the specified component of a version with a new number. This command could be used by a CI server to inject a build number.

**Example:** `bumpy -a 3 99`

```
\Source\Bumpy\Properties\AssemblyInfo.cs (35): 1.2.5.0 -> 1.2.99.0
\Source\Bumpy\Properties\AssemblyInfo.cs (36): 1.2.5.0 -> 1.2.99.0
\NuSpec\Chocolatey\Bumpy.Portable.nuspec (5): 1.2.5.0 -> 1.2.99.0
\NuSpec\NuGet\Bumpy.nuspec (5): 1.2.5 -> 1.2.99
```

## Configuration

Bumpy's configuration is based on the presence of a `.bumpyconfig` file in the current working directory. This file dictates the behaviour of Bumpy using a pair of [glob patterns](https://github.com/kthompson/glob/) and regular expressions, e.g:

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

Type `bumpy -c` to create a new configuration file. This file contains additional information about configuration possibilities (e.g. how to change the read/write encoding).

### Profiles

The lines in a `.bumpyconfig` file can be organized using profiles ("groups"):

```
[my_profile]
*.txt = ...
```

Most of Bumpy's commands can be applied to a certain profile by specifing the profile name, e.g. `bumpy my_profile -l`. This feature can be useful if you need to target a specific set of files in isolation (e.g. a `AssemblyInfo.cs` file in C# can only deal with versions of the format `1.0.0.0`, while a `.nuspec` file could contain textual elements such as `1.0.0-beta`).

## Trivia

- The name Bumpy is loosely based on the phrase "to bump something up" instead of the original meaning of the word (e.g. "a bumpy road")
- Inspiration taken from [Zero29](https://github.com/ploeh/ZeroToNine)

## License

[MIT](http://opensource.org/licenses/MIT)

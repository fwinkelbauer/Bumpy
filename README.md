# Bumpy

Bumpy is a tool to maintain version information across multiple files found in the current working directory using a configuration file which consists of glob patterns and regular expressions.

## Usage

Bumpy is a command line utility:

```
bumpy <command> <arguments>
```

### List

```
bumpy -l
```

Lists all versions.

**Example:** `bumpy -l`

```
\Source\HelloWorldApplication\HelloWorldApplication\Properties\AssemblyInfo.cs (33): 1.0.0.0
\Source\HelloWorldApplication\HelloWorldApplication\Properties\AssemblyInfo.cs (34): 1.0.0.0
\NuSpec\HelloWorldApplication\SomeProject.nuspec (4): 1.0.0.0
```

### Increment

```
bumpy -i <zero-based index number>
```

Increments the specified component of each version.

**Example:** `bumpy -i 1`

```
\Source\HelloWorldApplication\HelloWorldApplication\Properties\AssemblyInfo.cs (33): 1.0.0.0 -> 1.1.0.0
\Source\HelloWorldApplication\HelloWorldApplication\Properties\AssemblyInfo.cs (34): 1.0.0.0 -> 1.1.0.0
\NuSpec\HelloWorldApplication\SomeProject.nuspec (4): 1.0.0.0 -> 1.1.0.0
```

### Write

```
bumpy -w <version string>
```

Overwrites a version with another version.

This command could used to e.g:

- Unify the version information of projects and files in a solution
- Change the version information of a newly created project to be in line with other projects in a solution

**Example:** `bumpy -w 1.2.0.5`

```
\Source\HelloWorldApplication\HelloWorldApplication\Properties\AssemblyInfo.cs (33): 1.0.0.0 -> 1.2.0.5
\Source\HelloWorldApplication\HelloWorldApplication\Properties\AssemblyInfo.cs (34): 1.0.0.0 -> 1.2.0.5
\NuSpec\HelloWorldApplication\SomeProject.nuspec (4): 1.0.0.0 -> 1.2.0.5
```

### Assign

```
bumpy -a <zero-based index number> <version number>
```

Replaces the specified component of a version with a new number. This command could be used by a CI server to inject a build number.

**Example:** `bumpy -a 2 99`

```
\Source\HelloWorldApplication\HelloWorldApplication\Properties\AssemblyInfo.cs (33): 1.0.0.0 -> 1.0.99.0
\Source\HelloWorldApplication\HelloWorldApplication\Properties\AssemblyInfo.cs (34): 1.0.0.0 -> 1.0.99.0
\NuSpec\HelloWorldApplication\SomeProject.nuspec (4): 1.0.0.0 -> 1.0.99.0
```

## Example

The `Example` folder contains a C# solution (which contains a `AssemblyInfo.cs` file) and a `HelloWorldApplication.nuspec` file with version `1.0.0.0`. Through the `.bumpyconfig` file we can manipulate all versions with one command line. Try it yourself!

## Configuration

Bumpy's configuration is based on the presence of a `.bumpyconfig` file in the current working directory. This file dictates the behaviour of Bumpy using a pair of glob patterns and regex expressions, e.g:

```
# Searches for a version of the format a.b.c.d in all .nuspec files in the folder NuSpec
NuSpec\**\*.nuspec = (?<version>\d+\.\d+\.\d+\.\d+)
```

For each line of a specific file (found through the glob pattern) Bumpy uses the provided regular expression to extract the named regex group `?<version>`. This extracted information is used for all other functions. Note though that internally Bumpy currenlty only supports version text of the format `\d+(\.\d+)*` (meaning `1`, `1.0`, `1.0.0`, `1.0.0.0`, ...).

## Trivia

- The name Bumpy is loosely based on "to bump something up" instead of the original meaning of the word (as in "a bumpy road"
- Inspiration taken from [Zero29](https://github.com/ploeh/ZeroToNine)

## License

[MIT](http://opensource.org/licenses/MIT)

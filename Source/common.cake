#tool vswhere

private const string ArtifactsDirectory = "../Artifacts";

void CleanArtifacts()
{
    CleanDirectory(ArtifactsDirectory);
}

void StoreChocolatey(FilePath nuspec)
{
    var dir = $"{ArtifactsDirectory}/Chocolatey";
    EnsureDirectoryExists(dir);
    Information("Creating Chocolatey package in directory {0}", dir);
    ChocolateyPack(nuspec, new ChocolateyPackSettings { OutputDirectory = dir });
}

void StoreNuGet(FilePath nuspec)
{
    var dir = $"{ArtifactsDirectory}/NuGet";
    EnsureDirectoryExists(dir);
    Information("Creating NuGet package in directory {0}", dir);
    NuGetPack(nuspec, new NuGetPackSettings { OutputDirectory = dir });
}

void StoreArtifacts(string projectName, string includePattern)
{
    StoreArtifacts(projectName, GetFiles(includePattern));
}

void StoreArtifacts(string projectName, IEnumerable<FilePath> files)
{
    var dir = $"{ArtifactsDirectory}/{projectName}";
    EnsureDirectoryExists(dir);

    Information("Copying artifacts to {0}", dir);
    CopyFiles(files, dir);
    DeleteFiles($"{dir}/*.pdb");
    DeleteFiles($"{dir}/*.lastcodeanalysissucceeded");
    DeleteFiles($"{dir}/*.CodeAnalysisLog.xml");
}

void MSTest2_VS2017(string pattern)
{
    MSTest2_VS2017(GetFiles(pattern));
}

void MSTest2_VS2017(IEnumerable<FilePath> assemblyPaths)
{
    VSTest(assemblyPaths, FixToolPath(new VSTestSettings { Logger = "trx", TestAdapterPath = "." }));
}

// https://github.com/cake-build/cake/issues/1522
VSTestSettings FixToolPath(VSTestSettings settings)
{
    settings.ToolPath =
        VSWhereLatest(new VSWhereLatestSettings { Requires = "Microsoft.VisualStudio.PackageGroup.TestTools.Core" })
        .CombineWithFilePath(File(@"Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"));
    return settings;
}

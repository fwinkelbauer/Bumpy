public DirectoryPath RepositoryHome = ".";

void CleanArtifacts()
{
    CleanDirectory($"{RepositoryHome}/Artifacts");
}

void StoreBuildArtifacts(string projectName, string filePattern)
{
    StoreBuildArtifacts(projectName, GetFiles(filePattern));
}

void StoreBuildArtifacts(string projectName, IEnumerable<FilePath> filePaths)
{
    var dir = $"{RepositoryHome}/Artifacts/Build/{projectName}";

    Information("Copying artifacts to {0}", dir);
    EnsureDirectoryExists(dir);
    CopyFiles(filePaths, dir, true);
}

FilePath GetChocolateyArtifact(string packageId)
{
    return GetFiles($"{RepositoryHome}/Artifacts/Chocolatey/{packageId}/*.nupkg").First();
}

void PublishChocolateyArtifact(string packageId, string pushSource)
{
    ChocolateyPush(GetChocolateyArtifact(packageId), new ChocolateyPushSettings { Source = pushSource });
}

void PackChocolateyArtifacts(string nuspecPathPattern)
{
    foreach (var nuspecPath in GetFiles(nuspecPathPattern))
    {
        PackChocolateyArtifact(nuspecPath);
    }
}

void PackChocolateyArtifact(FilePath nuspecPath)
{
    var dir = $"{RepositoryHome}/Artifacts/Chocolatey/{nuspecPath.GetFilenameWithoutExtension()}";

    EnsureDirectoryExists(dir);
    ChocolateyPack(nuspecPath, new ChocolateyPackSettings { OutputDirectory = dir });
}

FilePath GetNuGetArtifact(string packageId)
{
    return GetFiles($"{RepositoryHome}/Artifacts/NuGet/{packageId}/*.nupkg").First();
}

void PublishNuGetArtifact(string packageId, string pushSource)
{
    var files = GetNuGetArtifact(packageId);

    NuGetPush(files, new NuGetPushSettings { Source = pushSource });
}

void PackNuGetArtifacts(string nuspecPathPattern)
{
    foreach (var nuspecPath in GetFiles(nuspecPathPattern))
    {
        PackNuGetArtifact(nuspecPath);
    }
}

void PackNuGetArtifact(FilePath nuspecPath)
{
    var dir = $"{RepositoryHome}/Artifacts/NuGet/{nuspecPath.GetFilenameWithoutExtension()}";

    EnsureDirectoryExists(dir);
    NuGetPack(nuspecPath, new NuGetPackSettings { OutputDirectory = dir });
}

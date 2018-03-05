public DirectoryPath ArtifactsDirectory = "Artifacts";

void CleanArtifacts()
{
    CleanDirectory($"{ArtifactsDirectory}");
}

void StoreBuildArtifacts(string projectName, string filePattern)
{
    StoreBuildArtifacts(projectName, GetFiles(filePattern));
}

void StoreBuildArtifacts(string projectName, IEnumerable<FilePath> filePaths)
{
    var dir = $"{ArtifactsDirectory}/Build/{projectName}";
    EnsureDirectoryExists(dir);

    Information("Copying artifacts to {0}", dir);
    CopyFiles(filePaths, dir, true);
}

FilePath GetChocolateyArtifact(string packageId)
{
    return GetFiles($"{ArtifactsDirectory}/Chocolatey/{packageId}/*.nupkg").First();
}

void PublishChocolateyArtifact(string packageId, string pushSource)
{
    ChocolateyPush(GetChocolateyArtifact(packageId), new ChocolateyPushSettings { Source = pushSource });
}

void StoreChocolateyArtifact(FilePath nuspecPath)
{
    var dir = $"{ArtifactsDirectory}/Chocolatey/{nuspecPath.GetFilenameWithoutExtension()}";
    EnsureDirectoryExists(dir);
    ChocolateyPack(nuspecPath, new ChocolateyPackSettings { OutputDirectory = dir });
}

FilePath GetNuGetArtifact(string packageId)
{
    return GetFiles($"{ArtifactsDirectory}/NuGet/{packageId}/*.nupkg").First();
}

void PublishNuGetArtifact(string packageId, string pushSource)
{
    var files = GetNuGetArtifact(packageId);
    NuGetPush(files, new NuGetPushSettings { Source = pushSource });
}

void StoreNuGetArtifact(FilePath nuspecPath)
{
    var dir = $"{ArtifactsDirectory}/NuGet/{nuspecPath.GetFilenameWithoutExtension()}";
    EnsureDirectoryExists(dir);
    NuGetPack(nuspecPath, new NuGetPackSettings { OutputDirectory = dir });
}

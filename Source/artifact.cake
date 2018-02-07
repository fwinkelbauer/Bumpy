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

void PublishChocolateyArtifact(string packageId, string pushSource)
{
    var files = GetFiles($"{ArtifactsDirectory}/Chocolatey/{packageId}/*.nupkg");
    ChocolateyPush(files, new ChocolateyPushSettings { Source = pushSource });
}

void StoreChocolateyArtifact(FilePath nuspecPath)
{
    var dir = $"{ArtifactsDirectory}/Chocolatey/{nuspecPath.GetFilenameWithoutExtension()}";
    EnsureDirectoryExists(dir);
    ChocolateyPack(nuspecPath, new ChocolateyPackSettings { OutputDirectory = dir });
}

void PublishNuGetArtifact(string packageId, string pushSource)
{
    var files = GetFiles($"{ArtifactsDirectory}/NuGet/{packageId}/*.nupkg");
    NuGetPush(files, new NuGetPushSettings { Source = pushSource });
}

void StoreNuGetArtifact(FilePath nuspecPath)
{
    var dir = $"{ArtifactsDirectory}/NuGet/{nuspecPath.GetFilenameWithoutExtension()}";
    EnsureDirectoryExists(dir);
    NuGetPack(nuspecPath, new NuGetPackSettings { OutputDirectory = dir });
}

#addin nuget:?package=octokit

public class OctokitSettings
{
    public string Owner { get; set; }
    public string Repository { get; set; }
    public string Token { get; set; }
    public string GitTag { get; set; }
    public string TextBody { get; set; }
    public bool IsDraft { get; set; }
    public bool IsPrerelease { get; set; }
}

public class OctokitAsset
{
    public OctokitAsset(FilePath artifact, string contentType)
    {
        Artifact = artifact;
        ContentType = contentType;
    }

    public FilePath Artifact { get; }
    public string ContentType { get; }
}

public void PublishGitHubReleaseWithArtifacts(OctokitSettings settings, IEnumerable<OctokitAsset> assets)
{
    var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(settings.Repository))
    {
        Credentials = new Octokit.Credentials(settings.Token)
    };
    
    var newRelease = new Octokit.NewRelease(settings.GitTag)
    {
        Name = settings.GitTag,
        Body = settings.TextBody,
        Draft = settings.IsDraft,
        Prerelease = settings.IsPrerelease
    };

    var publishedRelease = client.Repository.Release.Create(settings.Owner, settings.Repository, newRelease).Result;

    Information("Created GitHub release {0}", settings.GitTag);

    foreach (var asset in assets)
    {
        var fileName = asset.Artifact.GetFilename().FullPath;

        var assetUpload = new Octokit.ReleaseAssetUpload
        {
            FileName = fileName,
            ContentType = asset.ContentType,
            RawData = System.IO.File.OpenRead(asset.Artifact.FullPath)
        };

        var publishedAsset = client.Repository.Release.UploadAsset(publishedRelease, assetUpload).Result;

        Information("Published artifact {0}", fileName);
    }
}

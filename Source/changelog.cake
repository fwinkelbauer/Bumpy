public void EnsureChangelog(FilePath changelog, string version)
{
    var regex = new System.Text.RegularExpressions.Regex($"# {version}");
    var text = System.IO.File.ReadAllText(changelog.FullPath);

    if (!regex.IsMatch(text))
    {
        throw new InvalidOperationException($"Version {version} not mentioned in CHANGELOG!");
    }
}

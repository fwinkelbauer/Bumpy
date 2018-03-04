void MSTest2_VS2017(string assemblyPattern)
{
    MSTest2_VS2017(GetFiles(assemblyPattern));
}

void MSTest2_VS2017(IEnumerable<FilePath> assemblyPaths)
{
    VSTest(assemblyPaths, FixToolPath(new VSTestSettings { TestAdapterPath = "." }));
}

// https://github.com/cake-build/cake/issues/1522
VSTestSettings FixToolPath(VSTestSettings settings)
{
    #tool vswhere
    settings.ToolPath =
        VSWhereLatest(new VSWhereLatestSettings { Requires = "Microsoft.VisualStudio.PackageGroup.TestTools.Core" })
        .CombineWithFilePath(File(@"Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"));
    return settings;
}

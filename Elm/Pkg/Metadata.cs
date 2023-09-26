namespace ElmX.Elm.Pkg
{
    public class Metadata
    {
        public string Name { get; private set; } = "";
        public string Summary { get; private set; } = "";
        public string License { get; private set; } = "";
        public string Version { get; private set; } = "";
        public string ElmMinVersion { get; private set; } = "";
        public string ElmMaxVersion { get; private set; } = "";
        public List<string> ExposedModules { get; private set; } = new List<string>();
        public Dictionary<string, string> Dependencies { get; private set; } = new();
        public Dictionary<string, string> TestDependencies { get; private set; } = new();
        public Metadata(Json package)
        {
            if (package != null)
            {
                Name = package.Name;
                Summary = package.Summary;
                License = package.License;
                Version = package.Version;
                ExposedModules = package.ExposedModules;
                Dependencies = package.Dependencies;
                TestDependencies = package.TestDependencies;

                if (package.ElmVersion.Contains("v"))
                {
                    var elmVersion = package.ElmVersion.Split("v");
                    ElmMinVersion = elmVersion[0];
                    ElmMaxVersion = elmVersion[1];
                }
                else
                {
                    ElmMinVersion = package.ElmVersion;
                    ElmMaxVersion = package.ElmVersion;
                }
            }
        }

    }
}
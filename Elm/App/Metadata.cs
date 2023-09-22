
namespace ElmX.Elm.App
{
    public class Metadata
    {
        public string ElmVersion { get; private set; } = "";
        public List<string> SourceDirs { get; private set; } = new List<string>();
        public Dependencies Dependencies { get; private set; } = new Dependencies();
        public Dependencies TestDependencies { get; private set; } = new Dependencies();

        public Metadata(Json application)
        {
            if (application != null)
            {
                ElmVersion = application.ElmVersion;
                SourceDirs = application.SourceDirs;
                Dependencies = application.Dependencies;
                TestDependencies = application.TestDependencies;
            }
        }
    }
}
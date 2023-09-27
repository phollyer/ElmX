using ElmX.Elm.Code;
using ElmX.Elm.Pkg;

namespace ElmX.Elm
{
    public class Package : Elm
    {
        // Metadata
        public Metadata Metadata { get; private set; }

        // Modules

        public List<Module> ExposedModules { get; private set; } = new();

        public Package(Pkg.Json json, Core.Json elmxJson)
        {
            Metadata = new Metadata(json);

            ExcludeDirs = elmxJson.AppJson.ExcludeDirs.ToList();
            ExcludeFiles = elmxJson.AppJson.ExcludeFiles.ToList();
        }
        public List<string> FindUnusedModules()
        {
            Module module = new();
            return module.FindUnused(this);
        }
    }
}
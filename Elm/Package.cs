using ElmX.Elm.Code;
using ElmX.Elm.Pkg;

namespace ElmX.Elm
{
    public class Package : Elm
    {
        public List<Module> ExposedModules { get; private set; } = new();

        public Package(Pkg.Json json, Core.Json elmxJson)
        {
            ExcludeDirs = elmxJson.AppJson.ExcludeDirs.ToList();
            ExcludeFiles = elmxJson.AppJson.ExcludeFiles.ToList();
            ExposedModules = json.ExposedModules.Select(m => new Module(m)).ToList();

            FileList = FindAllFiles("src", json.ExposedModules, ExcludeDirs);
        }
    }
}
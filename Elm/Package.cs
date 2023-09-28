using ElmX.Elm.Code;
using ElmX.Core.Console;

namespace ElmX.Elm
{
    public class Package : Elm
    {
        public List<Module> ExposedModules { get; private set; } = new();

        public Package(Pkg.Json json, Core.Json elmxJson)
        {
            ExcludeDirs = elmxJson.AppJson.ExcludeDirs.ToList();
            ExcludeFiles = elmxJson.AppJson.ExcludeFiles.ToList();

            foreach (string dotNotation in json.ExposedModules)
            {
                Module module = new();
                module.DotNotation = dotNotation;
                module.FilePath = ElmX.Core.Path.FromDotNotation(dotNotation);

                ExposedModules.Add(module);
            }

            FileList = FindAllFiles("src", ExposedModules, ExcludeDirs);
        }
    }
}
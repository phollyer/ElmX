using ElmX.Elm.Code;

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
                string filePath = System.IO.Path.Join("src", ElmX.Core.Path.FromDotNotation(dotNotation));
                Module module = new(filePath);

                ExposedModules.Add(module);
            }

            foreach (string filePath in ExposedModules.Select(module => module.FilePath))
            {
                Module module = new(filePath);

                module.ParseImports();

                foreach (Import import in module.Imports)
                {
                    Imports.Add(import);
                }

                ModulePaths.Add(module.FilePath);
            }

            ModulesFromImports(Modules, ModulePaths, "src", Imports);


            FileList = FindAllFiles("src");
        }
    }
}
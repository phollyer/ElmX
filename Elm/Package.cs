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
        }

        public Package FindAllFiles()
        {
            FileList = FindAllFiles("src");

            return this;
        }

        public new List<string> FindUnusedModules()
        {
            foreach (Module module in ExposedModules)
            {
                module.ParseImports();

                foreach (Import import in module.Imports)
                {
                    Imports.Add(import);
                }

                ModulePaths.Add(module.FilePath);
            }

            ModulesFromImports(Modules, ModulePaths, "src", Imports);

            return base.FindUnusedModules();
        }

        public new Dictionary<string, List<string>> FindUnusedImports()
        {

            return base.FindUnusedImports();
        }
    }
}
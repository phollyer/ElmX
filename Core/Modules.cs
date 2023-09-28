using ElmX.Core.Console;
using ElmX.Elm;
using ElmX.Elm.Code;

namespace ElmX.Core
{
    static class Modules
    {
        static public List<string> FindUnused(Application app)
        {
            foreach (Import import in app.EntryModule.Imports)
            {
                app.Imports.Add(import);
            }

            foreach (string srcDir in app.SourceDirs)
            {
                ModulesFromImports(app.Modules, app.ModulePaths, srcDir, app.Imports);
            }

            return FindUnused(app.SourceDirs, app.FileList, app.ModulePaths, app.ExcludeDirs, app.ExcludeFiles);
        }
        static public List<string> FindUnused(Package pkg)
        {
            foreach (string filePath in pkg.ExposedModules.Select(module => module.FilePath))
            {
                Elm.Code.Module module = new();
                module.FilePath = System.IO.Path.Join("src", filePath);
                module.Read();
                module.RemoveDocumentationComments();
                module.ParseImports();

                foreach (Import import in module.Imports)
                {
                    pkg.Imports.Add(import);
                }

                pkg.ModulePaths.Add(module.FilePath);
            }

            ModulesFromImports(pkg.Modules, pkg.ModulePaths, "src", pkg.Imports);

            return FindUnused(new List<string>() { "src" }, pkg.FileList, pkg.ModulePaths, pkg.ExcludeDirs, pkg.ExcludeFiles); ;
        }

        static private List<string> FindUnused(List<string> srcDirs, List<string> fileList, List<string> modulePaths, List<string> excludeDirs, List<string> excludeFiles)
        {
            WriteSummary(srcDirs, excludeDirs, excludeFiles, modulePaths, fileList);

            short lineNumberToWriteAt = (short)(10 + (short)srcDirs.Count() + (short)excludeDirs.Count() + (short)excludeFiles.Count());

            List<string> Unused = new();

            int fileCount = 0;

            foreach (var filePath in fileList)
            {
                fileCount++;

                bool found = false;
                foreach (var importPath in modulePaths)
                {
                    if (filePath == importPath)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Unused.Add(filePath);
                }
            }

            Writer.WriteAt($"Found: {Unused.Count} unused files", 0, lineNumberToWriteAt);
            Writer.EmptyLine();

            return Unused;
        }

        static private void ModulesFromImports(List<Module> modules, List<string> modulePaths, string srcDir, List<Import> imports)
        {
            foreach (Import import in imports)
            {
                string modulePath = ModulePathFromDotNotation(srcDir, import.Name);

                if (File.Exists(modulePath))
                {
                    Elm.Code.Module module = ModuleFromPath(modulePath);

                    modules.Add(module);

                    modulePaths.Add(module.FilePath);

                    CreateModuleList(modules, modulePaths, srcDir, module);
                }
            }

            modulePaths.Sort();
        }

        static public string ModulePathFromDotNotation(string srcDir, string dotNotation)
        {
            return System.IO.Path.Join(srcDir, ElmX.Core.Path.FromDotNotation(dotNotation));
        }

        static private Module ModuleFromPath(string path)
        {
            Module module = new();
            module.FilePath = path;
            module.Read();
            module.RemoveDocumentationComments();
            module.ParseImports();

            return module;
        }


        static private void CreateModuleList(List<Module> list, List<string> paths, string srcDir, Elm.Code.Module module)
        {
            foreach (Import import in module.Imports)
            {
                string modulePath = ModulePathFromDotNotation(srcDir, import.Name);

                if (paths.IndexOf(modulePath) == -1 && File.Exists(modulePath))
                {
                    Elm.Code.Module _module = ModuleFromPath(modulePath);

                    list.Add(_module);

                    paths.Add(modulePath);

                    CreateModuleList(list, paths, srcDir, _module);
                }
            }
        }

        static private void WriteSummary(List<string> sourceDirs, List<string> excludeDirs, List<string> excludeFiles, List<string> modulePaths, List<string> allFiles)
        {
            Writer.EmptyLine();
            Writer.WriteLine("Searching Dirs:");
            Writer.WriteLines("\t", sourceDirs);
            Writer.EmptyLine();

            Writer.WriteLine("Exclude Dirs:");
            Writer.WriteLines("\t", excludeDirs);
            Writer.EmptyLine();

            Writer.WriteLine("Exclude Files:");
            Writer.WriteLines("\t", excludeFiles);
            Writer.EmptyLine();

            Writer.WriteLine($"Found: {modulePaths.Count()} unique imports");
            Writer.WriteLine($"Found: {allFiles.Count()} files");
        }
    }
}
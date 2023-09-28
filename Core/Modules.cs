using ElmX.Core.Console;
using ElmX.Elm;
using ElmX.Elm.Code;

namespace ElmX.Core
{
    static class Modules
    {
        static public List<string> FindUnused(Application app)
        {
            app.ModulePaths.Add(app.EntryModule.FilePath);

            foreach (Import import in app.EntryModule.Imports)
            {
                app.Imports.Add(import);
            }

            foreach (string srcDir in app.SourceDirs)
            {
                ModulesFromImports(app.Modules, app.ModulePaths, srcDir, app.Imports);
            }

            List<string> Unused = new();

            int fileCount = 0;

            WriteSummary(app.SourceDirs, app.ExcludeDirs, app.ExcludeFiles, app.ModulePaths, app.FileList);

            short lineNumberToWriteAt = (short)(10 + (short)app.SourceDirs.Count() + (short)app.ExcludeDirs.Count() + (short)app.ExcludeFiles.Count());

            foreach (var filePath in app.FileList)
            {
                fileCount++;

                Writer.WriteAt($"Checking file {fileCount}", 0, lineNumberToWriteAt);
                Writer.EmptyLine();

                bool found = false;
                foreach (var importPath in app.ModulePaths)
                {
                    if (filePath == importPath || filePath == app.EntryModule.FilePath)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found && !app.ExcludeFiles.Contains(filePath))
                {
                    Unused.Add(filePath);
                }
            }

            Writer.WriteAt($"Found: {Unused.Count} unused files", 0, lineNumberToWriteAt);
            Writer.EmptyLine();

            return Unused;
        }
        static public List<string> FindUnused(Package pkg)
        {
            foreach (string exposedModule in pkg.ExposedModules.Select(module => module.FilePath))
            {
                Elm.Code.Module module = new();
                module.FilePath = ModulePathFromDotNotation("src", exposedModule); ;

                module.ParseImports();

                foreach (Import import in module.Imports)
                {
                    pkg.Imports.Add(import);
                }

                pkg.ModulePaths.Add(module.FilePath);
            }

            ModulesFromImports(pkg.Modules, pkg.ModulePaths, "src", pkg.Imports);

            List<string> Unused = new();

            int fileCount = 0;

            WriteSummary(new List<string>() { "src" }, pkg.ExcludeDirs, pkg.ExcludeFiles, pkg.ModulePaths, pkg.FileList);

            short lineNumberToWriteAt = (short)(10 + (short)pkg.ExcludeDirs.Count() + (short)pkg.ExcludeFiles.Count());

            foreach (var filePath in pkg.FileList)
            {
                fileCount++;

                Writer.WriteAt($"Checking file {fileCount}", 0, lineNumberToWriteAt);
                Writer.EmptyLine();

                bool found = false;
                foreach (var importPath in pkg.ModulePaths)
                {
                    if (filePath == importPath || pkg.ExposedModules.Select(module => module.FilePath).Contains(filePath))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found && !pkg.ExcludeFiles.Contains(filePath))
                {
                    Unused.Add(filePath);
                }
            }

            Writer.WriteAt($"Found: {Unused.Count().ToString()} unused files", 0, lineNumberToWriteAt);
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
        static private bool IsNotExcluded(string file, List<string> excludedDirs)
        {
            string seperator = System.IO.Path.DirectorySeparatorChar.ToString();

            foreach (string excludedDir in excludedDirs)
            {
                string searchStr = $"{seperator}{excludedDir}{seperator}";
                if (file.Contains(searchStr))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
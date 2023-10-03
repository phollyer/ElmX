using ElmX.Core.Console;
using ElmX.Elm;
using ElmX.Elm.Code;

namespace ElmX.Core
{
    static class Imports
    {
        static public Dictionary<string, List<string>> FindUnused(Application app)
        {
            Dictionary<string, List<string>> Unused = new();

            CheckFile(app.EntryModule.FilePath, Unused);

            foreach (string filePath in app.FileList)
            {

            }

            Writer.EmptyLine();

            return Unused;
        }
        static public Dictionary<string, List<string>> FindUnused(Package pkg)
        {
            Dictionary<string, List<string>> Unused = new();

            return Unused;
        }

        static private void CheckFile(string filePath, Dictionary<string, List<string>> unused)
        {
            Elm.Code.Module module = new(filePath);
            module.ParseImports();

            Writer.WriteLine(module.RawContent);


            foreach (Import import in module.Imports)
            {
                bool found = false;

                if (ImportIsUsedByName(module.RawContent, import.Name))
                {
                    found = true;
                }
                else if (ImportIsUsedAsName(module.RawContent, import.As))
                {
                    found = true;
                }

                if (!found)
                {
                    if (unused.ContainsKey(filePath))
                    {
                        unused[filePath].Add(import.Name);
                    }
                    else
                    {
                        unused.Add(filePath, new List<string>() { import.Name });
                    }
                }
            }
        }

        static private bool ImportIsUsedByName(string content, string name)
        {
            return
                content.Contains($": {name} ")
                || content.Contains($"-> {name}\n")
                || content.Contains($"\n{name} ")
                || content.Contains($"\n{name}\n")
                ;
        }

        static private bool ImportIsUsedAsName(string content, string _as)
        {
            return
            // followed by a space
                content.Contains($": {_as} ")
                || content.Contains($"-> {_as} ")
                || content.Contains($"\t{_as} ")

            // followed by a dot
                || content.Contains($": {_as}.")
                || content.Contains($"-> {_as}.")
                || content.Contains($" {_as}.")
                || content.Contains($"\t{_as}.")
                || content.Contains($"\n{_as}.");
            ;
        }

    }
}
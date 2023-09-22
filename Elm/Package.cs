using ElmX.Elm.Code;
using ElmX.Elm.Pkg;

namespace ElmX.Elm
{
    public class Package
    {
        // Metadata
        public Metadata Metadata { get; private set; } = new();

        // Modules

        public List<Module> ExposedModules { get; private set; } = new();

        public List<Module> UsedModules { get; private set; } = new();

        public List<Module> UnusedModules { get; private set; } = new();
    }
}
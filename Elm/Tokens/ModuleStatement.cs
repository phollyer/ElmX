using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElmX.Elm.Tokens
{
    public class ModuleStatement
    {
        public string Name { get; set; } = "";

        public List<String> Exposing { get; set; } = new();
    }
}
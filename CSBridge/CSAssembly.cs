using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace CSBridge
{
    public class CSAssembly
    {
        // Top Level representation of C# Assembly (.dll) and its types.
        public string name;
        public AssemblyDefinition assembly;
        public List<CSObject> types = new List<CSObject>();



        public void AddObject(CSObject type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (!types.Any(t => t.name == type.name && t.assembly.Name.Name == type.assembly.Name.Name))
            {
                types.Add(type);
            }

        }

        public CSAssembly(string name, AssemblyDefinition assembly)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null.");
            }
            this.name = name;
            this.assembly = assembly;
        }
    }
}

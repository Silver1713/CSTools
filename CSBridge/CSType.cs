using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
namespace CSBridge
{
    public class CSType
    {
        public string name;
        public AssemblyDefinition assembly;
        public TypeDefinition typeDefinition;


        public bool isStruct = false; // Is this a struct type?
        public bool isPrimitive = false; // Is this a primitive type?
        public bool isGeneric = false; // Is this a generic type?
        public CSType(string name, AssemblyDefinition assembly, TypeDefinition typeDefinition)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null.");
            }
            if (typeDefinition == null)
            {
                throw new ArgumentNullException(nameof(typeDefinition), "Type definition cannot be null.");
            }
            this.name = name;
            this.assembly = assembly;
            this.typeDefinition = typeDefinition;

            this.isStruct = typeDefinition.IsValueType && !typeDefinition.IsPrimitive && !typeDefinition.IsEnum;
            this.isPrimitive = typeDefinition.IsPrimitive;
            this.isGeneric = typeDefinition.HasGenericParameters;
        }
    }
}

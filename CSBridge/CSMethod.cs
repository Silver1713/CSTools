using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
namespace CSBridge
{
    public class CSMethod
    {
        public string methodName;
        public AssemblyDefinition assembly;
        public TypeDefinition typeDefinition;


        public CSType returnType;
        public List<CSParameter> parameters;


        public List<CSAttribute> methodAttributes = new List<CSAttribute>();
        
        public CSMethod(string methodName, AssemblyDefinition assembly, TypeDefinition typeDefinition)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException("Method name cannot be null or empty.", nameof(methodName));
            }
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null.");
            }
            if (typeDefinition == null)
            {
                throw new ArgumentNullException(nameof(typeDefinition), "Type definition cannot be null.");
            }
            this.methodName = methodName;
            this.assembly = assembly;
            this.typeDefinition = typeDefinition;
            this.parameters = new List<CSParameter>();
            this.methodAttributes = new List<CSAttribute>();
        }


        public bool ContainAttributes
        {
            get
            {
                return methodAttributes.Count > 0;
            }
        }



    }
}

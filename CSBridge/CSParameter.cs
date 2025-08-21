using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
namespace CSBridge
{
    public class CSParameter
    {
        public string name;
        public AssemblyDefinition assembly;
        public ParameterDefinition parameterDefinition;

        bool isOut = false; // Is this an out parameter?
        bool isIn = false; // Is this an in parameter?
        bool isOptional = false; // Is this an optional parameter?
        bool isRef = false; // Is this a ref parameter?

        bool isStruct = false; // Is this a struct type?
        bool isPrimitive = false; // Is this a primitive type?



        bool hasOtherAttributes = false; // Override method attributes.
        public List<CSAttribute> attributes = new List<CSAttribute>();

        public CSParameter(string name, AssemblyDefinition assembly, ParameterDefinition parameterDefinition)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null.");
            }
            if (parameterDefinition == null)
            {
                throw new ArgumentNullException(nameof(parameterDefinition), "ParameterDefinition cannot be null.");
            }


            TypeDefinition typeDefinition = parameterDefinition.ParameterType.Resolve();

            this.name = name;
            this.assembly = assembly;
            this.parameterDefinition = parameterDefinition;

            this.isOut = parameterDefinition.IsOut;
            this.isIn = parameterDefinition.IsIn;
            this.isOptional = parameterDefinition.IsOptional;
            this.isRef = typeDefinition.IsByReference;
            


            this.isStruct = typeDefinition.IsValueType && !typeDefinition.IsPrimitive && !typeDefinition.IsEnum;
            this.isPrimitive = typeDefinition.IsPrimitive;
        }
    }
}

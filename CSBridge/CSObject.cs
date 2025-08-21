using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace CSBridge
{
    public class CSObject
    {

        // Represents a C# type with its name, assembly, and type definition.s
        public string name;
        public AssemblyDefinition assembly;
        public TypeDefinition typeDefinition;

        public List<CSMethod> methods = new List<CSMethod>(); // Methods defined in this type.

        // Serialized header information for the type.
        public List<string> headers = new List<string>();


        bool hasOtherAttributes = false; // Override method attributes.

        public List<CSAttribute> attributes = new List<CSAttribute>();

        public CSObject(string name, AssemblyDefinition assembly, TypeDefinition typeDefinition)
        {
            this.name = name;
            this.assembly = assembly;
            this.typeDefinition = typeDefinition;
        }


        public override string ToString()
        {
            string info = $"File Information:\n" +
                   $"Name: {name}\n" +
                   $"Assembly: {assembly.Name.Name}\n" +
                   $"Type Definition: {typeDefinition.FullName}\n" +
                   $"Headers: {string.Join(", ", headers)}\n" +
                   $"Has Other Attributes: {hasOtherAttributes}\n" +
                   $"Attributes Count: {attributes.Count}";

            if (attributes.Count > 0)
            {
                info += "\nAttributes:\n";
                foreach (CSAttribute attribute in attributes)
                {
                    switch (attribute.Attribute)
                    {
                        case CSAttribute.AttributeType.StaticAccessor:
                            Engine.Bindings.StaticAccessorAttribute staticAccessor = attribute.staticAccessorAttribute;
                            info += $"StaticAccessor: Name = {staticAccessor.Name}, Type = {staticAccessor.AccessorType.ToString()}\n";
                            break;


                    }

                }
            }



            return info;


        }

        public void AddHeader(string header)
        {
            if (!headers.Contains(header))
            {
                headers.Add(header);
            }
        }


        public void AddMethod(CSMethod method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method), "Method cannot be null.");
            }
            
            methods.Add(method);
        }

        public bool HasOtherAttribute
        {
            get { return hasOtherAttributes; }
            set
            {
                if (value != hasOtherAttributes)
                {
                    hasOtherAttributes = value;
                }
            }
        }


    }
}

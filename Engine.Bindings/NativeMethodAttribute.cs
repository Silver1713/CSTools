using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Bindings
{
    public class NativeMethodAttribute : Attribute
    {
       public string Name { get; }

       public NativeMethodAttribute(string name)
       {
            Name = name;
       }
    }
}

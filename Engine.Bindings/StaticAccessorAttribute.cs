using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Bindings
{
    public enum StaticAccessorType
    {
        Dot,
        DoubleColon,
        Arrow
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class StaticAccessorAttribute : Attribute
    {
        public string Name { get; }
        public StaticAccessorType AccessorType { get; }

        public StaticAccessorAttribute(string name, StaticAccessorType accessorType = StaticAccessorType.Dot)
        {
            Name = name;
            AccessorType = accessorType;
        }

    }
}

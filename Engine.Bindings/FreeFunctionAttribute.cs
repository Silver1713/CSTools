using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Bindings
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class FreeFunctionAttribute : Attribute
    {
        public string Name { get; }
        public bool IsThreadSafe { get; }
        public bool IsFree { get; }
        public FreeFunctionAttribute(string name, bool isThreadSafe = false, bool isFree = false)
        {
            Name = name;
            IsThreadSafe = isThreadSafe;
            IsFree = isFree;
        }
    }
}

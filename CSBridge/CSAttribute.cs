using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Bindings;
namespace CSBridge
{
    public class CSAttribute
    {

        public enum AttributeType
        {
            None,
            FreeFunction,
            StaticAccessor,
            NativeMethod
        }
       
        public FreeFunctionAttribute freeFunctionAttribute = null;
        public StaticAccessorAttribute staticAccessorAttribute = null;
        public NativeMethodAttribute nativeMethodAttribute = null;

        public AttributeType Attribute
        {
            get
            {
                if (freeFunctionAttribute != null)
                {
                    return AttributeType.FreeFunction;
                }
                else if (staticAccessorAttribute != null)
                {
                    return AttributeType.StaticAccessor;
                }
                else if (nativeMethodAttribute != null)
                {
                    return AttributeType.NativeMethod;
                }
                return AttributeType.None;
            }
        }

        public CSAttribute(FreeFunctionAttribute freeFunctionAttribute)
        {
            this.freeFunctionAttribute = freeFunctionAttribute ?? throw new ArgumentNullException(nameof(freeFunctionAttribute));
        }

        public CSAttribute(StaticAccessorAttribute staticAccessorAttribute)
        {
            this.staticAccessorAttribute = staticAccessorAttribute ?? throw new ArgumentNullException(nameof(staticAccessorAttribute));
        }

        public CSAttribute(NativeMethodAttribute nativeMethodAttribute)
        {
            this.nativeMethodAttribute = nativeMethodAttribute ?? throw new ArgumentNullException(nameof(nativeMethodAttribute));
        }

    }
}

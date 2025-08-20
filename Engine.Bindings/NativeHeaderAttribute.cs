namespace Engine.Bindings;

[AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Interface, Inherited = false)]
public class NativeHeaderAttribute : Attribute
{
    public string Header { get; }
    
    public NativeHeaderAttribute(string header)
    {
        Header = header;
    }

}

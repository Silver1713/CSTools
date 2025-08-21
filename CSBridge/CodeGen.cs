using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace CSBridge
{
    public class CodeGen

    {
        public static CodeGen Instance { get; private set; } = null;

        public List<CSAssembly> assemblies = new List<CSAssembly>();
        GeneratorPreference preference;
        DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();

        public CodeGen(GeneratorPreference preference)
        {
            this.preference = preference;
        }


        public static CodeGen Initialize(GeneratorPreference preference)
        {
            if (Instance == null)
            {
                Instance = new CodeGen(preference);
            }

            return Instance;
        }

        public void Log(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (preference.Verbose)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
            }

        }

        public void LogError(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (preference.Verbose)
            {
                Console.Error.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
            }


        }



        public void Generate()
        {

            if (preference == null)
            {
                throw new ArgumentNullException(nameof(preference), "GeneratorPreference cannot be null.");
            }
            resolver.AddSearchDirectory(AppDomain.CurrentDomain.BaseDirectory);
            if (preference.Verbose)
            {
                Console.WriteLine("CSBridge - C# glue generation");
                Console.WriteLine("Made for a currently unnamed game engine.");
                Console.WriteLine();

                Log("Starting code generation with the following settings:");
                Log(preference.ToString());
                Console.WriteLine();



            }

            foreach (var input in preference.inputs.Select(
                d => d.EndsWith(".dll") ? d : d.EndsWith(".cs") ? d : $"{d}.dll")
            )
            {
                AnalyzeAsync(input).GetAwaiter().GetResult();
            }

            if (preference.Verbose)
            {
                Log("Code generation completed successfully.");
            }
            resolver.Dispose();
            Log("Resolver disposed.");
            Log("Generating code for all assemblies...");
            foreach (var assembly in assemblies)
            {
                Log($"Generating code for assembly: {assembly.name}");
                GenerateAsync(assembly).GetAwaiter().GetResult();
            }

        }


        public async Task GenerateAsync(CSAssembly assembly)
        {
            foreach (CSObject _o in assembly.types)
            {
                Log($"Generating code for type: {_o.name}");
                StringBuilder sb = new StringBuilder();
               

                // ...
                await Emit($"{_o.name.Replace('.', '_')}_GEN.hpp", sb);


            }



            
        }




        #region Code Generation Methods

        public async Task Emit(string path, StringBuilder sb)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Filename cannot be null or empty.", nameof(path));
            }
            if (sb == null)
            {
                throw new ArgumentNullException(nameof(sb), "StringBuilder cannot be null.");
            }
            // Write the generated code to a file
            System.IO.File.WriteAllText(path, sb.ToString());
        }

        public void GenerateHeader(CSObject obj ,int ext, ref StringBuilder sb)
        {

        }




        public async Task GenerateSourceAsync(CSObject obj)
        {
        }
        public void GenerateHeaderGuardStart(string guardName, ref StringBuilder sb)
        {

        }
        #endregion


        public async Task AnalyzeAsync(string dllPath)
        {


            if (string.IsNullOrEmpty(dllPath))
            {
                throw new ArgumentException("DLL path cannot be null or empty.", nameof(dllPath));
            }
            Log($"Analyzing DLL: {dllPath}");

            string fullPath = System.IO.Path.GetFullPath(dllPath);
            resolver.AddSearchDirectory(System.IO.Path.GetDirectoryName(fullPath));


            try
            {
                AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(dllPath,
                 new ReaderParameters
                 {
                     AssemblyResolver = resolver
                 }
             );

                TypeReference nativeHeaderRef = assembly.MainModule.ImportReference(typeof(Engine.Bindings.NativeHeaderAttribute));
                TypeDefinition nativeHeaderType = nativeHeaderRef.Resolve();

                TypeReference StaticAccessorRef = assembly.MainModule.ImportReference(typeof(Engine.Bindings.StaticAccessorAttribute));
                TypeDefinition staticAccessorType = StaticAccessorRef.Resolve();

                TypeReference nativeMethodRef = assembly.MainModule.ImportReference(typeof(Engine.Bindings.NativeMethodAttribute));
                TypeDefinition nativeMethodType = nativeMethodRef.Resolve();


                // Print assembly name
                Log("Loaded Assembly: " + assembly.Name);
                CSAssembly cSAssembly = new CSAssembly(assembly.Name.Name, assembly);

                foreach (ModuleDefinition module in assembly.Modules)
                {
                    Log($"Module: {module.Name}");
                    // get all objects in the module
                    foreach (TypeDefinition type in module.Types)
                    {
                        if (type.Name == "<Module>")
                        {
                            continue; // Skip the <Module> type
                        }
                        Log($"Class: {type.FullName}");
                        CSObject csObject = new CSObject(type.FullName, assembly, type);
                        // Get Attribute for type
                        if (type.HasCustomAttributes)
                        {
                            foreach (var attribute in type.CustomAttributes)
                            {
                                // Only get attribute from Engine.Bindings
                                TypeDefinition typeDefinition = attribute.AttributeType.Resolve();
                                if (typeDefinition == nativeHeaderType)
                                {
                                    Log($"Found NativeHeader attribute on class: {type.FullName}");


                                    foreach (CustomAttributeArgument arg in attribute.ConstructorArguments)
                                    {
                                        string headerValue = arg.Value.ToString()!;
                                        Log($" --> Header: {headerValue}"); // Print the header values
                                        csObject.AddHeader(headerValue);
                                    }




                                }
                                else if (typeDefinition == staticAccessorType)
                                {
                                    csObject.HasOtherAttribute = true; // Set the flag for other attributes
                                    Log($"Found StaticAccessor attribute on class: {type.FullName}");
                                    CustomAttributeArgument nameArg = attribute.ConstructorArguments.FirstOrDefault();
                                    CustomAttributeArgument typeArg = attribute.ConstructorArguments.Skip(1).FirstOrDefault();

                                    string name = nameArg.Value?.ToString() ?? string.Empty;
                                    int valueType = typeArg.Value is int ? (int)typeArg.Value : 0;

                                    Engine.Bindings.StaticAccessorType staticAccessorTypeValue = (Engine.Bindings.StaticAccessorType)valueType;
                                    Engine.Bindings.StaticAccessorAttribute staticAccessor = new Engine.Bindings.StaticAccessorAttribute(name, staticAccessorTypeValue);
                                    csObject.attributes.Add(new CSAttribute(staticAccessor));
                                }






                            }
                        }


                        // Methods
                        foreach (MethodDefinition method in type.Methods)
                        {
                            if (!method.IsInternalCall)
                                continue;

                            if (method.HasGenericParameters)
                            {
                                Log($"Skipping generic method {method.FullName} as it is not supported.");
                                continue;
                            }
                            Log("Found External Method: " + method.FullName);
                            CSMethod cSMethod = new CSMethod(method.Name, assembly, type);

                            //Get Method Attributes
                            foreach (CustomAttribute attribute in method.CustomAttributes)
                            {
                                TypeDefinition attributeDef = attribute.AttributeType.Resolve();
                                if (attributeDef == nativeMethodType)
                                {
                                    string name = attribute.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? method.Name;
                                    Log($"Found Native method: {name} on method: {method.FullName}");

                                    Engine.Bindings.NativeMethodAttribute nativeMethod = new Engine.Bindings.NativeMethodAttribute(name);
                                    cSMethod.methodAttributes.Add(new CSAttribute(nativeMethod));

                                }
                                else if (attributeDef == staticAccessorType)
                                {
                                    string calling_class = attribute.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? string.Empty;
                                    int valueType = attribute.ConstructorArguments.Skip(1).FirstOrDefault().Value is int ? (int)attribute.ConstructorArguments.Skip(1).FirstOrDefault().Value : 0;


                                    Engine.Bindings.StaticAccessorType staticAccessorTypeValue = (Engine.Bindings.StaticAccessorType)valueType;
                                    Engine.Bindings.StaticAccessorAttribute staticAccessor = new Engine.Bindings.StaticAccessorAttribute(calling_class, staticAccessorTypeValue);
                                    Log($"Found StaticAccessor attribute on method: {method.FullName} with calling class: {calling_class} and type: {staticAccessorTypeValue.ToString()}");

                                    cSMethod.methodAttributes.Add(new CSAttribute(staticAccessor));

                                }
                            }


                            // Get the method parameters
                            foreach (ParameterDefinition parameter in method.Parameters)
                            {
                                CSParameter param = new CSParameter(parameter.Name, assembly, parameter);



                                cSMethod.parameters.Add(param);
                            }


                            csObject.AddMethod(cSMethod);

                        }


                        cSAssembly.AddObject(csObject);
                    }
                }


                // Add the assembly to the list of assemblies
                assemblies.Add(cSAssembly);
            }
            catch (Exception ex)
            {
                LogError($"Error analyzing {dllPath}: {ex.Message}");
            }

            resolver.RemoveSearchDirectory(System.IO.Path.GetDirectoryName(dllPath));
        }
    }
}

using CSBridge;
class Program
{

    static bool exitRequested = false;
    static GeneratorPreference GetSettings(string[] args)
    {
        List<string> inputs = new List<string>();
        string outputDirectory = string.Empty;
        bool usingMonoAPI = true;
        bool verbose = false;


        // This is a simple C# program that prints "Hello, World!" to the console.
        for (int i = 1; i < args.Length; ++i)
        {
            string arg = args[i];


            if (arg.StartsWith("-h") || arg.StartsWith("--help"))
            {
                // Print help message
                Console.WriteLine("Usage: CSBridge [options] <input files>");
                Console.WriteLine("Options:");
                Console.WriteLine("  -h, --help       Show this help message");
                Console.WriteLine("  -o <directory>   Specify output directory");
                Console.WriteLine("  -i               Use interop mode (Mono API)");
                Console.WriteLine("  -v               Enable verbose output");
                Console.WriteLine("-s, --syntax       Enable syntax checking, this uses raw .cs files and do not accept a DLL ");
                Console.WriteLine("By default the generator will do IL meta/sematic checking which uses a precompiled DLL");

                Program.exitRequested = true;

                return null;
            }



            if (arg.StartsWith("-o"))
            {
                // Output directory specified
                if (i + 1 < args.Length)
                {
                    outputDirectory = args[i + 1];
                    i++; // Skip the next argument as it is the directory
                    continue;
                }
                else
                {
                    Console.Error.WriteLine("Error: No output directory specified after -o");
                    return null;
                }
            }


            if (arg.StartsWith("-i"))
            {
                // Interop mode specified
                usingMonoAPI = false;
                continue;

            }

            if (arg.StartsWith("-v"))
            {
                // Verbose mode specified
                verbose = true;
                continue;
            }

            inputs.Add(arg);
        }


        if (inputs.Count == 0)
        {
            Console.Error.WriteLine("Error: No input files specified.");
            return null;
        }

        if (string.IsNullOrEmpty(outputDirectory))
        {
            Console.Error.WriteLine("Error: No output directory specified.");
            return null;
        }

        string fullOutputDirectory = System.IO.Path.GetFullPath(outputDirectory);
        if (!System.IO.Directory.Exists(fullOutputDirectory))
        {
            // Make the directory if it does not exist
            try
            {
                System.IO.Directory.CreateDirectory(fullOutputDirectory);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error creating output directory: {ex.Message}");
                return null;
            }
        }
        GeneratorPreference settings = new GeneratorPreference(inputs, fullOutputDirectory, usingMonoAPI, verbose);

        return settings;
    }
    static void Main(string[] args)
    {
        GeneratorPreference settings = GetSettings(args);
        CodeGen.Initialize(settings);

        if (Program.exitRequested)
        {
            return;
        }
        if (settings == null)
        {
           Console.WriteLine("Exiting due to errors in settings.");
            return;
        }

        if (settings.syntaxCheck)
        {
            Console.WriteLine("Warning: Running syntax check mode, analysis on a raw *.cs files would have limited information.");
            settings.ILMetaCheck = false; // Disable IL meta checking in syntax check mode
            
            
            


            //CodeGen.Instance.GenerateSyntax(settings);

            return;
        }
        else
        {
            //CodeGen.Instance.GenerateIL(settings);
            CodeGen.Instance.Generate();
            return;
        }
    }
}
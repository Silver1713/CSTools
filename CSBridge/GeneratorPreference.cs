using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSBridge
{
    public class GeneratorPreference
    {
        public List<string> inputs = new List<string>();
        public string outputDirectory;
        public bool usingMonoAPI = true;
        public bool syntaxCheck = false;
        public bool ILMetaCheck = true;
        public bool Verbose { get; set; } = false;

        public GeneratorPreference(List<string> inputs, string outputDirectory, bool usingMonoAPI = true, bool verbose = false)
        {
            this.inputs = inputs;
            this.outputDirectory = outputDirectory;
            this.usingMonoAPI = usingMonoAPI;
            this.Verbose = verbose;
        }

        public override string ToString()
        {
            return $"Inputs: {string.Join(", ", inputs)}\n" +
                   $"Output Directory: {outputDirectory}\n" +
                   $"Using Mono API: {usingMonoAPI}\n" +
                   $"Verbose: {Verbose}\n" +
                   $"Mode: {(syntaxCheck ? "Syntax" : "IL")}";
        }

    }
}

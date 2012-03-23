using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;
using NDesk.Options;

namespace AssemblyEmbed
{
    class Program
    {
        static int Main(string[] args)
        {
            string inputFile = null, outputFile = null;
            var opts = new OptionSet()
                           {
                               {"i|input=", "Input file",
                                   i => inputFile = i},
                               {"o|output=", "Output file",
                                   o => outputFile = o}
                           };
            var files = opts.Parse(args);
            if (string.IsNullOrWhiteSpace(inputFile))
            {
                Console.WriteLine("No assembly specified as embedding target.  Please use the -i option.");
                return -1;
            }
            if (outputFile == null)
            {
                Console.WriteLine("Overwriting target assembly {0}", inputFile);
                outputFile = inputFile;
            }
            if (files.Count <= 0)
            {
                Console.WriteLine("No input files specified");
                return -1;
            }

            Console.WriteLine("Reading {0}", inputFile);
            var module = ModuleDefinition.ReadModule(inputFile);
            foreach (var file in files)
            {
                Console.WriteLine("Embedding {0}", file);
                var bytes = File.ReadAllBytes(file);
                var resname = file.Replace("/", ".");
                var resource = new EmbeddedResource(resname, ManifestResourceAttributes.Public, bytes);
                module.Resources.Add(resource);
            }
            Console.WriteLine("Writing {0}", outputFile);
            module.Write(outputFile);
            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using McMaster.Extensions.CommandLineUtils;

namespace Veldrid.SPIRV
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CommandLineApplication.Execute<Program>(args);
        }

        [Option(
            "--search-path",
            "The set of directories to search for shader source files.",
            CommandOptionType.MultipleValue
        )]
        public string[] SearchPaths { get; }

        [Option(
            "--output-path",
            "The directory where compiled files are placed.",
            CommandOptionType.SingleValue
        )]
        public string OutputPath { get; }

        [Option(
            "--set",
            "The path to the JSON file containing shader variant definitions to compile.",
            CommandOptionType.SingleValue
        )]
        public string SetDefinitionPath { get; }

        public void OnExecute()
        {
            if (!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
            }

            ShaderVariantDescription[] descs;
            using (Stream sr = File.OpenRead(SetDefinitionPath))
            {
                descs = JsonSerializer.Deserialize<ShaderVariantDescription[]>(
                    sr,
                    VariantCompiler.JsonOptions
                );
            }

            HashSet<string> generatedPaths = new();

            VariantCompiler compiler = new(new List<string>(SearchPaths), OutputPath);
            foreach (ShaderVariantDescription desc in descs)
            {
                string[] newPaths = compiler.Compile(desc);
                foreach (string s in newPaths)
                {
                    generatedPaths.Add(s);
                }
            }

            string generatedFilesListText = string.Join(Environment.NewLine, generatedPaths);
            string generatedFilesListPath = Path.Combine(OutputPath, "vspv_generated_files.txt");
            File.WriteAllText(generatedFilesListPath, generatedFilesListText);
        }
    }
}

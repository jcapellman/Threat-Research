using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EEM
{
    
    class Program
    {
        private static byte[] CompileCodeToBytes(string sourceCode)
        {
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp9);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCode, options);

            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(File).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "System.Runtime.dll"))
            };

            var tempFile = Path.GetTempFileName();

            var result = CSharpCompilation.Create("EEM",
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication,
                    optimizationLevel: OptimizationLevel.Release,
                    platform: Platform.X86,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default)).Emit(tempFile);

            if (result.Success)
            {
                var bytes = File.ReadAllBytes(tempFile);

                File.Delete(tempFile);

                return bytes;
            }

            var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var diagnostic in failures)
            {
                Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
            }

            return null;
        }
        
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No file specified to embed");

                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"Could not find {args[0]} file");

                return;
            }

            var embedBytes = File.ReadAllBytes(args[0]);

            var base64String = Convert.ToBase64String(embedBytes);

            var templateString = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Template.csl"));

            templateString = templateString.Replace("{BASE64STRING}", base64String);

            File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "gen", "source.cs"), templateString);

            var compiledBytes = CompileCodeToBytes(templateString);
            
            File.WriteAllBytes("EEMgenerated.exe", compiledBytes);
        }
    }
}
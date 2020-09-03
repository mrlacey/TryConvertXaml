using System;
using System.IO;
using RapidXaml;

namespace TryConvertXamlSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select version:");
            Console.WriteLine("1 - Single file conversion");
            Console.WriteLine("2 - Project conversion");
            Console.WriteLine();

            while (true)
            {
                var key = Console.ReadKey();

                if (key.KeyChar == '1')
                {
                    ProcessSingleFile();
                    break;
                }
                else if (key.KeyChar == '2')
                {
                    ProcessProjectFile();
                    break;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
        }

        private static void ProcessSingleFile()
        {
            // Copy the "original" file so can easily rerun the program
            File.Copy("./OriginalDocument.xaml", "./TestDocument.xaml", overwrite: true);

            Console.WriteLine();
            Console.WriteLine("Original file contents");
            Console.WriteLine("----------------------");
            Console.WriteLine(File.ReadAllText("./TestDocument.xaml"));

            // This is the class that does the conversions
            var sut = new RapidXaml.XamlConverter();

            // This early preview only works on a single file.
            // But the intention is to support taking a csproj or vbproj file as input too.
            // The second parameter passed is a collection of ICustomAnalyzer implementations that document the changes to make as part of the conversion.
            var (success, details) = sut.ConvertFile("./TestDocument.xaml", new[] { new WebViewToWebView2Converter() });

            Console.WriteLine(success ? "File updated successfully" : "Error updating file");

            Console.WriteLine();

            Console.WriteLine("Details from converter");
            Console.WriteLine("----------------------");

            foreach (var line in details)
            {
                Console.WriteLine(line);
            }

            Console.WriteLine();
            Console.WriteLine("Modified file contents");
            Console.WriteLine("----------------------");
            Console.WriteLine(File.ReadAllText("./TestDocument.xaml"));
        }
        private static void ProcessProjectFile()
        {
            // Copy project folder to run through converter
            var sourcePath = "./SampleProject/";
            var destPath = "./TestProject/";

            if (Directory.Exists(destPath))
                Directory.Delete(destPath, recursive: true);

            // Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destPath));

            // Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, destPath), true);

            Console.WriteLine();
            Console.WriteLine("Converting './TestProject/MyDemoApp.csproj'");

            // This is the class that does the conversions
            var sut = new RapidXaml.XamlConverter();

            // This early preview only works on a single file.
            // But the intention is to support taking a csproj or vbproj file as input too.
            // The second parameter passed is a collection of ICustomAnalyzer implementations that document the changes to make as part of the conversion.
            var (success, details) = sut.ConvertAllFilesInProject("./TestProject/MyDemoApp.csproj", new[] { new WebViewToWebView2Converter() });

            Console.WriteLine(success ? "Project updated successfully" : "Error updating project");

            Console.WriteLine();

            Console.WriteLine("Details from converter");
            Console.WriteLine("----------------------");

            foreach (var line in details)
            {
                Console.WriteLine(line);
            }
        }
    }

    // This is what identifies the changes to make.
    public class WebViewToWebView2Converter : ICustomAnalyzer
    {
        // This indicates which elements in the XAML document this Analyzer will run against
        public string TargetType() => "WebView";

        public AnalysisActions Analyze(RapidXamlElement element, ExtraAnalysisDetails extraDetails)
        {
            // This is the simplest change to make.
            // It just says rename the element.
            // Checking for and then adding and/or removing attributes can also be done
            // There are helpers available to work with the `element` more easily.
            return AutoFixAnalysisActions.RenameElement("WebView2");
        }
    }
}

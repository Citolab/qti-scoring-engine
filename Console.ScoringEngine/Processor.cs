using Citolab.QTI.Scoring;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Console.Scoring
{
    public class Processor
    {
        private readonly ILogger<Processor> _logger;
        private readonly ProcessingSetting _settings;

        public Processor(ILogger<Processor> logger, ProcessingSetting settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public void Process()
        {
            var packageFolder = ExtractPackage(_settings.PackageFileLocation);
            var manifest = GetManifest(packageFolder);
            var assessmentResults = new DirectoryInfo(_settings.AssessmentResultFolderLocation).GetFiles("*.xml")
                .Select(file => new { Document = GetDocument(file.FullName), FileName = file.Name }).ToList();
            var qtiScoringEngine = new ScoringEngine();
            var scoredAssessmentResults = qtiScoringEngine.ProcessResponsesAndOutcomes(new ScoringContext
            {
                AssessmentItems = manifest.Items.Select(itemRef => GetDocument(Path.Combine(packageFolder.FullName, itemRef.Href))).ToList(),
                AssessmentTest = GetDocument(Path.Combine(packageFolder.FullName, manifest.Test.Href)),
                AssessmentmentResults = assessmentResults.Select(a => a.Document).ToList(),
                Logger = _logger
            });

             // write the output result
             var newBaseDir = Path.Combine(_settings.AssessmentResultFolderLocation, "processed");
            if (!Directory.Exists(newBaseDir))
            {
                Directory.CreateDirectory(newBaseDir);
            }
            assessmentResults.ForEach(assessmentResult =>
            {
                File.WriteAllText(Path.Combine(newBaseDir, assessmentResult.FileName), assessmentResult.Document.ToString());
            });
          
        }


        private static Manifest GetManifest(DirectoryInfo packageFolder)
        {
            var manifestFile = packageFolder
             .GetFiles("imsmanifest.xml", SearchOption.AllDirectories)
             .FirstOrDefault();
            if (manifestFile != null)
            {
                return new Manifest(manifestFile.Directory);
            }
            return null;
        }


        private static DirectoryInfo ExtractPackage(string packageRef)
        {
            // extract package
            var extractPath = new DirectoryInfo(Path.Combine(Path.Combine(Path.GetTempPath(), "_temp"), Path.GetFileNameWithoutExtension(Path.GetRandomFileName())));
            if (!extractPath.Exists) extractPath.Create();
            var packageFolder = new DirectoryInfo(Path.Combine(extractPath.FullName, "package"));

            if (!packageFolder.Exists) packageFolder.Create();
            using var packageStream = new FileStream(packageRef, FileMode.Open);
            using var packageZip = new ZipArchive(packageStream);
            packageZip.ExtractToDirectory(packageFolder.FullName);
            return packageFolder;
        }

        private static XDocument GetDocument(string file)
        {
            XDocument xDoc = null;
            using (var openRead = File.OpenRead(file))
            {
                xDoc = XDocument.Load(openRead);
            };
            return xDoc;
        }
    }

}

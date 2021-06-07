using Citolab.QTI.ScoringEngine;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            //for (int i = 0; i < 1000; i++)
            //{
            //    assessmentResults.Add(new { Document = XDocument.Parse(assessmentResults[0].Document.ToString()), FileName = assessmentResults[0].FileName });
            //}

            var qtiScoringEngine = new ScoringEngine();

            var results = assessmentResults.Select(a => a.Document).ToList();
            var sw = new Stopwatch();
            sw.Start();
            var scoredAssessmentResults = qtiScoringEngine.ProcessResponsesAndOutcomes(new ScoringContext
            {
                AssessmentItems = manifest.Items.Select(itemRef => GetDocument(Path.Combine(packageFolder.FullName, itemRef.Href))).ToList(),
                AssessmentTest = GetDocument(Path.Combine(packageFolder.FullName, manifest.Test.Href)),
                AssessmentmentResults = results,
                Logger = _logger
            });
            sw.Stop();
            _logger.LogInformation($"Total elapesed seconds: { sw.ElapsedMilliseconds / 1000}");
            // write the output result
            //var newBaseDir = Path.Combine(_settings.AssessmentResultFolderLocation, "processed");
            //if (!Directory.Exists(newBaseDir))
            //{
            //    Directory.CreateDirectory(newBaseDir);
            //}
            //var assessementIndex = 0;
            //assessmentResults.ForEach(assessmentResult =>
            //{
            //    var doc = scoredAssessmentResults[assessementIndex];
            //    assessementIndex++;
            //    File.WriteAllText(Path.Combine(newBaseDir, assessmentResult.FileName), doc.ToString());
            //});

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

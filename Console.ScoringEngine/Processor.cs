using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Console.ScoringEngine
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

            // setup items and tests
            var assessmentItems = manifest.Items.Select(itemRef =>
            {
                var assessmentItem = new AssessmentItem(_logger, GetDocument(Path.Combine(packageFolder.FullName, itemRef.Href)));
                return assessmentItem;
            }).ToList();

            var assessmentTest = new AssessmentTest(_logger, GetDocument(Path.Combine(packageFolder.FullName, manifest.Test.Href)));
            var responseProcessing = new ResponseProcessor();

            // search for assessmentResults
            var assessmentResultFolder = new DirectoryInfo(_settings.AssessmentResultFolderLocation);
            var assessmentResults = new List<AssessmentResult>();
            foreach (var assessmentRefHref in assessmentResultFolder.GetFiles("*.xml"))
            {
                var assessmentResult = new AssessmentResult(_logger, GetDocument(assessmentRefHref.FullName));
                foreach (var assessmentItem in assessmentItems)
                {
                    responseProcessing.Process(assessmentItem, assessmentResult, _logger);
                }
                var newBaseDir = Path.Combine(assessmentRefHref.DirectoryName, "processed");
                if (!Directory.Exists(newBaseDir))
                {
                    Directory.CreateDirectory(newBaseDir);
                }
                File.WriteAllText(Path.Combine(newBaseDir, assessmentRefHref.Name), assessmentResult.ToString());
            }
        }


        private Manifest GetManifest(DirectoryInfo packageFolder)
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


        private DirectoryInfo ExtractPackage(string packageRef)
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

        private XDocument GetDocument(string file)
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

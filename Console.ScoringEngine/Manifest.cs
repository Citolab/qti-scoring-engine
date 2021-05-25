using Citolab.QTI.ScoringEngine.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Console.ScoringEngine
{
    public class Manifest : XDocument
    {
        private readonly DirectoryInfo _packageLocation;
        private List<ResourceRef> _items = null;
        private List<ResourceRef> _genericResources = null;
        private ResourceRef _test = null;
        public Manifest(DirectoryInfo packageLocation) : 
            base(Parse(File.ReadAllText(Path.Combine(packageLocation.FullName, "imsmanifest.xml"))))
        {
            _packageLocation = packageLocation;
        }
        public ResourceRef Test => _test ??= GetTest();
        public List<ResourceRef> Items => _items ??= GetItems();
        public List<ResourceRef> GenericResources => _genericResources ??= GetGenericResources();

        public void AddImage(string filename, string itemId)
        {
            var identifier = $"RES-{Path.GetFileName(filename).Replace(".", "_")}";
            if (GenericResources.All(r => r.Identifier != identifier))
            {
                var imageElement = XElement.Parse($@"<resource identifier=""{identifier}"" type=""associatedcontent/xmlv1p0/learning-application-resource"" href=""img/%filename%"">
                                                    <file href = ""img/%filename%"" />
                                                </resource> ".Replace("%filename%", Path.GetFileName(filename)));
                this.FindElementsByName("resource").LastOrDefault()?.AddAfterSelf(imageElement);
                if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(filename)))
                    filename = Path.Combine(Path.GetDirectoryName(typeof(Manifest).GetTypeInfo().Assembly.Location), Path.Combine("Resources", filename));
                var imagePath = Path.Combine(_packageLocation.FullName, "img");
                if (!Directory.Exists(imagePath)) Directory.CreateDirectory(imagePath);
                File.Copy(filename, Path.Combine(imagePath, Path.GetFileName(filename)));
                _genericResources = GetGenericResources();
            }
            var itemElement = this.FindElementsByElementAndAttributeValue("resource", "type", "imsqti_item_xmlv2p1")
                .FirstOrDefault(i => i.GetAttributeValue("identifier").Equals(itemId, StringComparison.OrdinalIgnoreCase));

            itemElement.FindElementsByName("file").FirstOrDefault()
                ?.AddAfterSelf(XElement.Parse($@"<dependency identifierref=""{identifier}"" />"));
        }

        public void Save()
        {
            var content = this.ToString().Replace(@"xmlns=""""", string.Empty);
            File.WriteAllText(Path.Combine(_packageLocation.FullName, "imsmanifest.xml"), content);
        }

        private List<ResourceRef> GetItems()
        {
            var items =
                this.FindElementsByElementAndAttributeThatContainsValue("resource", "type", "_item_").
                Select(d =>
                {
                    return new ResourceRef()
                    {
                        Identifier = d.GetAttributeValue("identifier"),
                        Href = d.GetAttributeValue("href")
                    };
                }).ToList();
            return items;
        }

        private List<ResourceRef> GetGenericResources()
        {
            var genericResources = this.FindElementsByElementAndAttributeThatContainsValue("resource", "type", "learning-application-resource")
                .Concat(this.FindElementsByElementAndAttributeValue("resource", "type", "webcontent"));
            return genericResources.
                Select(d => new ResourceRef()
                {
                    Identifier = d.GetAttributeValue("identifier"),
                    Href = d.GetAttributeValue("href")
                }).ToList();
        }

        private ResourceRef GetTest()
        {
            return this.FindElementsByElementAndAttributeThatContainsValue("resource", "type", "_test_").
                Select(d => new ResourceRef()
                {
                    Identifier = d.GetAttributeValue("identifier"),
                    Href = d.GetAttributeValue("href")
                }).FirstOrDefault();
        }

    }
}

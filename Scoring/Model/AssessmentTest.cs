using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class AssessmentTest : QtiDocument
    {
        public Dictionary<string, OutcomeDeclaration> OutcomeDeclarations;
        public Dictionary<string, AssessmentItemRef> AssessmentItemRefs;
        public List<string> Categories;
        public HashSet<string> CalculatedOutcomes;
        public XElement OutcomeProcessingElement => Content.FindElementByName("qti-outcome-processing");
        public AssessmentTest(ILogger logger, XDocument assessmentTest) : base(logger, assessmentTest)
        {
            Init();
        }

        public void Init()
        {
            OutcomeDeclarations = Content.FindElementsByName("qti-outcome-declaration").Select(outcomeDeclaration =>
            {
                return GetOutcomeDeclaration(outcomeDeclaration);
            }).ToDictionary(o => o.Identifier, o => o);

            AssessmentItemRefs = Content.FindElementsByName("qti-assessment-item-ref")
               .Select(assessmentItemRefElement =>
               {
                   var itemIdentifier = assessmentItemRefElement.Identifier();

                   var assessmentItemRef = new AssessmentItemRef
                   {
                       Identifier = itemIdentifier,
                       Weights = assessmentItemRefElement.FindElementsByName("qti-weight").Select(weight =>
                       {
                           var weightString = weight.GetAttributeValue("value");
                           if (int.TryParse(weightString, out var weightValue))
                           {
                               return new { Value = weightValue, Identifier = weight.Identifier() };
                           }
                           else
                           {
                               Logger.LogError($"sourceId weight: {weightString} cannot be parsed to int.");
                               return null;
                           }
                       }).Where(w => w != null)
                       .ToDictionary(w => w.Identifier, w => w.Value),
                       Categories = assessmentItemRefElement.GetAttributeValue("category").Split(' ')?.ToHashSet(),
                   };
                   return assessmentItemRef;
               }).ToDictionary(a => a.Identifier, a => a);
            Categories = AssessmentItemRefs.Values
                .SelectMany(itemRef => itemRef.Categories)
                .Distinct()
                .ToList();

            var setOutcomeElements = OutcomeProcessingElement?
              .FindElementsByName("qti-set-outcome-value");
            var setOutcomes = setOutcomeElements?.Select(v => v.Identifier());
            CalculatedOutcomes = setOutcomes.Distinct().ToHashSet();
        }

        public override void Upgrade()
        {
            Upgrade(Content);
        }

        public static void Upgrade(XDocument doc)
        {
            XNamespace xNamespace = "http://www.imsglobal.org/xsd/imsqtiasi_v3p0";
            foreach (var element in doc.Descendants())
            {
                var tagName = element.Name.LocalName;
                var kebabTagName = tagName.ToKebabCase();
                element.Name = xNamespace + $"qti-{kebabTagName}";
            }

            // fix attributes
            foreach (var element in doc.Descendants())
            {
                var attributesToRemove = new List<XAttribute>();
                var attributesToAdd = new List<XAttribute>();
                foreach (var attribute in element.Attributes()
                    .Where(attr => !attr.IsNamespaceDeclaration && string.IsNullOrEmpty(attr.Name.NamespaceName)))
                {
                    var attributeName = attribute.Name.LocalName;
                    var kebabAttributeName = attributeName.ToKebabCase();
                    if (attributeName != kebabAttributeName)
                    {
                        var newAttr = new XAttribute($"{kebabAttributeName}", attribute.Value);
                        attributesToRemove.Add(attribute);
                        attributesToAdd.Add(newAttr);
                    }
                }
                attributesToRemove.ForEach(a => a.Remove());
                attributesToAdd.ForEach(a => element.Add(a));
            }
        }
    }
}

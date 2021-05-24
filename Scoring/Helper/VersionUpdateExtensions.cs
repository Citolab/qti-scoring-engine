using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Char;

namespace Citolab.QTI.ScoringEngine.Helper
{
    public static class VersionUpdateExtensions
    {

        public static XElement Convert(this XElement element)
        {
            var firstElement = element.FirstNode.ToString();
            switch (firstElement)
            {
                case "assessmentItem":
                    {
                        return element.ConvertAssessmentItem();
                    }
                case "assessmentTest":
                    {
                        return element.ConvertAssessmentTest();
                    }
            }
            return element;
        }

        public static XElement ConvertAssessmentTest(this XElement element)
        {

            return element;
        }

        public static XElement ConvertAssessmentItem(this XElement element)
        {

            return element;
        }


        public static string ReplaceRunsTabsAndLineBraks(this string text)
        {
            return text
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Replace("\r", " ");
        }

        private static string GetBaseSchema(QtiResourceType resourceType, QtiVersion version)
            => XsdHelper.BaseSchemas[$"{resourceType}-{version}"];

        public static string GetBaseSchemaLocation(QtiResourceType resourceType, QtiVersion version)
            => XsdHelper.BaseSchemaLocations[$"{resourceType.ToString()}-{version.ToString()}"];


        public static string ReplaceSchemas(this string xml, QtiResourceType resourceType, QtiVersion newVersion, QtiVersion oldVersion, bool localSchema)
        {
            var tagName = resourceType == QtiResourceType.AssessmentItem ? "assessmentItem" :
                resourceType == QtiResourceType.AssessmentTest ? "assessmentTest" :
                "manifest";
            var baseScheme = GetBaseSchema(resourceType, newVersion);
            var baseSchemeLocation = GetBaseSchemaLocation(resourceType, newVersion);
            var qtiParentTag = Regex.Match(xml, $"<{tagName}(.*?)>").Value;
            var qtiParentOrg = qtiParentTag;
            var schemaPrefix = Regex.Match(qtiParentTag, " (.*?)schemaLocation")
                .Value.Replace(":schemaLocation", "").Trim();
            schemaPrefix = schemaPrefix.Substring(schemaPrefix.LastIndexOf(" ", StringComparison.Ordinal), schemaPrefix.Length - schemaPrefix.LastIndexOf(" ", StringComparison.Ordinal)).Trim();

            var schemaLocations = Regex.Match(qtiParentTag, @$"{schemaPrefix}:schemaLocation=""(.*?)""").Value;
            var extensionSchemas = RemoveSchemaFromLocation(schemaLocations, GetBaseSchema(resourceType, oldVersion));
            if (resourceType == QtiResourceType.Manifest)
            {
                extensionSchemas = RemoveSchemaFromLocation(extensionSchemas, GetBaseSchema(QtiResourceType.AssessmentItem, oldVersion));
                extensionSchemas = extensionSchemas +
                                   $" {GetBaseSchema(QtiResourceType.AssessmentItem, newVersion)} {GetBaseSchemaLocation(QtiResourceType.AssessmentItem, newVersion)}";
            }

            qtiParentTag = Regex.Replace(qtiParentTag, @$"{schemaPrefix}:schemaLocation=""(.*?)""", "");
            qtiParentTag = Regex.Replace(qtiParentTag, @$"xmlns:{schemaPrefix}=""(.*?)""", "");
            qtiParentTag = Regex.Replace(qtiParentTag, @$"xmlns:xsi=""(.*?)""", "");
            qtiParentTag = Regex.Replace(qtiParentTag, @"xmlns=""(.*?)""", "");

            var schemaLocation = $"xsi:schemaLocation=\"{baseScheme}  {baseSchemeLocation} ";

            qtiParentTag = qtiParentTag.Replace($"{tagName} ",
                $@"{tagName} xmlns=""{baseScheme}"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" " +
                schemaLocation + "  " + extensionSchemas + @""" "
            );

            xml = xml.Replace(qtiParentOrg, qtiParentTag);
            if (schemaPrefix != "xsi")
            {
                xml = xml.Replace($":{schemaPrefix}", ":xsi");
                xml = xml.Replace($"{schemaPrefix}:", "xsi:");
            }
            return xml;
        }

        private static string RemoveSchemaFromLocation(string schemaLocations, string schemaToRemove)
        {
            var schemas = schemaLocations.Split(" ")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            var schemaIndex = schemas.FindIndex(s => s.IndexOf(schemaToRemove, StringComparison.Ordinal) != -1);
            var extensionSchemas = schemaIndex == -1
                ? schemaLocations
                : string.Join(" ", schemas.Where((schema) =>
                        schemas.IndexOf(schema) != schemaIndex &&
                        schemas.IndexOf(schema) != 1 + schemaIndex)
                    .ToArray());
            return extensionSchemas.Trim('"');
        }

        public static string GetElementValue(this XElement el, string name)
        {
            return el.FindElementsByName(name).FirstOrDefault()?.Value ?? String.Empty;
        }

        public static bool IsAlphaNumeric(this char strToCheck)
        {
            var rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(strToCheck.ToString());
        }
              
           /// <summary>
        /// Force tags to be non-selfclosing
        /// </summary>
        /// <param name="document"></param>
        public static void ForceTags(this XDocument document)
        {
            var allowedSelfClosingTags = new HashSet<string>
            {
                "br", "img", "qti-stylesheet", "qti-text-entry-interaction"
            };
            foreach (var childElement in
                from x in document.DescendantNodes().OfType<XElement>()
                where x.IsEmpty && !allowedSelfClosingTags.Contains(x.Name.LocalName.ToLower())
                select x)
            {
                childElement.Value = string.Empty;
            }
        }


        public static string ReplaceAllOccurrenceExceptFirst(this string source, string find, string replace)
        {
            var result = source;
            while (result.CountStringOccurrences(find) > 1)
            {
                var place = result.LastIndexOf(find, StringComparison.Ordinal);
                if (place == -1)
                    return result;
                result = result.Remove(place, find.Length).Insert(place, replace);
            }

            return result;
        }

        public static int CountStringOccurrences(this string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            var count = 0;
            var i = 0;
            while ((i = text.IndexOf(pattern, i, StringComparison.Ordinal)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        public static string ToKebabCase(this string source)
        {
            if (source is null) return null;

            if (source.Length == 0) return string.Empty;

            var builder = new StringBuilder();

            for (var i = 0; i < source.Length; i++)
            {
                if (IsLower(source[i])) // if current char is already lowercase
                {
                    builder.Append(source[i]);
                }
                else if (i == 0) // if current char is the first char
                {
                    builder.Append(ToLower(source[i]));
                }
                else if (IsLower(source[i - 1])) // if current char is upper and previous char is lower
                {
                    builder.Append('-');
                    builder.Append(ToLower(source[i]));
                }
                else if (i + 1 == source.Length || IsUpper(source[i + 1])) // if current char is upper and next char doesn't exist or is upper
                {
                    builder.Append(ToLower(source[i]));
                }
                else // if current char is upper and next char is lower
                {
                    builder.Append('-');
                    builder.Append(ToLower(source[i]));
                }
            }

            return builder.ToString();
        }
    }
}

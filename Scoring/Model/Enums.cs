using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citolab.QTI.ScoringEngine.Model
{
    public enum ProcessingType
    {
        ResponseProcessig,
        OutcomeProcessing
    }
    public enum BaseType
    {

        Identifier,
        String,
        Float,
        Boolean,
        Int,
        Point,
        // A point value represents an integer tuple corresponding to a graphic point. The two integers correspond to the horizontal (x-axis) and vertical (y-axis) positions respectively. The up/down and left/right senses of the axes are context dependent.
        Pair,
        // A pair value represents a pair of identifiers corresponding to an association between two objects. The association is undirected so (A, B) and (B, A) are equivalent.
        DirectedPair,
        // A directedPair value represents a pair of identifiers corresponding to a directed association between two objects. The two identifiers correspond to the source and destination objects.
        Duration,
        // A duration value specifies a distance (in time) between two time points. In other words, a time period as defined by [ISO8601], but represented as a single float that records time in seconds. Durations may have a fractional part. Durations are represented using the xsd:double datatype rather than xsd:dateTime for convenience and backward compatibility.
        file,
        // A file value is any sequence of octets (bytes) qualified by a content-type and an optional filename given to the file (for example, by the candidate when uploading it as part of an interaction). The content type of the file is one of the MIME types defined by[RFC2045]
        Uri,
        // A URI value is a Uniform Resource Identifier as defined by[URI].
        IntOrIdentifier
    }

    internal static class EnumHelper
    {
        public static string GetString(this Cardinality cardinality)
        {
            switch (cardinality)
            {
                case Cardinality.Single:
                    {
                        return "single";
                    }
                case Cardinality.Multiple:
                    {
                        return "multiple";
                    }
                case Cardinality.Ordered:
                    {
                        return "ordered";
                    }
            }
            return "single";
        }

        public static string GetString(this Shape shape)
        {
            switch (shape)
            {
                case Shape.Circle:
                    {
                        return "circle";
                    }
                case Shape.Ellipse:
                    {
                        return "ellipse";
                    }
                case Shape.Poly:
                    {
                        return "poly";
                    }
                case Shape.Rect:
                    {
                        return "rect";
                    }
                case Shape.Default:
                    {
                        return "default";
                    }
            }
            return "circle";
        }

        internal static string GetString(this BaseType baseType)
        {
            switch (baseType)
            {
                case BaseType.Identifier:
                    {
                        return "identifier";
                    }
                case BaseType.Float:
                    {
                        return "float";
                    }
                case BaseType.Int:
                    {
                        return "integer";
                    }
                case BaseType.String:
                    {
                        return "string";
                    }
                case BaseType.Pair:
                    {
                        return "pair";
                    }
                case BaseType.DirectedPair:
                    {
                        return "directedPair";
                    }
                case BaseType.Point:
                    return "point";
                case BaseType.Boolean:
                    return "boolean";
                case BaseType.Duration:
                    return "duration";
                case BaseType.file:
                    return "file";
                case BaseType.Uri:
                    return "uri";
                case BaseType.IntOrIdentifier:
                    return "intOrIdentifier";
            }
            return "string";
        }


        private static readonly Dictionary<string, BaseType> BaseTypesByName =
            new Dictionary<string, BaseType>(StringComparer.OrdinalIgnoreCase)
            {
                { "identifier", BaseType.Identifier },
                { "float", BaseType.Float },
                { "integer", BaseType.Int },
                { "string", BaseType.String },
                { "pair", BaseType.Pair },
                { "directedPair", BaseType.DirectedPair },
                { "point", BaseType.Point },
                { "boolean", BaseType.Boolean },
                { "duration", BaseType.Duration },
                { "file", BaseType.file },
                { "uri", BaseType.Uri },
                { "intOrIdentifier", BaseType.IntOrIdentifier }
            };

        /// <summary>
        /// Falls back to string, as it always has, but says so: a base-type that is not
        /// recognised used to be scored as a string with nothing written to the log.
        /// </summary>
        internal static BaseType ToBaseType(this string baseTypeString, ILogger logger = null)
        {
            if (string.IsNullOrEmpty(baseTypeString))
            {
                return BaseType.String;
            }
            if (BaseTypesByName.TryGetValue(baseTypeString.Trim(), out var baseType))
            {
                return baseType;
            }
            logger?.LogWarning($"Unknown base-type: {baseTypeString}. Treating it as a string.");
            return BaseType.String;
        }

        internal static Cardinality ToCardinality(this string cardinalityString)
        {
            switch (cardinalityString)
            {
                case "single":
                    {
                        return Cardinality.Single;
                    }
                case "multiple":
                    {
                        return Cardinality.Multiple;
                    }
                case "ordered":
                    {
                        return Cardinality.Ordered;
                    }
            }
            return Cardinality.Single;
        }

        internal static Shape ToShape(this string shapeString)
        {
            switch (shapeString)
            {
                case "circle":
                    {
                        return Shape.Circle;
                    }
                case "ellipse":
                    {
                        return Shape.Ellipse;
                    }
                case "poly":
                    {
                        return Shape.Poly;
                    }
                case "rect":
                    {
                        return Shape.Rect;
                    }
                case "default":
                    {
                        return Shape.Default;
                    }
            }
            return Shape.Circle;
        }
    }


    public enum Cardinality
    {
        Single,
        Multiple,
        Ordered
    }
    public enum Shape
    {
        Circle,
        Ellipse,
        Poly,
        Rect,
        Default
    }

}


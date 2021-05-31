using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citolab.QTI.ScoringEngine.Model
{
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
                        return "int";
                    }
                case BaseType.String:
                    {
                        return "string";
                    }
            }
            return "string";
        }


        internal static BaseType ToBaseType(this string baseTypeString)
        {
            switch (baseTypeString)
            {
                case "identifier":
                    {
                        return BaseType.Identifier;
                    }
                case "float":
                    {
                        return BaseType.Float;
                    }
                case "integer":
                    {
                        return BaseType.Int;
                    }
                case "string":
                    {
                        return BaseType.String;
                    }
            }
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
    }


    internal enum Cardinality
    {
        Single,
        Multiple,
        Ordered
    }

    public enum QtiVersion
    {
        Qti21,
        Qti22,
        Qti30
    }

    internal enum QtiResourceType
    {
        AssessmentItem,
        AssessmentTest,
        Manifest
    }
}


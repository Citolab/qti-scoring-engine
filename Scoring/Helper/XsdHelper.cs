using Citolab.QTI.Scoring.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citolab.QTI.Scoring.Helper
{
    internal static class XsdHelper
    {

        public static Dictionary<string, string> BaseSchemaLocations = new Dictionary<string, string>
        {
            {$"{QtiResourceType.AssessmentItem}-{QtiVersion.Qti21}", "http://www.imsglobal.org/xsd/qti/qtiv2p1/imsqti_v2p1.xsd"},
            {$"{QtiResourceType.AssessmentItem}-{QtiVersion.Qti22}", "http://www.imsglobal.org/xsd/qti/qtiv2p2/imsqti_v2p2p2.xsd"},
            {$"{QtiResourceType.AssessmentItem}-{QtiVersion.Qti30}", "https://purl.imsglobal.org/spec/qti/v3p0/schema/xsd/imsqti_asiv3p0_v1p0.xsd"},
            {$"{QtiResourceType.AssessmentTest}-{QtiVersion.Qti21}",  "http://www.imsglobal.org/xsd/qti/qtiv2p1/imsqti_v2p1.xsd"},
            {$"{QtiResourceType.AssessmentTest}-{QtiVersion.Qti22}", "http://www.imsglobal.org/xsd/qti/qtiv2p2/imsqti_v2p2p2.xsd"},
            {$"{QtiResourceType.AssessmentTest}-{QtiVersion.Qti30}", "https://purl.imsglobal.org/spec/qti/v3p0/schema/xsd/imsqti_asiv3p0_v1p0.xsd"},
            {$"{QtiResourceType.Manifest}-{QtiVersion.Qti21}", "http://www.imsglobal.org/xsd/imscp_v1p2.xsd"},
            {$"{QtiResourceType.Manifest}-{QtiVersion.Qti22}", " http://www.imsglobal.org/xsd/qti/qtiv2p2/qtiv2p2_imscpv1p2_v1p0.xsd"},
            {$"{QtiResourceType.Manifest}-{QtiVersion.Qti30}", "https://purl.imsglobal.org/spec/qti/v3p0/schema/xsd/imsqtiv3p0_imscpv1p2_v1p0.xsd"},
        };

        public static Dictionary<string, string> BaseSchemas = new Dictionary<string, string>
        {
            {$"{QtiResourceType.AssessmentItem}-{QtiVersion.Qti21}", "http://www.imsglobal.org/xsd/imsqti_v2p1"},
            {$"{QtiResourceType.AssessmentItem}-{QtiVersion.Qti22}", "http://www.imsglobal.org/xsd/imsqti_v2p2"},
            {$"{QtiResourceType.AssessmentItem}-{QtiVersion.Qti30}", "http://www.imsglobal.org/xsd/imsqtiasi_v3p0"},
            {$"{QtiResourceType.AssessmentTest}-{QtiVersion.Qti21}", "http://www.imsglobal.org/xsd/imsqti_v2p1"},
            {$"{QtiResourceType.AssessmentTest}-{QtiVersion.Qti22}", "http://www.imsglobal.org/xsd/imsqti_v2p2"},
            {$"{QtiResourceType.AssessmentTest}-{QtiVersion.Qti30}", "http://www.imsglobal.org/xsd/imsqtiasi_v3p0"},
            {$"{QtiResourceType.Manifest}-{QtiVersion.Qti21}", "http://www.imsglobal.org/xsd/imscp_v1p1"},
            {$"{QtiResourceType.Manifest}-{QtiVersion.Qti22}", "http://www.imsglobal.org/xsd/imscp_v1p1"},
            {$"{QtiResourceType.Manifest}-{QtiVersion.Qti30}", "http://www.imsglobal.org/xsd/qti/qtiv3p0/imscp_v1p1"},
        };
    }
}

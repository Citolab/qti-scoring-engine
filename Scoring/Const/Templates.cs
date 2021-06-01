using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Const
{
    public class Templates
    {
        internal static XElement MatchCorrect = XElement.Parse(@"<responseProcessing><responseCondition><responseIf><match><variable identifier=""RESPONSE""/><correct identifier=""RESPONSE""/></match><setOutcomeValue identifier=""SCORE""><baseValue baseType=""float"">1</baseValue></setOutcomeValue></responseIf><responseElse><setOutcomeValue identifier=""SCORE""><baseValue baseType=""float"">0</baseValue></setOutcomeValue></responseElse></responseCondition></responseProcessing>");
        internal static XElement MapResponse = XElement.Parse(@"<responseProcessing><responseCondition><responseIf><isNull><variable identifier=""RESPONSE""/></isNull><setOutcomeValue identifier=""SCORE""><baseValue baseType=""float"">0.0</baseValue></setOutcomeValue></responseIf><responseElse><setOutcomeValue identifier=""SCORE""><mapResponse identifier=""RESPONSE""/></setOutcomeValue></responseElse></responseCondition></responseProcessing>");
        internal static XElement MapResponsePoint = XElement.Parse(@"<responseProcessing xsi:schemaLocation=""http://www.imsglobal.org/xsd/imsqti_v2p2 http://www.imsglobal.org/xsd/qti/qtiv2p2/imsqti_v2p2.xsd"" xmlns=""http://www.imsglobal.org/xsd/imsqti_v2p2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><responseCondition><responseIf><isNull><variable identifier=""RESPONSE""/></isNull><setOutcomeValue identifier=""SCORE""><baseValue baseType=""float"">0</baseValue></setOutcomeValue></responseIf><responseElse><setOutcomeValue identifier=""SCORE""><mapResponsePoint identifier=""RESPONSE""/></setOutcomeValue></responseElse></responseCondition></responseProcessing>");
    }
}

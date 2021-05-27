using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.Scoring.Const
{
    public class Templates
    {
        internal static XElement MatchCorrect = XElement.Parse(@"<responseProcessing><responseCondition><responseIf><match><variable identifier=""RESPONSE""/><correct identifier=""RESPONSE""/></match><setOutcomeValue identifier=""SCORE""><baseValue baseType=""float"">1</baseValue></setOutcomeValue></responseIf><responseElse><setOutcomeValue identifier=""SCORE""><baseValue baseType=""float"">0</baseValue></setOutcomeValue></responseElse></responseCondition></responseProcessing>");
        internal static XElement MapResponse = XElement.Parse(@"<responseProcessing><responseCondition><responseIf><isNull><variable identifier=""RESPONSE""/></isNull><setOutcomeValue identifier=""SCORE""><baseValue baseType=""float"">0.0</baseValue></setOutcomeValue></responseIf><responseElse><setOutcomeValue identifier=""SCORE""><mapResponse identifier=""RESPONSE""/></setOutcomeValue></responseElse></responseCondition></responseProcessing>");
    }
}

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
        internal static XElement MatchCorrect = XElement.Parse(@"<qti-response-processing> <qti-response-condition> <qti-response-if> <qti-match> <qti-variable identifier=""RESPONSE""/> <qti-correct identifier=""RESPONSE""/> </qti-match> <qti-set-outcome-value identifier=""SCORE""> <qti-base-value base-type=""float"">1</qti-base-value> </qti-set-outcome-value> </qti-response-if> <qti-response-else> <qti-set-outcome-value identifier=""SCORE""> <qti-base-value base-type=""float"">0</qti-base-value> </qti-set-outcome-value> </qti-response-else> </qti-response-condition> </qti-response-processing>");
        internal static XElement MapResponse = XElement.Parse(@"<qti-response-processing><qti-response-condition><qti-response-if><qti-is-null><qti-variable identifier=""RESPONSE""/></qti-is-null><qti-set-outcome-value identifier=""SCORE""><qti-base-value base-type=""float"">0.0</qti-base-value></qti-set-outcome-value></qti-response-if><qti-response-else><qti-set-outcome-value identifier=""SCORE""><qti-map-response identifier=""RESPONSE""/></qti-set-outcome-value></qti-response-else></qti-response-condition></qti-response-processing>");
        internal static XElement MapResponsePoint = XElement.Parse(@"<qti-response-processing><qti-response-condition><qti-response-if><qti-is-null><qti-variable identifier=""RESPONSE""/></qti-is-null><qti-set-outcome-value identifier=""SCORE""><qti-base-value base-type=""float"">0</qti-base-value></qti-set-outcome-value></qti-response-if><qti-response-else><qti-set-outcome-value identifier=""SCORE""><qti-map-response-point identifier=""RESPONSE""/></qti-set-outcome-value></qti-response-else></qti-response-condition></qti-response-processing>");
    }
}

using Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression;
using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.OutcomeProcessing.BaseValueExpression;
using Citolab.QTI.ScoringEngine.ResponseProcessing.CustomOperators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.ScoringEngine.Const
{
    internal static class Mappings
    {
        internal static Dictionary<string, Type> ConditionalExpressions =
            new Dictionary<string, Type>
            {
                { "qti-and", typeof (And) },
                { "qti-equal", typeof (Equal) },
                { "qti-equal-rounded", typeof (EqualRounded) },
                { "qti-gt", typeof (Gt) },
                { "qti-gte", typeof (Gte) },
                { "qti-is-null", typeof (IsNull) },
                { "qti-lookup-outcome-value", typeof(LookupOutcomeValue) },
                { "qti-lt", typeof(Lt) },
                { "qti-lte", typeof(Lte) },
                { "qti-match",  typeof(Match) },
                { "qti-member", typeof(Member) },
                { "qti-not", typeof(Not) },
                { "qti-or", typeof(Or) },
                { "qti-outcome-condition", typeof(OutcomeCondition) },
                { "qti-outcome-else", typeof(OutcomeElse) },
                { "qti-outcome-else-if", typeof(OutcomeElseIf) },
                { "qti-outcome-if", typeof(OutcomeIf) },
                { "qti-response-condition", typeof(ResponseCondition) },
                { "qti-response-else", typeof(ResponseElse) },
                { "qti-response-else-if", typeof(ResponseElseIf) },
                { "qti-response-if", typeof(ResponseIf) },
                { "qti-set-outcome-value", typeof(SetOutcomeValue) },
                { "qti-string-match", typeof(StringMatch) },
                { "qti-substring" , typeof(Substring) }
            };

        internal static Dictionary<string, Type> ValueExpressions =
            new Dictionary<string, Type>
            {
                { "qti-ordered", typeof (Ordered) },
                { "qti-round", typeof (Round) },
                { "qti-sum", typeof(Sum) },
                { "qti-correct", typeof(Correct) },
                { "qti-custom-operator", typeof(CustomOperator) },
                { "qti-map-response", typeof(MapResponse) },
                { "qti-map-response-point", typeof(MapResponsePoint) },
                { "qti-null", typeof(Null) },
                { "qti-number-correct", typeof(NumberCorrect) }, // not supported but added to supress error
                { "qti-number-selected", typeof(NumberSelected) },
                { "qti-base-value", typeof(QtiBaseValue) },
                { "qti-test-variables", typeof(TestVariable) },
                { "qti-variable", typeof(VariableProcessing) } // the implementation of outcome and responseprocessing differs, proxy class will init correct class
            };

        internal static Dictionary<string, ICustomOperator> CustomOperators = new Dictionary<string, ICustomOperator>
           {
                { "depcp:ParseCommaDecimal", new ParseCommaDecimal() },
                { "questify:ParseCommaDecimal", new ParseCommaDecimal() },
                { "qade:ParseCommaDecimal", new ParseCommaDecimal() },
                { "depcp:ToAscii", new ToAscii() },
                { "questify:ToAscii", new ToAscii() },
                { "qade:ToAscii", new ToAscii() },
                { "depcp:Trim", new Trim() },
                { "questify:Trim", new Trim() },
                { "qade:Trim", new Trim() }
           };
    }
}

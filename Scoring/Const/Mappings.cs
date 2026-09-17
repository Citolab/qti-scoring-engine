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
                { "qti-any-n", typeof (AnyN) },
                { "qti-contains", typeof (Contains) },
                { "qti-duration-gte", typeof (DurationGte) },
                { "qti-duration-lt", typeof (DurationLt) },
                { "qti-equal", typeof (Equal) },
                { "qti-exit-response", typeof (ExitResponse) },
                { "qti-inside", typeof (Inside) },
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
                { "qti-pattern-match", typeof(PatternMatch) },
                { "qti-response-condition", typeof(ResponseCondition) },
                { "qti-response-else", typeof(ResponseElse) },
                { "qti-response-else-if", typeof(ResponseElseIf) },
                { "qti-response-if", typeof(ResponseIf) },
                { "qti-response-processing-fragment", typeof(ResponseProcessingFragment) },
                { "qti-set-outcome-value", typeof(SetOutcomeValue) },
                { "qti-string-match", typeof(StringMatch) },
                { "qti-substring" , typeof(Substring) }
            };

        internal static Dictionary<string, Type> ValueExpressions =
            new Dictionary<string, Type>
            {
                { "qti-container-size", typeof (ContainerSize) },
                { "qti-default", typeof (Expressions.GeneralExpressions.Default) },
                { "qti-delete", typeof (Delete) },
                { "qti-divide", typeof (Divide) },
                { "qti-gcd", typeof (Gcd) },
                { "qti-index", typeof (Expressions.BaseValueExpression.Index) },
                { "qti-integer-divide", typeof (IntegerDivide) },
                { "qti-integer-modulus", typeof (IntegerModulus) },
                { "qti-integer-to-float", typeof (IntegerToFloat) },
                { "qti-lcm", typeof (Lcm) },
                { "qti-math-constant", typeof (MathConstant) },
                { "qti-math-operator", typeof (MathOperator) },
                { "qti-multiple", typeof (Multiple) },
                { "qti-ordered", typeof (Ordered) },
                { "qti-power", typeof (Power) },
                { "qti-product", typeof (Product) },
                { "qti-random", typeof (Expressions.BaseValueExpression.Random) },
                { "qti-repeat", typeof (Repeat) },
                { "qti-round", typeof (Round) },
                { "qti-round-to", typeof (RoundTo) },
                { "qti-stats-operator", typeof (StatsOperator) },
                { "qti-truncate", typeof (Truncate) },
                { "qti-sum", typeof(Sum) },
                { "qti-subtract", typeof(Subtract) },
                { "qti-min", typeof(Min) },
                { "qti-max", typeof(Max) },
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

        /// <summary>
        /// The built-in operators. Treated as a read-only template: ExpressionFactory copies
        /// these into its own registry rather than adding the caller's operators here.
        /// </summary>
        internal static readonly Dictionary<string, ICustomOperator> CustomOperators = new Dictionary<string, ICustomOperator>
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

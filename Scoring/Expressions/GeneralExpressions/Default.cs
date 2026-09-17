using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System.Collections.Generic;

namespace Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions
{
    /// <summary>
    /// qti-default: the declared default value of an outcome or response variable, NULL when
    /// the declaration has none.
    /// </summary>
    internal class Default : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            var identifier = GetAttributeValue("identifier");
            if (string.IsNullOrEmpty(identifier))
            {
                ctx.LogError("qti-default is missing the identifier attribute.");
                return null;
            }
            if (ctx.OutcomeDeclarations != null && ctx.OutcomeDeclarations.ContainsKey(identifier))
            {
                var outcomeDeclaration = ctx.OutcomeDeclarations[identifier];
                if (outcomeDeclaration.Cardinality == Cardinality.Record)
                {
                    return ToRecord(identifier, outcomeDeclaration.BaseType, outcomeDeclaration.DefaultFields);
                }
                var defaultValue = outcomeDeclaration.DefaultValue?.ToString();
                if (defaultValue == null)
                {
                    return null;
                }
                return new BaseValue
                {
                    Identifier = identifier,
                    BaseType = outcomeDeclaration.BaseType,
                    Cardinality = outcomeDeclaration.Cardinality,
                    Value = defaultValue
                };
            }
            if (ctx.ResponseDeclarations != null && ctx.ResponseDeclarations.ContainsKey(identifier))
            {
                var responseDeclaration = ctx.ResponseDeclarations[identifier];
                if (responseDeclaration.Cardinality == Cardinality.Record)
                {
                    return ToRecord(identifier, responseDeclaration.BaseType, responseDeclaration.DefaultFields);
                }
                if (responseDeclaration.DefaultValues == null || responseDeclaration.DefaultValues.Count == 0)
                {
                    return null;
                }
                return new BaseValue
                {
                    Identifier = identifier,
                    BaseType = responseDeclaration.BaseType,
                    Cardinality = responseDeclaration.Cardinality,
                    Value = responseDeclaration.Cardinality == Cardinality.Single ? responseDeclaration.DefaultValues[0] : null,
                    Values = responseDeclaration.Cardinality == Cardinality.Single ? null : responseDeclaration.DefaultValues
                };
            }
            ctx.LogError($"qti-default cannot find a declaration for identifier: {identifier}");
            return null;
        }

        private static BaseValue ToRecord(string identifier, BaseType baseType, Dictionary<string, BaseValue> defaultFields)
        {
            if (defaultFields == null)
            {
                return null;
            }
            return new BaseValue
            {
                Identifier = identifier,
                BaseType = baseType,
                Cardinality = Cardinality.Record,
                Fields = defaultFields.Copy()
            };
        }
    }
}

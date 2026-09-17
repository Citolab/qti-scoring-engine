using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;

namespace Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions
{
    /// <summary>
    /// qti-field-value: one named field of a record, for instance a field of QTI_CONTEXT or of
    /// what a custom operator returned. A field that is not in the record is NULL.
    /// </summary>
    internal class FieldValue : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            var fieldIdentifier = GetAttributeValue("field-identifier");
            if (string.IsNullOrEmpty(fieldIdentifier))
            {
                // an item that was not upgraded from 2.x still spells it in camel case
                fieldIdentifier = GetAttributeValue("fieldIdentifier");
            }
            if (string.IsNullOrEmpty(fieldIdentifier))
            {
                ctx.LogError("qti-field-value is missing the field-identifier attribute.");
                return null;
            }
            if (expressions.Count != 1)
            {
                ctx.LogError($"qti-field-value should have exactly one child expression, found: {expressions.Count}");
                return null;
            }
            var record = expressions[0].Apply(ctx);
            if (record == null)
            {
                return null;
            }
            if (record.Cardinality != Cardinality.Record || record.Fields == null)
            {
                ctx.LogError($"qti-field-value: {record.Identifier} is not a record, field {fieldIdentifier} cannot be read from it.");
                return null;
            }
            if (!record.Fields.TryGetValue(fieldIdentifier, out var field) || field == null)
            {
                ctx.LogInformation($"qti-field-value: record {record.Identifier} has no field {fieldIdentifier}, returning null.");
                return null;
            }
            // a copy, because the expression tree is shared by every result and callers such as
            // the custom operators write to the value they are handed.
            return new BaseValue
            {
                Identifier = field.Identifier ?? fieldIdentifier,
                BaseType = field.BaseType,
                Cardinality = Cardinality.Single,
                Value = field.Value
            };
        }
    }
}

using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-math-operator: the built-in mathematical function named by the name attribute.
    /// Angles are in radians. A function that is undefined for its argument (sqrt of a
    /// negative number, log of zero) is NULL.
    /// </summary>
    internal class MathOperator : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            var name = GetAttributeValue("name");
            var numbers = Helper.GetNumbers(expressions, ctx, $"qti-math-operator {name}");
            if (numbers == null)
            {
                return null;
            }
            var expectedArguments = name == "atan2" ? 2 : 1;
            if (numbers.Count != expectedArguments)
            {
                ctx.LogError($"qti-math-operator {name} expects {expectedArguments} child expression(s), found: {numbers.Count}");
                return null;
            }
            var value = numbers[0];
            double result;
            switch (name)
            {
                case "sin": result = Math.Sin(value); break;
                case "cos": result = Math.Cos(value); break;
                case "tan": result = Math.Tan(value); break;
                case "secant": result = 1.0 / Math.Cos(value); break;
                case "cosecant": result = 1.0 / Math.Sin(value); break;
                case "cotangent": result = 1.0 / Math.Tan(value); break;
                case "asin": result = Math.Asin(value); break;
                case "acos": result = Math.Acos(value); break;
                case "atan": result = Math.Atan(value); break;
                case "atan2": result = Math.Atan2(value, numbers[1]); break;
                case "sinh": result = Math.Sinh(value); break;
                case "cosh": result = Math.Cosh(value); break;
                case "tanh": result = Math.Tanh(value); break;
                case "exp": result = Math.Exp(value); break;
                case "log": result = Math.Log10(value); break;
                case "ln": result = Math.Log(value); break;
                case "sqrt": result = Math.Sqrt(value); break;
                case "abs": result = Math.Abs(value); break;
                case "floor": result = Math.Floor(value); break;
                case "ceil": result = Math.Ceiling(value); break;
                case "signum": result = value == 0.0 ? 0.0 : (value < 0.0 ? -1.0 : 1.0); break;
                case "toDegrees": result = value * (180.0 / Math.PI); break;
                case "toRadians": result = value * (Math.PI / 180.0); break;
                default:
                    {
                        ctx.LogError($"Unsupported name in qti-math-operator: '{name}'");
                        return null;
                    }
            }
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                ctx.LogInformation($"qti-math-operator {name}({value}) has no finite result, returning null.");
                return null;
            }
            return result.ToBaseValue();
        }
    }
}

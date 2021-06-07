using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class Match : IConditionExpression
    {
        public string Name { get => "qti-match"; }

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            var values = qtiElement.Elements().Select(context.GetValue).ToList();
            if (values != null && values.Count() == 2)
            {
                var valueToMap = values[0];
                var correctValueInfo = values[1];
                
                if (valueToMap == null)
                {
                    context.LogError("No value for first child of match element");
                    return false;
                }
                if (correctValueInfo.Cardinality == null || correctValueInfo.Cardinality == Cardinality.Single)
                {
                    return Helper.CompareSingleValues(valueToMap.Value, correctValueInfo.Value, correctValueInfo.BaseType, context);
                }
                else
                {
                    // if length is not equal, don't check, it's incorrect.

                    var lengthEquals = valueToMap?.Values?.Count == correctValueInfo?.Values?.Count;
                    if (lengthEquals == false)
                    {
                        return false;
                    }
                    var answerIndex = 0;
                    var valuesToBeMapped = valueToMap.Values.Select(v => v).ToList();
                    foreach (var correctAnswer in correctValueInfo.Values)
                    {
                        if (correctValueInfo.Cardinality == Cardinality.Ordered)
                        {
                            var currentValueToMap = valuesToBeMapped[answerIndex];
                            var result = Helper.CompareSingleValues(correctAnswer, currentValueToMap, correctValueInfo.BaseType, context);
                            if (result == false)
                            {
                                return false;
                            }
                        } else
                        {
                            // sequence does not matter, find value somewhere in the array
                            // and remove when found.
                            string matchingValue = null;
                            foreach (var mv in valuesToBeMapped)
                            {
                                var result = Helper.CompareSingleValues(correctAnswer, mv, correctValueInfo.BaseType, context);
                                if (result)
                                {
                                    matchingValue = mv;
                                    break;
                                }
                            }
                            if (matchingValue != null)
                            {
                                valuesToBeMapped.Remove(matchingValue);
                            } else
                            {
                                return false;
                            }
      
                        }
                        answerIndex++;
                    }
                    return true; // if everything matches it's correct;
                }
            }
            else
            {
                context.LogError("Unexpected number of childElements. Match should have 2 childs.");
                return false;
            }
        }

    }
}
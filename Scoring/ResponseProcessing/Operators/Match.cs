using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.Operators
{
    internal class Match : IResponseProcessingOperator
    {
        public string Name { get => "match"; }

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            var values = qtiElement.Elements()?.ToArray();
            if (values != null && values.Count() == 2)
            {
                var valueToMap = values[0].GetVariableOrBaseValue(context);
                var correctValueInfo = values[1].GetCorrectValue(context);

                if (valueToMap == null)
                {
                    context.LogError("No value for first child of match element");
                    return false;
                }
                var correctValue = correctValueInfo.Value.BaseValue;
                if (correctValueInfo.Value.Cardinality == Cardinality.Single)
                {
                    return Helper.CompareSingleValues(valueToMap.Value, correctValue.Value, correctValue.BaseType, context);
                }
                else
                {
                    // if length is not equal, don't check, it's incorrect.

                    var lengthEquals = valueToMap?.Values?.Count == correctValue?.Values?.Count;
                    if (lengthEquals == false)
                    {
                        return false;
                    }
                    var answerIndex = 0;
                    var valuesToBeMapped = valueToMap.Values.Select(v => v).ToList();
                    foreach (var correctAnswer in correctValue.Values)
                    {
                        if (correctValueInfo.Value.Cardinality == Cardinality.Ordered)
                        {
                            var currentValueToMap = valuesToBeMapped[answerIndex];
                            var result = Helper.CompareSingleValues(correctAnswer, currentValueToMap, correctValue.BaseType, context);
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
                                var result = Helper.CompareSingleValues(correctAnswer, mv, correctValue.BaseType, context);
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
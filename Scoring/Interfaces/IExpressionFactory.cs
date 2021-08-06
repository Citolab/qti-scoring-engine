using Citolab.QTI.ScoringEngine.Interfaces;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    internal interface IExpressionFactory
    {
        IConditionExpression GetConditionExpression(XElement qtiElement, bool logErrorIfNotFound);
        IValueExpression GetValueExpression(XElement qtiElement, bool logErrorIfNotFound);
        ICustomOperator GetCustomOperator(string defintion);
    }
}
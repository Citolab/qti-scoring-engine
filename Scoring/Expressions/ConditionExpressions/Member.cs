
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class Member : ConditionExpressionBase
    {
        public override bool Execute(IProcessingContext ctx)
        {
            return Helper.ValueIsMemberOf(expressions, ctx, attributes);
        }

    }
}

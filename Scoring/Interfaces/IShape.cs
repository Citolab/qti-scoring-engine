using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    internal interface IShape
    {
        // used for scoring
        bool IsInside(string response);
        // used to test with the 
        PointF GetCenterPoint();
    }
}

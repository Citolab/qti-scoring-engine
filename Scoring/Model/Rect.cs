using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class Rect : IShape
    {
        private readonly IProcessingContext _logContext;
        private float _x0 = 0.0f;
        private float _y0 = 0.0f;
        private float _x1 = 0.0f;
        private float _y1 = 0.0f;
        public Rect(string coords, IProcessingContext logContext)
        {
            var splittedCoords = coords.Split(',');
            if (splittedCoords.Length == 4)
            {
                _x0 = coords[0];
                _y0 = coords[1];
                _x1 = coords[2];
                _y1 = coords[3];
            }
            else
            {
                logContext.LogError("Unexpect number of point in coords");
            }
            _logContext = logContext;
        }

        public PointF GetCenterPoint()
        {
            return new PointF { X = _x1, Y = _y1 };
        }

        public bool IsInside(string response)
        {
            var pointerInfo = Helper.GetPointsFromResponse(response, _logContext);
            if (pointerInfo.HasValue)
            {
                var pointer = pointerInfo.Value;
                return pointer.X > _x0 && pointer.X < _x1 && pointer.Y > _y0 && pointer.Y < _y1;
            }
            return false;
        }
    }
}

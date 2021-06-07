using Citolab.QTI.ScoringEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class Circle : IShape
    {
        private readonly IProcessingContext _logContext;
        private float _cx = 0.0f;
        private float _cy = 0.0f;
        private float _r = 0.0f;
        public Circle(string coords, IProcessingContext logContext)
        {
            var splittedCoords = coords.Split(',');
            if (splittedCoords.Length == 3)
            {
                _cx = coords[0];
                _cy = coords[1];
                _r = coords[2];
            }
            else
            {
                logContext.LogError("Unexpect number of point in coords");
            }

            _logContext = logContext;
        }

        public PointF GetCenterPoint() => new PointF { X = _cx, Y = _cy };

        public bool IsInside(string response)
        {
            var pointerInfo = Helpers.Helper.GetPointsFromResponse(response, _logContext);
            if (pointerInfo.HasValue)
            {
                var pointer = pointerInfo.Value;
                return ((_cx - pointer.X) * (_cx - pointer.X) + (_cy - pointer.Y) * (_cy - pointer.Y)) < _r * _r;
            }
            return false;
        }
    }
}

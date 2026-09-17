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
        private readonly bool _isValid;
        public Rect(string coords, IProcessingContext logContext)
        {
            _logContext = logContext;
            var splittedCoords = (coords ?? string.Empty).Split(',');
            if (splittedCoords.Length != 4)
            {
                logContext.LogError($"rect coords should contain 4 values: 'x1,y1,x2,y2', found: '{coords}'");
                return;
            }
            if (!splittedCoords[0].Trim().TryParseFloat(out var x1) ||
                !splittedCoords[1].Trim().TryParseFloat(out var y1) ||
                !splittedCoords[2].Trim().TryParseFloat(out var x2) ||
                !splittedCoords[3].Trim().TryParseFloat(out var y2))
            {
                logContext.LogError($"rect coords could not be parsed to floats: '{coords}'");
                return;
            }
            // normalize so the corners are ordered, whichever way round they were written.
            _x0 = Math.Min(x1, x2);
            _y0 = Math.Min(y1, y2);
            _x1 = Math.Max(x1, x2);
            _y1 = Math.Max(y1, y2);
            _isValid = true;
        }

        public PointF GetCenterPoint()
        {
            return new PointF { X = (_x0 + _x1) / 2, Y = (_y0 + _y1) / 2 };
        }

        public bool IsInside(string response)
        {
            if (!_isValid)
            {
                // an area we could not parse should never score.
                return false;
            }
            var pointerInfo = Helper.GetPointsFromResponse(response, _logContext);
            if (pointerInfo.HasValue)
            {
                var pointer = pointerInfo.Value;
                // a point on the edge counts as inside, like an HTML image map hotspot.
                return pointer.X >= _x0 && pointer.X <= _x1 && pointer.Y >= _y0 && pointer.Y <= _y1;
            }
            return false;
        }
    }
}

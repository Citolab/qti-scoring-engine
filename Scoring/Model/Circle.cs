using Citolab.QTI.ScoringEngine.Helpers;
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
        private readonly bool _isValid;
        public Circle(string coords, IProcessingContext logContext)
        {
            _logContext = logContext;
            var splittedCoords = (coords ?? string.Empty).Split(',');
            if (splittedCoords.Length != 3)
            {
                logContext.LogError($"circle coords should contain 3 values: 'cx,cy,r', found: '{coords}'");
                return;
            }
            if (!splittedCoords[0].Trim().TryParseFloat(out _cx) ||
                !splittedCoords[1].Trim().TryParseFloat(out _cy) ||
                !splittedCoords[2].Trim().TryParseFloat(out _r))
            {
                logContext.LogError($"circle coords could not be parsed to floats: '{coords}'");
                _cx = _cy = _r = 0.0f;
                return;
            }
            _isValid = true;
        }

        public PointF GetCenterPoint() => new PointF { X = _cx, Y = _cy };

        public bool IsInside(string response)
        {
            if (!_isValid)
            {
                // an area we could not parse should never score, rather than collapse to a
                // zero-radius circle at the origin that happens to match the response '0 0'.
                return false;
            }
            var pointerInfo = Helper.GetPointsFromResponse(response, _logContext);
            if (pointerInfo.HasValue)
            {
                var pointer = pointerInfo.Value;
                // a point on the edge counts as inside, like an HTML image map hotspot.
                return ((_cx - pointer.X) * (_cx - pointer.X) + (_cy - pointer.Y) * (_cy - pointer.Y)) <= _r * _r;
            }
            return false;
        }
    }
}

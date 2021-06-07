using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class Polygon : IShape
    {
        private readonly IProcessingContext _logContext;
        private readonly List<PointF> _allPoints = new List<PointF>();

        public Polygon(string coords, IProcessingContext logContext)
        {
            var splittedCoords = coords.Split(',');
            if (!IsEven(splittedCoords.Length))
            {
                logContext.LogError("Unexpect number of point in coords, its should contain a x and y and therefor should be an even number");
            }
            var index = 0;
            while (index < splittedCoords.Length)
            {
                var points = Helper.GetPointsFromResponse($"{splittedCoords[index]} {splittedCoords[index + 1]}", logContext);
                if (points.HasValue)
                {
                    _allPoints.Add(points.Value);
                }
                index += 2;
            }
            _logContext = logContext;
        }

        /// <summary>
        /// Method to compute the centroid of a polygon. This does NOT work for a complex polygon.
        /// </summary>
        /// <param name="poly">points that define the polygon</param>
        /// <returns>centroid point, or PointF.Empty if something wrong</returns>
        public PointF GetCenterPoint()
        {
            var poly = _allPoints.ToArray();
            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;

            for (int i = 0, j = poly.Count() - 1; i < poly.Count(); j = i++)
            {
                float temp = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
                accumulatedArea += temp;
                centerX += (poly[i].X + poly[j].X) * temp;
                centerY += (poly[i].Y + poly[j].Y) * temp;
            }

            if (Math.Abs(accumulatedArea) < 1E-7f)
                return PointF.Empty;  // Avoid division by zero

            accumulatedArea *= 3f;
            return new PointF(centerX / accumulatedArea, centerY / accumulatedArea);
        }

        public bool IsEven(int value)
        {
            return value % 2 == 0;
        }

        public bool IsInside(string response)
        {
            var pointInfo = Helper.GetPointsFromResponse(response, _logContext);
            if (pointInfo.HasValue)
            {
                return IsInPolygon(_allPoints, pointInfo.Value);
            }
            return false;
        }

        public bool IsInPolygon(List<PointF> poly, PointF point)
        {
            var coef = poly.Skip(1).Select((p, i) =>
                                            (point.Y - poly[i].Y) * (p.X - poly[i].X)
                                          - (point.X - poly[i].X) * (p.Y - poly[i].Y))
                                    .ToList();

            if (coef.Any(p => p == 0))
                return true;

            for (int i = 1; i < coef.Count(); i++)
            {
                if (coef[i] * coef[i - 1] < 0)
                    return false;
            }
            return true;
        }
    }
}

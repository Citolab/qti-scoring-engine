using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.Tests;
using Xunit;

namespace ScoringEngine.Tests.ModelTests
{
    /// <summary>
    /// These assert against hard-coded coordinates on purpose. Deriving the expected point from
    /// GetCenterPoint() makes the test agree with whatever the constructor parsed, which is how
    /// a bug where the coords string was indexed character-by-character stayed hidden.
    /// </summary>
    public class ShapeTests
    {
        // the circle from the IMS select_point example: centre (102,113), radius 16.
        private const string CircleCoords = "102,113,16";
        // the first associableHotspot from the IMS graphic_gap_match example.
        private const string RectCoords = "12,108,39,121";

        private static Citolab.QTI.ScoringEngine.Interfaces.IProcessingContext Context() =>
            TestHelper.GetDefaultResponseProcessingContext(null);

        [Theory]
        [InlineData("rect", Shape.Rect)]
        [InlineData("circle", Shape.Circle)]
        [InlineData("poly", Shape.Poly)]
        [InlineData("ellipse", Shape.Ellipse)]
        [InlineData("default", Shape.Default)]
        public void ShapeStringMapsToShape(string shapeString, Shape expected)
        {
            Assert.Equal(expected, shapeString.ToShape());
        }

        [Fact]
        public void CircleParsesItsCoords()
        {
            var circle = new Circle(CircleCoords, Context());
            var center = circle.GetCenterPoint();
            Assert.Equal(102f, center.X);
            Assert.Equal(113f, center.Y);
        }

        [Theory]
        [InlineData("102 113", true)]   // the centre
        [InlineData("110 113", true)]   // 8 right of centre, well within r=16
        [InlineData("118 113", true)]   // exactly on the edge counts as inside
        [InlineData("113 124", true)]   // diagonal, distance^2 = 242 < 256
        [InlineData("119 113", false)]  // 17 right of centre, just outside
        [InlineData("113 125", false)]  // diagonal, distance^2 = 265 > 256
        [InlineData("0 0", false)]
        public void CircleIsInside(string response, bool expected)
        {
            Assert.Equal(expected, new Circle(CircleCoords, Context()).IsInside(response));
        }

        [Fact]
        public void RectParsesItsCoordsAndCentresBetweenTheCorners()
        {
            var rect = new Rect(RectCoords, Context());
            var center = rect.GetCenterPoint();
            Assert.Equal(25.5f, center.X);
            Assert.Equal(114.5f, center.Y);
        }

        [Theory]
        [InlineData("25 114", true)]    // middle
        [InlineData("12 108", true)]    // top-left corner is inside
        [InlineData("39 121", true)]    // bottom-right corner is inside
        [InlineData("11 114", false)]   // just left of the rect
        [InlineData("40 114", false)]   // just right of the rect
        [InlineData("25 107", false)]   // just above
        [InlineData("25 122", false)]   // just below
        [InlineData("0 0", false)]
        public void RectIsInside(string response, bool expected)
        {
            Assert.Equal(expected, new Rect(RectCoords, Context()).IsInside(response));
        }

        [Fact]
        public void RectAcceptsCornersInEitherOrder()
        {
            var reversed = new Rect("39,121,12,108", Context());
            Assert.True(reversed.IsInside("25 114"));
            Assert.False(reversed.IsInside("40 114"));
        }

        [Theory]
        [InlineData("50 50", true)]
        [InlineData("5 5", true)]       // inside the lower-left corner of the square
        [InlineData("150 50", false)]
        public void PolygonIsInside(string response, bool expected)
        {
            // a 0,0 - 100,100 square, closed as QTI writes it.
            var polygon = new Polygon("0,0,100,0,100,100,0,100,0,0", Context());
            Assert.Equal(expected, polygon.IsInside(response));
        }

        [Fact]
        public void MalformedCoordsAreReportedAndDoNotMatch()
        {
            Assert.False(new Circle("102,113", Context()).IsInside("102 113"));
            Assert.False(new Rect("12,108,39", Context()).IsInside("25 114"));
            Assert.False(new Circle("a,b,c", Context()).IsInside("0 0"));
        }
    }
}

using Citolab.QTI.ScoringEngine.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Xunit;

namespace Citolab.QTI.ScoringEngine.Tests.ExpressionTests.BaseValueExpressions
{
    public class MathOperatorTests
    {
        private static BaseValue Apply(string qtiXml)
        {
            var context = TestHelper.GetDefaultResponseProcessingContext(
                TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration>()));
            var expression = TestHelper.GetExpressionFactory().GetValueExpression(XElement.Parse(qtiXml), true);
            return expression.Apply(context);
        }

        private static double Number(string qtiXml)
        {
            return double.Parse(Apply(qtiXml).Value, CultureInfo.InvariantCulture);
        }

        private static string Container(params string[] values)
        {
            var baseValues = string.Join("", System.Array.ConvertAll(values,
                value => $@"<qti-base-value base-type=""float"">{value}</qti-base-value>"));
            return $"<qti-multiple>{baseValues}</qti-multiple>";
        }

        [Fact]
        public void Product_Multiplies_Its_Children()
        {
            Assert.Equal(150, Number(@"<qti-product>
                    <qti-base-value base-type=""integer"">3</qti-base-value>
                    <qti-base-value base-type=""integer"">50</qti-base-value>
                </qti-product>"));
        }

        [Fact]
        public void Divide_Returns_A_Float()
        {
            Assert.Equal(5.0, Number(@"<qti-divide>
                    <qti-base-value base-type=""float"">10.0</qti-base-value>
                    <qti-base-value base-type=""float"">2.0</qti-base-value>
                </qti-divide>"));
        }

        [Fact]
        public void Divide_By_Zero_Is_Null()
        {
            Assert.Null(Apply(@"<qti-divide>
                    <qti-base-value base-type=""float"">10.0</qti-base-value>
                    <qti-base-value base-type=""float"">0</qti-base-value>
                </qti-divide>"));
        }

        [Fact]
        public void IntegerDivide_Rounds_Down()
        {
            Assert.Equal(2, Number(@"<qti-integer-divide>
                    <qti-base-value base-type=""integer"">7</qti-base-value>
                    <qti-base-value base-type=""integer"">3</qti-base-value>
                </qti-integer-divide>"));
        }

        [Fact]
        public void IntegerModulus_Returns_The_Remainder()
        {
            Assert.Equal(1, Number(@"<qti-integer-modulus>
                    <qti-base-value base-type=""integer"">7</qti-base-value>
                    <qti-base-value base-type=""integer"">3</qti-base-value>
                </qti-integer-modulus>"));
        }

        [Fact]
        public void Power_Raises_To_The_Power_Of_The_Second_Child()
        {
            Assert.Equal(8, Number(@"<qti-power>
                    <qti-base-value base-type=""float"">2</qti-base-value>
                    <qti-base-value base-type=""float"">3</qti-base-value>
                </qti-power>"));
        }

        [Fact]
        public void RoundTo_Rounds_To_Decimal_Places()
        {
            Assert.Equal(3.14, Number(@"<qti-round-to figures=""2"" rounding-mode=""decimalPlaces"">
                    <qti-base-value base-type=""float"">3.14159</qti-base-value>
                </qti-round-to>"));
        }

        [Fact]
        public void RoundTo_Rounds_To_Significant_Figures()
        {
            Assert.Equal(3.14, Number(@"<qti-round-to figures=""3"" rounding-mode=""significantFigures"">
                    <qti-base-value base-type=""float"">3.14159</qti-base-value>
                </qti-round-to>"));
        }

        [Fact]
        public void Truncate_Cuts_Towards_Zero()
        {
            Assert.Equal(6, Number(@"<qti-truncate><qti-base-value base-type=""float"">6.8</qti-base-value></qti-truncate>"));
            Assert.Equal(-6, Number(@"<qti-truncate><qti-base-value base-type=""float"">-6.8</qti-base-value></qti-truncate>"));
        }

        [Fact]
        public void IntegerToFloat_Changes_The_BaseType()
        {
            var result = Apply(@"<qti-integer-to-float><qti-base-value base-type=""integer"">3</qti-base-value></qti-integer-to-float>");

            Assert.Equal(BaseType.Float, result.BaseType);
            Assert.Equal(3.0, double.Parse(result.Value, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void MathOperator_Calculates_The_Named_Function()
        {
            Assert.Equal(4, Number(@"<qti-math-operator name=""sqrt""><qti-base-value base-type=""float"">16</qti-base-value></qti-math-operator>"));
            Assert.Equal(2, Number(@"<qti-math-operator name=""abs""><qti-base-value base-type=""float"">-2</qti-base-value></qti-math-operator>"));
            Assert.Equal(3, Number(@"<qti-math-operator name=""log""><qti-base-value base-type=""float"">1000</qti-base-value></qti-math-operator>"));
            Assert.Equal(1, Number(@"<qti-math-operator name=""ln""><qti-base-value base-type=""float"">2.718281828459045</qti-base-value></qti-math-operator>"));
        }

        [Fact]
        public void MathOperator_Without_A_Finite_Result_Is_Null()
        {
            Assert.Null(Apply(@"<qti-math-operator name=""sqrt""><qti-base-value base-type=""float"">-1</qti-base-value></qti-math-operator>"));
        }

        [Fact]
        public void MathConstant_Returns_Pi_And_E()
        {
            Assert.Equal(System.Math.PI, Number(@"<qti-math-constant name=""pi""/>"));
            Assert.Equal(System.Math.E, Number(@"<qti-math-constant name=""e""/>"));
        }

        [Fact]
        public void StatsOperator_Calculates_The_Mean()
        {
            Assert.Equal(5, Number($"<qti-stats-operator name=\"mean\">{Container("2", "4", "9")}</qti-stats-operator>"));
        }

        [Fact]
        public void StatsOperator_Calculates_The_Median()
        {
            Assert.Equal(3.5, Number($"<qti-stats-operator name=\"median\">{Container("1", "3", "4", "7")}</qti-stats-operator>"));
        }

        [Fact]
        public void StatsOperator_Calculates_The_Population_Standard_Deviation()
        {
            Assert.Equal(2, Number($"<qti-stats-operator name=\"popSD\">{Container("2", "4", "4", "4", "5", "5", "7", "9")}</qti-stats-operator>"));
        }

        [Fact]
        public void StatsOperator_Calculates_The_Sample_Standard_Deviation()
        {
            // mean 4.5, sum of squares 13.5, divided by n - 1 is 2.7, the square root of which is 1.643
            Assert.Equal(1.643, Number($"<qti-stats-operator name=\"sampleSD\">{Container("2", "4", "4", "5", "5", "7")}</qti-stats-operator>"), 3);
        }

        [Fact]
        public void Gcd_And_Lcm()
        {
            Assert.Equal(6, Number(@"<qti-gcd>
                    <qti-base-value base-type=""integer"">12</qti-base-value>
                    <qti-base-value base-type=""integer"">18</qti-base-value>
                    <qti-base-value base-type=""integer"">24</qti-base-value>
                </qti-gcd>"));
            Assert.Equal(12, Number(@"<qti-lcm>
                    <qti-base-value base-type=""integer"">4</qti-base-value>
                    <qti-base-value base-type=""integer"">6</qti-base-value>
                </qti-lcm>"));
        }

        [Fact]
        public void A_Child_That_Is_Not_A_Number_Makes_The_Operator_Null()
        {
            Assert.Null(Apply(@"<qti-product>
                    <qti-base-value base-type=""integer"">3</qti-base-value>
                    <qti-null/>
                </qti-product>"));
        }
    }
}

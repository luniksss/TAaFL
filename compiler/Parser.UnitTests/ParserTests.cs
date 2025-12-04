using Parser;
using Xunit;

namespace Parser.UnitTests;

public class ParserTests
{
    [Theory]
    [InlineData("1", 1)]
    [InlineData("42", 42)]
    public void Parse_Numbers(string expr, double expected)
    {
        Assert.Equal(expected, Parser.ExecuteExpr(expr));
    }


    [Fact]
    public void Parse_ParenthesesPriority()
    {
        Assert.Equal(2.5, Parser.ExecuteExpr("(5 + 5) / 4"));
    }


    [Fact]
    public void Parse_UnaryMinusVsPower()
    {
        Assert.Equal(1024, Parser.ExecuteExpr("(-2) ^ 10"));
        Assert.Equal(-1024, Parser.ExecuteExpr("-(2 ^ 10)"));
    }


    [Fact]
    public void Parse_Constants()
    {
        Assert.Equal(3.141592653589793 * 3, Parser.ExecuteExpr("пи * 3"), 10);
        Assert.Equal(2.718281828459045 * 2, Parser.ExecuteExpr("эклер * 2"), 10);
    }


    [Fact]
    public void Parse_BooleanValues()
    {
        Assert.Equal(1, Parser.ExecuteExpr("кринж"));
        Assert.Equal(0, Parser.ExecuteExpr("некринж"));
    }


    [Fact]
    public void Parse_LogicalOr()
    {
        Assert.Equal(1, Parser.ExecuteExpr("0 || 5"));
        Assert.Equal(0, Parser.ExecuteExpr("0 || 0"));
    }


    [Fact]
    public void Parse_LogicalAnd()
    {
        Assert.Equal(1, Parser.ExecuteExpr("3 && 2"));
        Assert.Equal(0, Parser.ExecuteExpr("3 && 0"));
    }


    [Fact]
    public void Parse_Comparisons()
    {
        Assert.Equal(1, Parser.ExecuteExpr("5 > 2"));
        Assert.Equal(0, Parser.ExecuteExpr("2 >= 3"));
        Assert.Equal(1, Parser.ExecuteExpr("4 == 4"));
        Assert.Equal(1, Parser.ExecuteExpr("7 != 3"));
    }


    [Fact]
    public void Parse_BoolAsNumbers()
    {
        Assert.Equal(1, Parser.ExecuteExpr("кринж == 1"));
        Assert.Equal(1, Parser.ExecuteExpr("некринж == 0"));
    }


    [Fact]
    public void Parse_ComplexPrecedence()
    {
        Assert.Equal(2 + 3 * Math.Pow(4, 2), Parser.ExecuteExpr("2 + 3 * 4 ^ 2"));
    }


    [Fact]
    public void Parse_AssociativityMultDiv()
    {
        Assert.Equal(10 / 2 * 5 + 3, Parser.ExecuteExpr("10 / 2 * 5 + 3"));
    }


    [Fact]
    public void Parse_IntFloatComparison()
    {
        Assert.Equal(1, Parser.ExecuteExpr("6 == 6.0"));
        Assert.Equal(0, Parser.ExecuteExpr("6 == 6.2"));
    }
}

namespace Parser.UnitTests;

public class ParserTests
{
    [Theory]
    [MemberData(nameof(GetExprTests))]
    public void ParseTest(string expr, double expected, int precision)
    {
        double actual = Parser.ExecuteExpr(expr);
        Assert.Equal(expected, actual, precision);
    }

    [Theory]
    [MemberData(nameof(GetProgramTests))]
    public void ParseProgramTest(string program)
    {
        Parser.ParseProgram(program);
    }

    [Theory]
    [MemberData(nameof(GetErrorTests))]
    public void ParseErrorTest(string program)
    {
        Assert.ThrowsAny<Exception>(() => Parser.ParseProgram(program));
    }

    public static TheoryData<string, double, int> GetExprTests()
    {
        return new TheoryData<string, double, int>
        {
            { "1", 1, 15 },
            { "42", 42, 15 },
            { "(5 + 5) / 4", 2.5, 15 },
            { "(-2) ^ 10", 1024, 15 },
            { "-(2 ^ 10)", -1024, 15 },
            { "-2 ^ 10", -1024, 15 },
            { "пи * 3", 3.141592653589793 * 3, 10 },
            { "эклер * 2", 2.718281828459045 * 2, 10 },
            { "кринж", 1, 15 },
            { "некринж", 0, 15 },
            { "0 || 5", 1, 15 },
            { "0 || 0", 0, 15 },
            { "3 && 2", 1, 15 },
            { "3 && 0", 0, 15 },
            { "5 > 2", 1, 15 },
            { "2 >= 3", 0, 15 },
            { "4 == 4", 1, 15 },
            { "7 != 3", 1, 15 },
            { "кринж == 1", 1, 15 },
            { "некринж == 0", 1, 15 },
            { "!кринж == некринж", 1, 15 },
            { "2 + 3 * 4 ^ 2", 2 + 3 * Math.Pow(4, 2), 15 },
            { "10 / 2 * 5 + 3", 10.0 / 2 * 5 + 3, 15 },
            { "6 == 6.0", 1, 15 },
            { "6 == 6.2", 0, 15 },
        };
    }

    public static TheoryData<string> GetProgramTests()
    {
        return new TheoryData<string>
        {
            @"лапкапомощи() { мур цыфорка x царапнуть 1 мяу }",
            @"лапкапомощи() { мур цыфорка x мяу }",
            @"лапкапомощи() { строгиймур цыфорка y царапнуть 10 мяу }",
            @"лапкапомощи() { мур цыфорка x мяу x царапнуть 5 мяу }",
            @"лапкапомощи() { мур цыфорка x царапнуть 1 мяу строгиймур цыфорка y царапнуть 10 мяу мур цыфорка t мяу мурлыкать(x + y) мяу }",
            @"лапкапомощи() { мур цыфорка x мяу клацать(x) мяу }",
            @"лапкапомощи() { мур цыфорка x царапнуть 5 мяу триппитроппа (x > 0) { мурлыкать(x) мяу } мяу }",
            @"лапкапомощи() { мур цыфорка x царапнуть 5 мяу триппитроппа (x > 0) { мурлыкать(x) мяу } троппатриппа { мурлыкать(0) мяу } мяу }",
            @"лапкапомощи() { мурлыкать(42) мяу }",
        };
    }

    public static TheoryData<string> GetErrorTests()
    {
        return new TheoryData<string>
        {
            @"лапкапомощи() { мур цыфорка value царапнуть 0 }",
            @"лапкапомощи() { строгиймур цыфорка x царапнуть 5 мяу x царапнуть 10 мяу }",
            @"лапкапомощи() { x царапнуть 5 мяу }",
            @"лапкапомощи() { мур цыфорка x царапнуть 1 мяу мур цыфорка x царапнуть 2 мяу }",
            @"лапка x() { вернуть 0 мяу } лапкапомощи() { мур цыфорка x царапнуть 1 мяу }",
            @"лапкапомощи() { строгиймур цыфорка x царапнуть 1 мяу мур цыфорка x царапнуть 2 мяу }",
        };
    }
}

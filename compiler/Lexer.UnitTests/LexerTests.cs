using ExampleLib.UnitTests.Helpers;

using Xunit.Abstractions;

namespace Lexer.UnitTests;

public class LexerTests
{
    private readonly ITestOutputHelper _output;

    public LexerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [MemberData(nameof(GetTokenizeNumericLiteralData))]
    [MemberData(nameof(GetTokenizeIdentificatorData))]
    [MemberData(nameof(GetTokenizeExpressionLiteralData))]
    [MemberData(nameof(GetTokenizeCommentLiteralData))]
    [MemberData(nameof(GetTokenizeStrLiteralData))]
    public void CanCorrectTokenizeLexemes(string text, List<Token> expected)
    {

        List<Token> actual = Tokenize(text, _output);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetLexicalStatsData))]
    public void LexicalStatsTestTheory(string path, string expected)
    {
        using TempFile file = TempFile.Create(path);
        string actual = LexicalStats.CollectFromFile(file.Path);
        Assert.Equal(Normalize(expected), Normalize(actual));
    }

    public static TheoryData<string, List<Token>> GetTokenizeNumericLiteralData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "215", [
                    new Token(TokenType.NumericLiteral, new TokenValue(215.0))
                ]
            },
            {
                "0", [
                    new Token(TokenType.NumericLiteral, new TokenValue(0)) 
                    ]
            },
            {
                "3.14", [
                    new Token(TokenType.NumericLiteral, new TokenValue(3.14)) 
                    ]
            },
            {
                "0.0", [
                    new Token(TokenType.NumericLiteral, new TokenValue(0.0)) 
                    ]
            },
            {
                "0512", [
                    new Token(TokenType.NumericLiteral, new TokenValue(512)) 
                    ]
            },
            {
                "-0.314 -2712", [
                new Token(TokenType.NumericLiteral, new TokenValue(-0.314)),
                new Token(TokenType.NumericLiteral, new TokenValue(-2712))
            ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeIdentificatorData()
    {
        return new TheoryData<string, List<Token>>
        {
             {
               "number", [
                    new Token(TokenType.Identifier, new TokenValue("number"))
                    ]
             },
             {
                "переменная1", [
                    new Token(TokenType.Identifier, new TokenValue("переменная1"))
                    ]
             },
             {
                "_number12", [
                    new Token(TokenType.Identifier, new TokenValue("_number12"))
                    ]
             },
             {
                "МяУ строгийМУР царапнуть", [
                    new Token(TokenType.Semicolon, null),
                    new Token(TokenType.Const, null),
                    new Token(TokenType.Assign, null)
                    ]
             },
             {
                "15abc", [
                    new Token(TokenType.Error, new TokenValue("15abc"))
                ]
             },
             {
                "int_15number", [
                    new Token(TokenType.Identifier, new TokenValue("int_15number"))
                ]
             },
             {
                "IS_IT_NUMber is_it_number", [
                    new Token(TokenType.Identifier, new TokenValue("is_it_number")),
                    new Token(TokenType.Identifier, new TokenValue("is_it_number"))
                ]
             },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeExpressionLiteralData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "1 + 5 - 3", [
                    new Token(TokenType.NumericLiteral, new TokenValue(1)),
                    new Token(TokenType.PlusSign, null),
                    new Token(TokenType.NumericLiteral, new TokenValue(5)),
                    new Token(TokenType.MinusSign, null),
                    new Token(TokenType.NumericLiteral, new TokenValue(3)),
                ]
            },
            {
                "a * b", [
                    new Token(TokenType.Identifier, new TokenValue("a")),
                    new Token(TokenType.MultiplySign, null),
                    new Token(TokenType.Identifier, new TokenValue("b")),
                    ]
            },
            {
                "a ** b", [
                    new Token(TokenType.Identifier, new TokenValue("a")),
                    new Token(TokenType.ExponentSign, null),
                    new Token(TokenType.Identifier, new TokenValue("b")),
                    ]
            },
            {
                "10 < 5", [
                    new Token(TokenType.NumericLiteral, new TokenValue(10)),
                    new Token(TokenType.LessThan, null),
                    new Token(TokenType.NumericLiteral, new TokenValue(5)),
                    ]
            },
            {
                "abc >= 10", [
                    new Token(TokenType.Identifier, new TokenValue("abc")),
                    new Token(TokenType.NonStrictMore, null),
                    new Token(TokenType.NumericLiteral, new TokenValue(10)),
                    ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeCommentLiteralData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "окак Удалить после рефакторинга!", [
                    new Token(TokenType.Comment, null),
                ]
            },
            {
                "мур цыфорка number1 15 окак Буферная переменная", [
                    new Token(TokenType.Let, null),
                    new Token(TokenType.Int, null),
                    new Token(TokenType.Identifier, new TokenValue("number1")),
                    new Token(TokenType.NumericLiteral, new TokenValue(15)),
                    new Token(TokenType.Comment, null),
                    ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeStrLiteralData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "\" text text 123 aaa__\"", [
                    new Token(TokenType.StringLiteral, new TokenValue(" text text 123 aaa__")),
                ]
            },
            {
                "\"text \\n \\\\ \\\"\"", [
                    new Token(TokenType.StringLiteral, new TokenValue("text \n \\ \"")),
                    ]
            },
        };
    }

    public static TheoryData<string, string> GetLexicalStatsData()
    {
        return new TheoryData<string, string>
    {
      {
        @"лапкапомощи()
        {
        мур рилцыфорка радиус мяу
        клацать(радиус) мяу

        вывестиПлощадьКруга(радиус) мяу
        }

        лапка вывестиПлощадьКруга(рилцыферка радиус)
        {
        мур площадь царапнуть радиус * пи мяу
        мурлыкать(площадь) мяу
        }
        ",
        @"keywords: 7
        identifier: 11
        number literals: 0
        string literals: 0
        operators: 21
        other lexemes: 0
        "
      },
      {
        @"
        мур строгиймур мурлыкать клацать лапка лапкапомощи вернуть
        нитка  цыфорка рилцыфорка кринжли кринж некринж триппитроппа
        идентификатор супер_идентификатор englishIdentificator
        identificatorWithNumbers12_15 радиус площадь
        0.15 215 3.14 -777 -5.55
        ""строковый литерал!"" ""строковый \n литерал!""
        + - >= > < <= ==

        ",
        @"keywords: 14
        identifier: 6
        number literals: 5
        string literals: 2
        operators: 7
        other lexemes: 0
        "
      },
    };
    }

    private static string Normalize(string s) => string.Concat(s.Where(c => !char.IsWhiteSpace(c)));

    private static List<Token> Tokenize(string text, ITestOutputHelper? output = null)
    {
        List<Token> results = [];
        Lexer lexer = new(text);

        for (Token t = lexer.ParseToken(); t.Type != TokenType.EndOfFile; t = lexer.ParseToken())
        {
            results.Add(t);
            output?.WriteLine($"→ {t.Type} {(t.Value != null ? t.Value.ToString() : "")}");
        }

        return results;
    }
}

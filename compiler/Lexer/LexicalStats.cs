using System.Text;

namespace Lexer;

public static class LexicalStats
{
    private static readonly HashSet<TokenType> Keywords =
  [
    TokenType.Const, TokenType.Let, TokenType.String, TokenType.Int, TokenType.Bool, TokenType.Def,
    TokenType.True, TokenType.False, TokenType.If, TokenType.Else, TokenType.While, TokenType.Return,
    TokenType.Input, TokenType.Output, TokenType.Main, TokenType.Double,
  ];

    private static readonly HashSet<TokenType> Operators =
  [
    TokenType.EqualSign, TokenType.UnequalSign, TokenType.LessThan, TokenType.MoreThan, TokenType.NonStrictLess, TokenType.NonStrictMore,
    TokenType.PlusSign, TokenType.MinusSign, TokenType.MultiplySign, TokenType.DivisionSign,
    TokenType.ModuloSign, TokenType.ExponentSign, TokenType.Assign, TokenType.Semicolon,
    TokenType.OpenBrace, TokenType.CloseBrace, TokenType.OpenParenthesis, TokenType.CloseParenthesis,
  ];

    public static string CollectFromFile(string path)
    {
        string text = File.ReadAllText(path, Encoding.UTF8);
        Lexer lexer = new(text);
        LexicalReport lexicalReport = new();

        for (Token t = lexer.ParseToken(); t.Type != TokenType.EndOfFile; t = lexer.ParseToken())
        {
            if (t.Type == TokenType.Identifier)
            {
                lexicalReport.Identifiers++;
            }
            else if (t.Type == TokenType.NumericLiteral)
            {
                lexicalReport.NumberLiterals++;
            }
            else if (t.Type == TokenType.StringLiteral)
            {
                lexicalReport.StringLiterals++;
            }
            else if (Keywords.Contains(t.Type))
            {
                lexicalReport.Keywords++;
            }
            else if (Operators.Contains(t.Type))
            {
                lexicalReport.Operators++;
            }
            else
            {
                lexicalReport.OtherLexemes++;
            }
        }

        return lexicalReport.ToString();
    }
}

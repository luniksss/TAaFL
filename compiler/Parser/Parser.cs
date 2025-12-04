using Lexer;

namespace Parser;

/// <summary>
/// Выполняет синтаксический разбор.
/// Грамматика языка описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser
{
    private readonly TokenStream tokens;

    private Parser(string code)
    {
        tokens = new TokenStream(code);
    }

    public static double ExecuteExpr(string expr)
    {
        Parser p = new(expr);
        return p.ParseExpr();
    }

    /// <summary>
    /// Выполняет парсинг одного выражения.
    /// expression = logicalOr.
    /// </summary>
    private double ParseExpr()
    {
        return ParseOrExpr();
    }

    /// <summary>
    /// Выполняет парсинг выражения или.
    /// logicalOr = logicalAnd, { "||", logicalAnd }.
    /// </summary>
    private double ParseOrExpr()
    {
        double value = ParseAndExpr();
        while (tokens.Peek().Type == TokenType.LogicalOr)
        {
            tokens.Advance();
            double right = ParseAndExpr();
            value = (value != 0 || right != 0) ? 1 : 0;
        }

        return value;
    }

    /// <summary>
    /// Выполняет парсинг выражения и.
    /// logicalAnd = comparison, { "&&", comparison }.
    /// </summary>
    private double ParseAndExpr()
    {
        double value = ParseComparisonExpr();
        while (tokens.Peek().Type == TokenType.LogicalAnd)
        {
            tokens.Advance();
            double right = ParseComparisonExpr();
            value = (value != 0 && right != 0) ? 1 : 0;
        }

        return value;
    }

    /// <summary>
    /// Выполняет парсинг сравнения выражений.
    /// comparison = additive, [ ( ">" | ">=" | "<" | "<=" | "==" | "!=" ), additive ].
    /// </summary>
    private double ParseComparisonExpr()
    {
        double value = ParseAdditiveExpr();
        while (true)
        {
            switch (tokens.Peek().Type)
            {
                case TokenType.LessThan:
                    tokens.Advance();
                    value = (value < ParseAdditiveExpr()) ? 1 : 0;
                    break;
                case TokenType.MoreThan:
                    tokens.Advance();
                    value = (value > ParseAdditiveExpr()) ? 1 : 0;
                    break;
                case TokenType.NonStrictLess:
                    tokens.Advance();
                    value = (value <= ParseAdditiveExpr()) ? 1 : 0;
                    break;
                case TokenType.NonStrictMore:
                    tokens.Advance();
                    value = (value >= ParseAdditiveExpr()) ? 1 : 0;
                    break;
                case TokenType.EqualSign:
                    tokens.Advance();
                    value = (value == ParseAdditiveExpr()) ? 1 : 0;
                    break;
                case TokenType.UnequalSign:
                    tokens.Advance();
                    value = (value != ParseAdditiveExpr()) ? 1 : 0;
                    break;
                default:
                    return value;
            }
        }
    }

    /// <summary>
    /// Выполняет парсинг сложения/вычитания.
    /// additive = multiplicative, { ( "+" | "-" ), multiplicative }.
    /// </summary>
    private double ParseAdditiveExpr()
    {
        double value = ParseMultiplicativeExpr();

        while (true)
        {
            switch (tokens.Peek().Type)
            {
                case TokenType.PlusSign:
                    tokens.Advance();
                    value += ParseMultiplicativeExpr();
                    break;
                case TokenType.MinusSign:
                    tokens.Advance();
                    value -= ParseMultiplicativeExpr();
                    break;
                default:
                    return value;
            }
        }
    }

    /// <summary>
    /// Разбирает один операнд
    /// multiplicative = unary, { ( "*" | "/" | "%" ), unary }.
    /// </summary>
    private double ParseMultiplicativeExpr()
    {
        double value = ParseUnaryExpr();
        while (true)
        {
            switch (tokens.Peek().Type)
            {
                case TokenType.MultiplySign:
                    tokens.Advance();
                    value *= ParseUnaryExpr();
                    break;
                case TokenType.DivisionSign:
                    tokens.Advance();
                    {
                        double divisor = ParseUnaryExpr();
                        if (divisor == 0)
                        {
                            throw new DivideByZeroException();
                        }

                        value /= divisor;
                    }

                    break;
                case TokenType.ModuloSign:
                    tokens.Advance();
                    {
                        double divisor = ParseUnaryExpr();
                        if (divisor == 0)
                        {
                            throw new DivideByZeroException();
                        }

                        value %= divisor;
                    }

                    break;
                default:
                    return value;
            }
        }
    }

    /// <summary>
    /// Разбирает один префиксный операнд
    /// unary = [ "+" | "-" ], power.
    /// </summary>
    private double ParseUnaryExpr()
    {
        switch (tokens.Peek().Type)
        {
            case TokenType.PlusSign:
                tokens.Advance();
                return +ParseUnaryExpr();
            case TokenType.MinusSign:
                tokens.Advance();
                return -ParseUnaryExpr();
            default:
                return ParsePowerExpr();
        }
    }

    /// <summary>
    /// Разбирает одну операцию возведения в степень.
    /// power = primary, [ "^", power ].
    /// </summary>
    private double ParsePowerExpr()
    {
        double value = ParsePrimaryExpr();

        if (tokens.Peek().Type == TokenType.ExponentSign)
        {
            tokens.Advance();
            double right = ParsePowerExpr();
            value = Math.Pow(value, right);
        }

        return value;
    }

    /// <summary>
    /// парсинг основного выражения
    /// primary = literal | constant | boolean | identifier | "(", expression, ")" | functionCall.
    /// </summary>
    private double ParsePrimaryExpr()
    {
        Token t = tokens.Peek();
        switch (t.Type)
        {
            case TokenType.Identifier:
                throw new UnexpectedLexemeException(TokenType.Identifier, t);
            case TokenType.NumericLiteral:
            case TokenType.StringLiteral:
                return ParseLiteral();
            case TokenType.True:
            case TokenType.False:
                return ParseBool();
            case TokenType.Pi:
            case TokenType.Euler:
                return ParseConstant();
            case TokenType.OpenParenthesis:
                tokens.Advance();
                double value = ParseOrExpr();
                Match(TokenType.CloseParenthesis);
                return value;
            default:
                throw new UnexpectedLexemeException(t.Type, t);
        }
    }

    /// <summary>
    ///  Парсинг констант
    ///  constant = "пи" | "эклер".
    /// </summary>
    private double ParseConstant()
    {
        Token t = tokens.Peek();
        tokens.Advance();

        return t.Type switch
        {
            TokenType.Euler => 2.718281828459045,
            TokenType.Pi => 3.141592653589793,
            _ => throw new UnexpectedLexemeException(t.Type, t),
        };
    }

    /// <summary>
    /// Парсинг литерала
    /// literal = number | string.
    /// </summary>
    private double ParseLiteral()
    {
        Token t = tokens.Peek();
        switch (t.Type)
        {
            case TokenType.NumericLiteral:
                double value = t.Value!.ToDouble();
                tokens.Advance();
                return value;

            // case TokenType.StringLiteral:
            //   break;
            default:
                throw new UnexpectedLexemeException(t.Type, t);
        }
    }

    /// <summary>
    /// Парсинг логического значения
    /// boolean = "true" | "false" ;.
    /// </summary>
    private double ParseBool()
    {
        Token t = tokens.Peek();
        tokens.Advance();
        return t.Type switch
        {
            TokenType.True => 1,
            TokenType.False => 0,
            _ => throw new UnexpectedLexemeException(t.Type, t),
        };
    }

    private void Match(TokenType expected)
    {
        if (tokens.Peek().Type != expected)
        {
            throw new UnexpectedLexemeException(expected, tokens.Peek());
        }

        tokens.Advance();
    }
}
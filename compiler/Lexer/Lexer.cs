using System.Globalization;
using System.Text;

namespace Lexer;

public class Lexer
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        {
            "мур", TokenType.Let
        },
        {
            "строгиймур", TokenType.Const
        },
        {
            "лапкапомощи", TokenType.Main
        },
        {
            "нитка", TokenType.String
        },
        {
            "цыфорка", TokenType.Int
        },
        {
            "рилцыфорка", TokenType.Double
        },
        {
            "кринжли", TokenType.Bool
        },
        {
            "кринж", TokenType.True
        },
        {
            "некринж", TokenType.False
        },
        {
            "лапка", TokenType.Def
        },
        {
            "вернуть", TokenType.Return
        },
        {
            "триппитроппа", TokenType.If
        },
        {
            "магасияй", TokenType.While
        },
        {
            "царапнуть", TokenType.Assign
        },
        {
            "мяу", TokenType.Semicolon
        },
        {
            "мурлыкать", TokenType.Output
        },
        {
            "клацать", TokenType.Input
        },
        {
            "пи", TokenType.Pi
        },
        {
            "эклер", TokenType.Euler
        },
        {
            "ничего", TokenType.Void
        },
        {
            "хотябыраз", TokenType.DoWhile
        },
        {
            "хвостомкрутить", TokenType.For
        },
        {
            "стоп", TokenType.Break
        },
        {
            "троппатриппа", TokenType.Else
        },
    };

    private readonly TextScanner _scanner;
    private bool _isComment;

    public Lexer(string input)
    {
        _scanner = new TextScanner(input);
    }

    /// <summary>
    /// Обрабатывает строку и возвращает первый её токен.
    /// </summary>
    public Token ParseToken()
    {
        _isComment = false;
        SkipWhiteSpaces();

        if (_scanner.IsEnd())
        {
            return new Token(TokenType.EndOfFile, null);
        }

        char ch = _scanner.Peek();
        if (char.IsLetter(ch) || ch == '_' || (ch >= 'А' && ch <= 'я') || ch == 'Ё' || ch == 'ё')
        {
            return ParseIdentifierOrKeyword();
        }

        if (char.IsAsciiDigit(ch))
        {
            return ParseNumericLiteral();
        }

        return ParseSingleTokens();
    }

    /// <summary>
    /// Пропускает пробельный символ ' ', пока не встретит иной.
    /// </summary>
    private void SkipWhiteSpaces()
    {
        while (char.IsWhiteSpace(_scanner.Peek()))
        {
            _scanner.Advance();
        }
    }

    /// <summary>
    /// Пропускает однострочный комментарий до конца строки (включая символ
    /// перевода строки).
    /// </summary>
    private Token SkipComments()
    {
        while (_scanner.Peek() != '\n' && _scanner.Peek() != '\r' && !_scanner.IsEnd())
        {
            _scanner.Advance();
        }

        if (!_scanner.IsEnd())
        {
            _scanner.Advance();
        }

        return new Token(TokenType.Comment, null);
    }

    /// <summary>
    /// Проверяет идентификатор или ключевое слово (регистронезависимо).
    /// Если ключевое слово обозночает однострочный комментарий - пропускает всю строку,
    /// возвращая null.
    /// Идентификатором считается любой набор букв, цифр и символа "_",
    /// начинающийся с символа "_" или буквы.
    /// </summary>
    private Token ParseIdentifierOrKeyword()
    {
        string identifier = "";
        char ch = _scanner.Peek();
        while (char.IsLetter(ch) || char.IsAsciiDigit(ch) || ch == '_' || (ch >= 'А' && ch <= 'я') || ch == 'Ё' || ch == 'ё')
        {
            identifier += ch;
            _scanner.Advance();
            ch = _scanner.Peek();
        }

        string lowerIdentifier = identifier.ToLowerInvariant();
        if (lowerIdentifier == "окак")
        {
            _isComment = true;
            return SkipComments();
        }

        if (Keywords.TryGetValue(lowerIdentifier, out TokenType type))
        {
            return new Token(type, null);
        }

        return new Token(TokenType.Identifier, new TokenValue(lowerIdentifier));
    }

    /// <summary>
    /// Проверяет литерал числа по правилам:
    ///     number = digits_sequence, [ ".", digits_sequence ] ;
    ///     digits_sequence = digit { digit } ;
    ///     digit = "0" | "1" | ... | "9" ;
    /// Все числа представляются как 64-битные числа с плавающей точкой (double).
    /// Унарный минус обрабатывается парсером как отдельный токен.
    /// </summary>
    private Token ParseNumericLiteral()
    {
        StringBuilder sb = new StringBuilder();

        // integer part
        while (char.IsAsciiDigit(_scanner.Peek()))
        {
            sb.Append(_scanner.Peek());
            _scanner.Advance();
        }

        // fractional part
        if (_scanner.Peek() == '.')
        {
            sb.Append('.');
            _scanner.Advance();

            if (!char.IsAsciiDigit(_scanner.Peek()))
            {
                return new Token(TokenType.Error, new TokenValue(sb.ToString()));
            }

            while (char.IsAsciiDigit(_scanner.Peek()))
            {
                sb.Append(_scanner.Peek());
                _scanner.Advance();
            }
        }

        if (char.IsLetter(_scanner.Peek()))
        {
            while (!char.IsWhiteSpace(_scanner.Peek()) && !_scanner.IsEnd())
            {
                sb.Append(_scanner.Peek());
                _scanner.Advance();
            }

            return new Token(TokenType.Error, new TokenValue(sb.ToString()));
        }

        // parse
        if (double.TryParse(sb.ToString(), CultureInfo.InvariantCulture, out double result))
        {
            return new Token(TokenType.NumericLiteral, new TokenValue(result));
        }

        return new Token(TokenType.Error, new TokenValue(sb.ToString()));
    }

    /// <summary>
    /// Обрабатывает все одиночные литералы.
    /// </summary>
    private Token ParseSingleTokens()
    {
        char ch = _scanner.Peek();
        switch (ch)
        {
            case '"':
                return ParseString();
            case '{':
                _scanner.Advance();
                return new Token(TokenType.OpenBrace, null);
            case '}':
                _scanner.Advance();
                return new Token(TokenType.CloseBrace, null);
            case '(':
                _scanner.Advance();
                return new Token(TokenType.OpenParenthesis, null);
            case ')':
                _scanner.Advance();
                return new Token(TokenType.CloseParenthesis, null);
            case '+':
                _scanner.Advance();
                return new Token(TokenType.PlusSign, null);
            case '-':
                _scanner.Advance();
                return new Token(TokenType.MinusSign, null);
            case '>':
                if (_scanner.Peek(1) == '=')
                {
                    _scanner.Advance();
                    _scanner.Advance();
                    return new Token(TokenType.NonStrictMore, null);
                }

                _scanner.Advance();
                return new Token(TokenType.MoreThan, null);
            case '<':
                if (_scanner.Peek(1) == '=')
                {
                    _scanner.Advance();
                    _scanner.Advance();
                    return new Token(TokenType.NonStrictLess, null);
                }

                _scanner.Advance();
                return new Token(TokenType.LessThan, null);
            case '=':
                if (_scanner.Peek(1) == '=')
                {
                    _scanner.Advance();
                    _scanner.Advance();
                    return new Token(TokenType.EqualSign, null);
                }

                _scanner.Advance();
                return new Token(TokenType.Error, new TokenValue(ch + _scanner.Peek(1)));
            case '*':
                if (_scanner.Peek(1) == '*')
                {
                    _scanner.Advance();
                    _scanner.Advance();
                    return new Token(TokenType.ExponentSign, null);
                }

                _scanner.Advance();
                return new Token(TokenType.MultiplySign, null);
            case '^':
                _scanner.Advance();
                return new Token(TokenType.ExponentSign, null);
            case '/':
                _scanner.Advance();
                return new Token(TokenType.DivisionSign, null);
            case '%':
                _scanner.Advance();
                return new Token(TokenType.ModuloSign, null);
            case ',':
                _scanner.Advance();
                return new Token(TokenType.Comma, null);
            case '!':
                if (_scanner.Peek(1) == '=')
                {
                    _scanner.Advance();
                    _scanner.Advance();
                    return new Token(TokenType.UnequalSign, null);
                }

                _scanner.Advance();
                return new Token(TokenType.LogicalNot, null);
            case '&':
                if (_scanner.Peek(1) == '&')
                {
                    _scanner.Advance();
                    _scanner.Advance();
                    return new Token(TokenType.LogicalAnd, null);
                }

                _scanner.Advance();
                return new Token(TokenType.Error, new TokenValue("&"));
            case '|':
                if (_scanner.Peek(1) == '|')
                {
                    _scanner.Advance();
                    _scanner.Advance();
                    return new Token(TokenType.LogicalOr, null);
                }

                _scanner.Advance();
                return new Token(TokenType.Error, new TokenValue("|"));
            case ':':
                _scanner.Advance();
                return new Token(TokenType.Colon, null);
            default:
                _scanner.Advance();
                return new Token(TokenType.Error, new TokenValue(ch));
        }
    }

    /// <summary>
    /// Проверяет литерал строки.
    /// Корректным литералом строки считается любой набор символов,
    /// заключенный в двойные кавычки - '"'.
    /// </summary>
    private Token ParseString()
    {
        _scanner.Advance();
        StringBuilder sbTotal = new StringBuilder();
        while (_scanner.Peek() != '"')
        {
            if (_scanner.IsEnd())
            {
                return new Token(TokenType.Error, new TokenValue("Unterminated string"));
            }

            if (_scanner.Peek() == '\\')
            {
                _scanner.Advance();

                if (_scanner.IsEnd())
                {
                    return new Token(TokenType.Error, new TokenValue("Unterminated string"));
                }

                char nextChar = _scanner.Peek();

                switch (nextChar)
                {
                    case 'n':
                        sbTotal.Append('\n');
                        break;
                    case 't':
                        sbTotal.Append('\t');
                        break;
                    case '\\':
                        sbTotal.Append('\\');
                        break;
                    case '"':
                        sbTotal.Append('"');
                        break;
                    default:
                        sbTotal.Append('\\');
                        sbTotal.Append(nextChar);
                        break;
                }

                _scanner.Advance();
            }
            else
            {
                sbTotal.Append(_scanner.Peek());
                _scanner.Advance();
            }
        }

        _scanner.Advance();
        return new Token(TokenType.StringLiteral, new TokenValue(sbTotal.ToString()));
    }
}

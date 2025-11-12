using System.Text;

namespace Lexer;

public class Token
{
    private readonly TokenType _type;
    private readonly TokenValue? _value = null;

    public TokenType Type => _type;

    public TokenValue? Value => _value;

    public Token(TokenType type, TokenValue? value)
    {
        _type = type;
        _value = value;
    }

    /// <summary>
    /// Сравнивает токен по типу и значению
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is Token otherToken)
        {
            return otherToken._type == _type && Equals(otherToken._value, _value);
        }

        return false;
    }

    /// <summary>
    /// Возвращает хэш-код от свойств токена(пара тип-значение).
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine((int)_type, _value);
    }

    /// <summary>
    /// Форматирует токен в стиле "Type (Value)".
    /// </summary>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(_type.ToString());
        if (_value != null)
        {
            sb.Append($" ({_value})");
        }

        return sb.ToString();
    }
}

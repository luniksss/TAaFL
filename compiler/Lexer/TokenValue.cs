using System.Globalization;

namespace Lexer;

public class TokenValue
{
    private readonly object _value;

    public TokenValue(string value)
    {
        _value = value;
    }

    public TokenValue(double value)
    {
        _value = value;
    }

    /// <summary>
    /// Возвращает значение токена в виде строки.
    /// </summary>
    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            double d => d.ToString(CultureInfo.InvariantCulture),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Возвращает значение токена в виде числа.
    /// </summary>
    public double ToDouble()
    {
        return _value switch
        {
            string s => double.Parse(s),
            double d => d,
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Проверяет равенство значений токенов.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is TokenValue otherToken)
        {
            return _value switch
            {
                string s => (string)otherToken._value == s,
                double d => (double)otherToken._value == d,
                _ => throw new NotImplementedException(),
            };
        }

        return false;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}

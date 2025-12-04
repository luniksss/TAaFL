using Lexer;

namespace Parser;

/// <summary>
/// Представляет поток токенов с двумя операциями:
///  - Peek() возвращает текущий токен
///  - Advance() переходит к следующему токену
/// </summary>
public class TokenStream
{
    private readonly Lexer.Lexer _lexer;
    private Token _newToken;

    public TokenStream(string text)
    {
        _lexer = new Lexer.Lexer(text);
        _newToken = _lexer.ParseToken();
    }

    public Token Peek()
    {
        return _newToken;
    }

    public void Advance()
    {
        _newToken = _lexer.ParseToken();
    }
}

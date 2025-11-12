using System.Runtime.CompilerServices;

namespace Lexer;

/// <summary>
/// Сканирует текст МЯУ#, предоставляя три операции: Peek(n), Advance(), IsEnd()
/// </summary>
public class TextScanner
{
    private readonly string _input;
    private int _position;

    public TextScanner(string input)
    {
        _input = input;
    }

    /// <summary>
    /// Читает на n-символов вперёд текущей позиции.
    /// </summary>
    public char Peek(int n = 0)
    {
        int position = _position + n;
        return position >= _input.Length ? '\0' : _input[position];
    }

    /// <summary>
    /// Сдвигает текущую позицию на один символ вперед.
    /// </summary>
    public void Advance()
    {
        _position += 1;
    }

    /// <summary>
    /// Проверяет, находится ли текущая позиция за пределами строки.
    /// </summary>
    public bool IsEnd()
    {
        return _position >= _input.Length;
    }
}

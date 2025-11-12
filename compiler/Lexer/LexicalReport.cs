namespace Lexer;

public struct LexicalReport
{
    public int Keywords { get; set; }

    public int Identifiers { get; set; }

    public int NumberLiterals { get; set; }

    public int StringLiterals { get; set; }

    public int Operators { get; set; }

    public int OtherLexemes { get; set; }

    public override readonly string ToString() =>
    $"""
    keywords: {Keywords}
    identifier: {Identifiers}
    number literals: {NumberLiterals}
    string literals: {StringLiterals}
    operators: {Operators}
    other lexemes: {OtherLexemes}
    """;
}

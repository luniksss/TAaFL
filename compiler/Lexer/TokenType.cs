namespace Lexer;

public enum TokenType
{
    /// <summary>
    ///  Недопустимая лексема.
    /// </summary>
    Error,

    /// <summary>
    ///  Ключевое слово мур.
    /// </summary>
    Let,

    /// <summary>
    ///  Ключевое слово строгиймур.
    /// </summary>
    Const,

    /// <summary>
    ///  Ключевое слово цыфорка.
    /// </summary>
    Int,

    /// <summary>
    ///  Ключевое слово рилцыфорка.
    /// </summary>
    Double,

    /// <summary>
    ///  Ключевое слово кринжли.
    /// </summary>
    Bool,

    /// <summary>
    ///  Ключевое слово триппитроппа.
    /// </summary>
    If,

    /// <summary>
    ///  Ключевое слово троппатриппи.
    /// </summary>
    Else,

    /// <summary>
    ///  Идентификатор (имя символа).
    /// </summary>
    Identifier,

    /// <summary>
    ///  Литерал числа.
    /// </summary>
    NumericLiteral,

    /// <summary>
    ///  Оператор сложения '+'.
    /// </summary>
    PlusSign,

    /// <summary>
    ///  Оператор вычитания '-'.
    /// </summary>
    MinusSign,

    /// <summary>
    ///  Оператор умножения '*'.
    /// </summary>
    MultiplySign,

    /// <summary>
    ///  Оператор сравнения меньше '<'.
    /// </summary>
    LessThan,

    /// <summary>
    ///  Оператор сравнения больше '>'.
    /// </summary>
    MoreThan,

    /// <summary>
    ///  Открывающая круглая скобка '('.
    /// </summary>
    OpenParenthesis,

    /// <summary>
    ///  Закрывающая круглая скобка ')'.
    /// </summary>
    CloseParenthesis,

    /// <summary>
    ///  Открывающий оператор вложенности '{'.
    /// </summary>
    OpenBrace,

    /// <summary>
    ///  Закрывающий оператор вложенности '}'.
    /// </summary>
    CloseBrace,

    /// <summary>
    ///  Разделитель параметров ','.
    /// </summary>
    Comma,

    /// <summary>
    ///  Оператор присваивания "царапнуть".
    /// </summary>
    Assign,

    /// <summary>
    ///  Разделитель строки "мяу".
    /// </summary>
    Semicolon,

    /// <summary>
    ///  Конец файла.
    /// </summary>
    EndOfFile,

    /// <summary>
    ///  Нестрогое больше ">=".
    /// </summary>
    NonStrictMore,

    /// <summary>
    ///  Нестрогое меньше "<=".
    /// </summary>
    NonStrictLess,

    /// <summary>
    ///  Проверка равенства "==".
    /// </summary>
    EqualSign,

    /// <summary>
    ///  Возведение в степень "^".
    /// </summary>
    ExponentSign,

    /// <summary>
    ///  Деление нацело "/".
    /// </summary>
    DivisionSign,

    /// <summary>
    ///  Остаток от деления "%".
    /// </summary>
    ModuloSign,

    /// <summary>
    ///  Не равно "!=".
    /// </summary>
    UnequalSign,

    /// <summary>
    ///  Литерал строки.
    /// </summary>
    StringLiteral,

    /// <summary>
    ///  Ключевое слово "лапкапомощи" - объявление главной функции программы.
    /// </summary>
    Main,

    /// <summary>
    ///  Ключевое слово "нитка".
    /// </summary>
    String,

    /// <summary>
    ///  Ключевое слово "кринж".
    /// </summary>
    True,

    /// <summary>
    ///  Ключевое слово "некринж".
    /// </summary>
    False,

    /// <summary>
    ///  Ключевое слово "лапка" - объявление функции.
    /// </summary>
    Def,

    /// <summary>
    ///  Ключевое слово "вернуть" - возвращение из функции.
    /// </summary>
    Return,

    /// <summary>
    ///  Ключевое слово "магасияй" - цикл.
    /// </summary>
    While,

    /// <summary>
    ///  Ключевое слово "мурлыкать" - пользовательский ввод.
    /// </summary>
    Input,

    /// <summary>
    ///  Ключевое слово "клацать" - пользовательский вывод.
    /// </summary>
    Output,

    /// <summary>
    ///  Ключевое слово "клацать" - пользовательский вывод.
    /// </summary>
    Comment,

    /// <summary>
    /// Константа Pi (пи).
    /// </summary>
    Pi,

    /// <summary>
    /// Константа Euler (эклер).
    /// </summary>
    Euler,

    /// <summary>
    /// Логическое И (&&).
    /// </summary>
    LogicalAnd,

    /// <summary>
    /// Логическое ИЛИ (||).
    /// </summary>
    LogicalOr,

    /// <summary>
    /// Логическое NOT (!).
    /// </summary>
    LogicalNot,
}

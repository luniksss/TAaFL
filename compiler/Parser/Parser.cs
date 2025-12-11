using Execution;
using Lexer;

namespace Parser;

/// <summary>
/// Выполняет синтаксический разбор программы на языке МЯУ#.
/// Грамматика программы описана в top-level-grammar.
/// </summary>
public class Parser(Context context, IEnvironment environment, string code)
{
    private readonly TokenStream tokens = new TokenStream(code);
    private readonly Context context = context;
    private readonly IEnvironment environment = environment;
    private readonly Dictionary<string, int> _functionParameterCounts = new();

    public Parser(string code)
        : this(new Context(), new ConsoleEnvironment(), code)
    {
    }

    /// <summary>
    /// Выполняет парсинг выражения.
    /// </summary>
    public static double ExecuteExpr(string expr)
    {
        Context ctx = new Context();
        IEnvironment env = new ConsoleEnvironment();
        Parser p = new Parser(ctx, env, expr);
        return p.ParseExpr();
    }

    /// <summary>
    /// Выполняет парсинг программы.
    /// </summary>
    public static void ParseProgram(string code)
    {
        Context ctx = new Context();
        IEnvironment env = new ConsoleEnvironment();
        Parser p = new Parser(ctx, env, code);
        p.ParseProgram();
    }

    /// <summary>
    /// Парсинг программы.
    /// program = { functionDeclaration }, mainFunction, { functionDeclaration }.
    /// </summary>
    private void ParseProgram()
    {
        while (tokens.Peek().Type == TokenType.Def)
        {
            ParseFunctionDeclaration();
        }

        ParseMainFunction();
        while (tokens.Peek().Type == TokenType.Def)
        {
            ParseFunctionDeclaration();
        }

        if (tokens.Peek().Type != TokenType.EndOfFile)
        {
            throw new UnexpectedLexemeException(TokenType.EndOfFile, tokens.Peek());
        }
    }

    /// <summary>
    /// Парсинг главной функции.
    /// mainFunction = "лапкапомощи", "(", ")", block.
    /// </summary>
    private void ParseMainFunction()
    {
        Match(TokenType.Main);
        Match(TokenType.OpenParenthesis);
        Match(TokenType.CloseParenthesis);

        context.PushScope();
        ParseBlock();
        context.PopScope();
    }

    /// <summary>
    /// Парсинг объявления функции.
    /// functionDeclaration = "лапка", identifier, "(", [ parameterList ], ")", [":", returnType ], block.
    /// </summary>
    private void ParseFunctionDeclaration()
    {
        Match(TokenType.Def);
        if (tokens.Peek().Type != TokenType.Identifier)
        {
            throw new UnexpectedLexemeException(TokenType.Identifier, tokens.Peek());
        }

        string functionName = tokens.Peek().Value!.ToString()!;
        tokens.Advance();

        if (context.IsFunctionLocal(functionName))
        {
            throw new InvalidOperationException($"Function '{functionName}' is already declared");
        }

        Match(TokenType.OpenParenthesis);

        List<string> parameterNames = new();
        if (tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            ParseParameterList(parameterNames);
        }

        Match(TokenType.CloseParenthesis);

        if (tokens.Peek().Type == TokenType.Colon)
        {
            tokens.Advance();
            ParseReturnType();
        }

        // global scope
        context.DefineFunction(functionName);
        _functionParameterCounts[functionName] = parameterNames.Count;

        // new scope
        context.PushScope();
        for (int i = 0; i < parameterNames.Count; i++)
        {
            context.DefineVariable(parameterNames[i], 0);
        }

        ParseBlock();

        // return scope
        context.PopScope();
    }

    /// <summary>
    /// Парсинг списка параметров.
    /// parameterList = parameter, { ",", parameter }.
    /// parameter = type, identifier.
    /// </summary>
    private void ParseParameterList(List<string> parameterNames)
    {
        // first type parameter
        SkipType();
        if (tokens.Peek().Type != TokenType.Identifier)
        {
            throw new UnexpectedLexemeException(TokenType.Identifier, tokens.Peek());
        }

        string paramName = tokens.Peek().Value!.ToString()!;
        tokens.Advance();
        parameterNames.Add(paramName);

        // other parameters
        while (tokens.Peek().Type == TokenType.Comma)
        {
            tokens.Advance();
            SkipType();
            if (tokens.Peek().Type != TokenType.Identifier)
            {
                throw new UnexpectedLexemeException(TokenType.Identifier, tokens.Peek());
            }

            paramName = tokens.Peek().Value!.ToString()!;
            tokens.Advance();
            parameterNames.Add(paramName);
        }
    }

    /// <summary>
    /// Парсинг типа возврата (пропускаем токен типа).
    /// returnType = "цыфорка" | "рилцыфорка" | "кринжли" | "ничего".
    /// </summary>
    private void ParseReturnType()
    {
        TokenType tokenType = tokens.Peek().Type;
        if (tokenType != TokenType.Int && tokenType != TokenType.Double && tokenType != TokenType.Bool && tokenType != TokenType.Void)
        {
            throw new UnexpectedLexemeException(TokenType.Int, tokens.Peek());
        }

        tokens.Advance();
    }

    /// <summary>
    /// Пропускает токен типа.
    /// type = "цыфорка" | "рилцыфорка" | "кринжли".
    /// </summary>
    private void SkipType()
    {
        TokenType tokenType = tokens.Peek().Type;
        if (tokenType != TokenType.Int && tokenType != TokenType.Double && tokenType != TokenType.Bool)
        {
            throw new UnexpectedLexemeException(TokenType.Int, tokens.Peek());
        }

        tokens.Advance();
    }

    /// <summary>
    /// Парсинг блока.
    /// block = "{", { statement }, "}".
    /// </summary>
    private void ParseBlock()
    {
        Match(TokenType.OpenBrace);

        // new scope
        context.PushScope();

        while (tokens.Peek().Type != TokenType.CloseBrace)
        {
            ParseStatement();
        }

        // return scope
        context.PopScope();
        Match(TokenType.CloseBrace);
    }

    /// <summary>
    /// Пропускает блок без выполнения.
    /// block = "{", { statement }, "}".
    /// </summary>
    private void SkipBlock()
    {
        Match(TokenType.OpenBrace);
        int braceCount = 1;
        while (braceCount > 0)
        {
            TokenType tokenType = tokens.Peek().Type;
            tokens.Advance();
            if (tokenType == TokenType.OpenBrace)
            {
                braceCount++;
            }
            else if (tokenType == TokenType.CloseBrace)
            {
                braceCount--;
            }
        }
    }

    /// <summary>
    /// Парсинг инструкции.
    /// statement = commonStatement, instructionDelimiter.
    /// </summary>
    private void ParseStatement()
    {
        ParseCommonStatement();
        Match(TokenType.Semicolon);
    }

    /// <summary>
    /// Парсинг общей инструкции.
    /// commonStatement = (variableDecl | constantDecl | assignment | inputStatement | outputStatement |
    ///                    ifStatement | whileStatement | doWhileStatement | forStatement |
    ///                    returnStatement | functionCall).
    /// </summary>
    private void ParseCommonStatement()
    {
        TokenType tokenType = tokens.Peek().Type;

        switch (tokenType)
        {
            case TokenType.Let:
                ParseVariableDecl();
                break;
            case TokenType.Const:
                ParseConstantDecl();
                break;
            case TokenType.If:
                ParseIfStatement();
                break;
            case TokenType.While:
                break;
            case TokenType.DoWhile:
                break;
            case TokenType.For:
                break;
            case TokenType.Return:
                ParseReturnStatement();
                break;
            case TokenType.Input:
                ParseInputStatement();
                break;
            case TokenType.Output:
                ParseOutputStatement();
                break;
            case TokenType.Break:
                break;
            case TokenType.Identifier:
                // functionCall or assignment
                string identifierName = tokens.Peek().Value!.ToString()!;
                tokens.Advance();
                if (tokens.Peek().Type == TokenType.Assign)
                {
                    ParseAssignment(identifierName);
                }
                else if (tokens.Peek().Type == TokenType.OpenParenthesis)
                {
                    ParseFunctionCall(identifierName);
                }
                else
                {
                    throw new UnexpectedLexemeException(TokenType.Assign, tokens.Peek());
                }

                break;
            default:
                throw new UnexpectedLexemeException(TokenType.Let, tokens.Peek());
        }
    }

    /// <summary>
    /// Парсинг объявления переменной.
    /// variableDecl = "мур", type, identifier, [ "царапнуть", expression ].
    /// </summary>
    private void ParseVariableDecl()
    {
        Match(TokenType.Let);
        SkipType();

        if (tokens.Peek().Type != TokenType.Identifier)
        {
            throw new UnexpectedLexemeException(TokenType.Identifier, tokens.Peek());
        }

        string varName = tokens.Peek().Value!.ToString()!;
        tokens.Advance();
        if (context.HasValueLocal(varName))
        {
            throw new InvalidOperationException($"Variable '{varName}' is already declared in this scope");
        }

        if (context.IsConstant(varName))
        {
            throw new InvalidOperationException($"Cannot declare variable '{varName}' - name is already used by constant");
        }

        if (context.IsFunction(varName))
        {
            throw new InvalidOperationException($"Cannot declare variable '{varName}' - name is already used by function");
        }

        // initialization
        double value = 0;
        if (tokens.Peek().Type == TokenType.Assign)
        {
            tokens.Advance();
            value = ParseExpr();
        }

        context.DefineVariable(varName, value);
    }

    /// <summary>
    /// Парсинг объявления константы.
    /// constantDecl = "строгиймур", type, identifier, "царапнуть", expression.
    /// </summary>
    private void ParseConstantDecl()
    {
        Match(TokenType.Const);
        SkipType();

        if (tokens.Peek().Type != TokenType.Identifier)
        {
            throw new UnexpectedLexemeException(TokenType.Identifier, tokens.Peek());
        }

        string constName = tokens.Peek().Value!.ToString()!;
        tokens.Advance();
        if (context.HasValue(constName))
        {
            throw new InvalidOperationException($"Cannot declare constant '{constName}' - name is already used");
        }

        if (context.IsFunction(constName))
        {
            throw new InvalidOperationException($"Cannot declare constant '{constName}' - name is already used by function");
        }

        Match(TokenType.Assign);
        double value = ParseExpr();

        context.DefineConstant(constName, value);
    }

    /// <summary>
    /// Парсинг присваивания.
    /// assignment = identifier, "царапнуть", expression.
    /// </summary>
    private void ParseAssignment(string varName)
    {
        if (!context.HasValue(varName))
        {
            throw new InvalidOperationException($"Variable '{varName}' is not declared");
        }

        if (context.IsConstant(varName))
        {
            throw new InvalidOperationException($"Cannot assign to '{varName}' - it is a constant");
        }

        if (context.IsFunction(varName))
        {
            throw new InvalidOperationException($"Cannot assign to '{varName}' - it is a function");
        }

        Match(TokenType.Assign);
        double value = ParseExpr();
        context.AssignVariable(varName, value);
    }

    /// <summary>
    /// Парсинг инструкции ввода.
    /// inputStatement = "клацать", "(", identifier, ")".
    /// </summary>
    private void ParseInputStatement()
    {
        Match(TokenType.Input);
        Match(TokenType.OpenParenthesis);

        if (tokens.Peek().Type != TokenType.Identifier)
        {
            throw new UnexpectedLexemeException(TokenType.Identifier, tokens.Peek());
        }

        string varName = tokens.Peek().Value!.ToString()!;
        tokens.Advance();

        if (!context.HasValue(varName))
        {
            throw new InvalidOperationException($"Variable '{varName}' is not declared");
        }

        Match(TokenType.CloseParenthesis);
        double inputValue = environment.ReadNumber();
        context.AssignVariable(varName, inputValue);
    }

    /// <summary>
    /// Парсинг инструкции вывода.
    /// outputStatement = "мурлыкать", "(", expression, { ",", expression }, ")".
    /// </summary>
    private void ParseOutputStatement()
    {
        Match(TokenType.Output);
        Match(TokenType.OpenParenthesis);

        double value = ParseExpr();
        environment.WriteNumber(value);
        while (tokens.Peek().Type == TokenType.Comma)
        {
            tokens.Advance();
            value = ParseExpr();
            environment.WriteNumber(value);
        }

        Match(TokenType.CloseParenthesis);
    }

    /// <summary>
    /// Парсинг условной инструкции.
    /// ifStatement = "триппитроппа", "(", expression, ")", block, [ elseStatement ].
    /// </summary>
    private void ParseIfStatement()
    {
        Match(TokenType.If);
        Match(TokenType.OpenParenthesis);
        double condition = ParseExpr();
        Match(TokenType.CloseParenthesis);
        if (condition != 0)
        {
            ParseBlock();
        }
        else
        {
            SkipBlock();
        }

        if (tokens.Peek().Type == TokenType.Else)
        {
            tokens.Advance();
            if (condition == 0)
            {
                ParseBlock();
            }
            else
            {
                SkipBlock();
            }
        }
    }

    /// <summary>
    /// Парсинг инструкции return.
    /// returnStatement = "вернуть", [ expression ].
    /// </summary>
    private void ParseReturnStatement()
    {
        Match(TokenType.Return);
        if (tokens.Peek().Type != TokenType.Semicolon && tokens.Peek().Type != TokenType.CloseBrace)
        {
            ParseExpr();
        }
    }

    /// <summary>
    /// Парсинг вызова функции.
    /// functionCall = identifier, "(", [ argumentList ], ")".
    /// argumentList = identifier, { ",", identifier }.
    /// </summary>
    private void ParseFunctionCall(string functionName)
    {
/*    if (!context.IsFunction(functionName))
    {
        throw new InvalidOperationException($"Function '{functionName}' is not declared");
    }

    Match(TokenType.OpenParenthesis);

    int argCount = 0;
    if (tokens.Peek().Type != TokenType.CloseParenthesis)
    {
        if (tokens.Peek().Type != TokenType.Identifier)
        {
            throw new UnexpectedLexemeException(TokenType.Identifier, tokens.Peek());
        }

        string argName = tokens.Peek().Value!.ToString()!;
        tokens.Advance();
        argCount++;
        if (!context.HasValue(argName))
        {
            throw new InvalidOperationException($"Variable '{argName}' is not declared");
        }

        while (tokens.Peek().Type == TokenType.Comma)
        {
            tokens.Advance();
            if (tokens.Peek().Type != TokenType.Identifier)
            {
                throw new UnexpectedLexemeException(TokenType.Identifier, tokens.Peek());
            }

            argName = tokens.Peek().Value!.ToString()!;
            tokens.Advance();
            argCount++;

            if (!context.HasValue(argName))
            {
                throw new InvalidOperationException($"Variable '{argName}' is not declared");
            }
        }
    }

    if (_functionParameterCounts.TryGetValue(functionName, out int expectedCount) && expectedCount != argCount)
    {
        throw new InvalidOperationException($"Function '{functionName}' expects {expectedCount} arguments, but {argCount} provided");
    }

    Match(TokenType.CloseParenthesis);*/
    }

    /// <summary>
    /// Парсинг выражения.
    /// expression = logicalOr.
    /// </summary>
    private double ParseExpr()
    {
        return ParseOrExpr();
    }

    /// <summary>
    /// Парсинг выражения или.
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
    /// Парсинг выражения и.
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
    /// Парсинг сравнения.
    /// comparison = additive, [ ( ">" | ">=" | "<" | "<=" | "==" | "!=" ), additive ].
    /// </summary>
    private double ParseComparisonExpr()
    {
    double leftValue = ParseAdditiveExpr();
    if (tokens.Peek().Type == TokenType.LessThan || tokens.Peek().Type == TokenType.MoreThan ||
        tokens.Peek().Type == TokenType.NonStrictLess || tokens.Peek().Type == TokenType.NonStrictMore ||
        tokens.Peek().Type == TokenType.EqualSign || tokens.Peek().Type == TokenType.UnequalSign)
    {
        TokenType op = tokens.Peek().Type;
        tokens.Advance();
        double rightValue = ParseAdditiveExpr();

        return op switch
        {
        TokenType.LessThan => (leftValue < rightValue) ? 1 : 0,
        TokenType.MoreThan => (leftValue > rightValue) ? 1 : 0,
        TokenType.NonStrictLess => (leftValue <= rightValue) ? 1 : 0,
        TokenType.NonStrictMore => (leftValue >= rightValue) ? 1 : 0,
        TokenType.EqualSign => (leftValue == rightValue) ? 1 : 0,
        TokenType.UnequalSign => (leftValue != rightValue) ? 1 : 0,
        _ => throw new InvalidOperationException("Unknown comparison operator"),
        };
    }

    return leftValue;
    }

    /// <summary>
    /// Парсинг сложения/вычитания.
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
    /// Парсинг умножения/деления/остатка.
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
    /// Парсинг унарного выражения.
    /// unary = [ "!" ], [ "+" | "-" ], power.
    /// Унарный минус применяется ПОСЛЕ возведения в степень для правильного приоритета.
    /// </summary>
    private double ParseUnaryExpr()
    {
    if (tokens.Peek().Type == TokenType.LogicalNot)
    {
        tokens.Advance();
        double value = ParseUnaryExpr();
        return value == 0 ? 1 : 0;
    }

    bool isNegative = false;
    if (tokens.Peek().Type == TokenType.MinusSign)
    {
        tokens.Advance();
        isNegative = true;
    }
    else if (tokens.Peek().Type == TokenType.PlusSign)
    {
        tokens.Advance();
    }

    double value2 = ParsePowerExpr();
    return isNegative ? -value2 : value2;
    }

    /// <summary>
    /// Парсинг возведения в степень.
    /// power = primary, [ "^", unary ].
    /// Правая часть должна парситься через unary, чтобы правильно обработать унарный минус.
    /// </summary>
    private double ParsePowerExpr()
    {
        double value = ParsePrimaryExpr();
        if (tokens.Peek().Type == TokenType.ExponentSign)
        {
            tokens.Advance();
            double right = ParseUnaryExpr();
            value = Math.Pow(value, right);
        }

        return value;
    }

    /// <summary>
    /// Парсинг основного выражения.
    /// primary = number | constant | boolean | identifierOrCall | "(", expression, ")".
    /// </summary>
    private double ParsePrimaryExpr()
    {
        Token t = tokens.Peek();
        switch (t.Type)
        {
            case TokenType.NumericLiteral:
                return ParseLiteral();
            case TokenType.True:
            case TokenType.False:
                return ParseBool();
            case TokenType.Pi:
            case TokenType.Euler:
                return ParseConstant();
            case TokenType.Identifier:
                // function call/variable/constant
                string name = t.Value!.ToString()!;
                tokens.Advance();
                if (tokens.Peek().Type == TokenType.OpenParenthesis)
                {
                    // function call
                    tokens.Advance();
                    while (tokens.Peek().Type != TokenType.CloseParenthesis)
                    {
                        if (tokens.Peek().Type == TokenType.Identifier)
                        {
                            tokens.Advance();
                        }
                        else if (tokens.Peek().Type == TokenType.Comma)
                        {
                            tokens.Advance();
                        }
                        else
                        {
                            ParseExpr();
                        }
                    }

                    tokens.Advance();
                    return 0;
                }
                else
                {
                    // var / const
                    return !context.HasValue(name)
                        ? throw new InvalidOperationException($"Variable or constant '{name}' is not declared")
                        : context.GetValue(name);
                }

            case TokenType.OpenParenthesis:
                tokens.Advance();
                double value = ParseExpr();
                Match(TokenType.CloseParenthesis);
                return value;
            default:
                throw new UnexpectedLexemeException(TokenType.Identifier, t);
        }
    }

    /// <summary>
    /// Парсинг литерала числа.
    /// </summary>
    private double ParseLiteral()
    {
        Token t = tokens.Peek();
        if (t.Type == TokenType.NumericLiteral)
        {
            double value = t.Value!.ToDouble();
            tokens.Advance();
            return value;
        }

        throw new UnexpectedLexemeException(TokenType.NumericLiteral, t);
    }

    /// <summary>
    /// Парсинг логического значения.
    /// boolean = "кринж" | "некринж".
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

    /// <summary>
    /// Парсинг констант.
    /// constant = "пи" | "эклер".
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

    private void Match(TokenType expected)
    {
        if (tokens.Peek().Type != expected)
        {
            throw new UnexpectedLexemeException(expected, tokens.Peek());
        }

        tokens.Advance();
    }
}

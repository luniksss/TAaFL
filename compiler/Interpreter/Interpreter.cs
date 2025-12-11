using Execution;

namespace Interpreter;

/// <summary>
/// Интерпретатор для выполнения программ.
/// </summary>
public class Interpreter
{
    private readonly Context _context;
    private readonly IEnvironment _environment;

    public Interpreter()
        : this(new Context(), new ConsoleEnvironment())
    {
    }

    public Interpreter(Context context, IEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    /// <summary>
    /// Выполняет программу из переданного кода.
    /// </summary>
    public void Execute(string code)
    {
        Parser.Parser parser = new Parser.Parser(_context, _environment, code);
        parser.ExecuteProgram();
    }
}

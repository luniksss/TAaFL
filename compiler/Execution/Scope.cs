namespace Execution;

/// <summary>
/// Представляет область видимости для выполнения программы.
/// Хранит значения переменных и констант в текущей области видимости.
/// </summary>
public class Scope
{
    private readonly Dictionary<string, double> _variables = new();
    private readonly HashSet<string> _constants = new();
    private readonly HashSet<string> _functions = new();
    private readonly Scope? _parent;

    public Scope(Scope? parent = null)
    {
    _parent = parent;
    }

    /// <summary>
    /// Определяет переменную с начальным значением в текущей области видимости.
    /// </summary>
    public void DefineVariable(string name, double value)
    {
        if (_variables.ContainsKey(name))
        {
            throw new InvalidOperationException($"Variable '{name}' is already declared in this scope");
        }

        _variables[name] = value;
    }

    /// <summary>
    /// Присваивает значение переменной. Ищет переменную в текущей области и родительских областях.
    /// </summary>
    public void AssignVariable(string name, double value)
    {
        if (_constants.Contains(name))
        {
            throw new InvalidOperationException($"Cannot assign to constant '{name}'");
        }

        if (_variables.ContainsKey(name))
        {
            _variables[name] = value;
            return;
        }

        if (_parent != null)
        {
            _parent.AssignVariable(name, value);
            return;
        }

        throw new InvalidOperationException($"Variable '{name}' is not declared");
    }

    /// <summary>
    /// Определяет константу со значением в текущей области видимости.
    /// </summary>
    public void DefineConstant(string name, double value)
    {
        if (_variables.ContainsKey(name) || _constants.Contains(name))
        {
            throw new InvalidOperationException($"Constant '{name}' is already declared in this scope");
        }

        _constants.Add(name);
        _variables[name] = value;
    }

    /// <summary>
    /// Получает значение переменной или константы. Ищет в текущей области и родительских областях.
    /// </summary>
    public double GetValue(string name)
    {
        if (_variables.TryGetValue(name, out double value))
        {
            return value;
        }

        if (_parent != null)
        {
            return _parent.GetValue(name);
        }

        throw new InvalidOperationException($"Variable or constant '{name}' is not declared");
    }

    /// <summary>
    /// Проверяет, существует ли переменная или константа в текущей области или родительских областях.
    /// </summary>
    public bool HasValue(string name)
    {
        if (_variables.ContainsKey(name))
        {
            return true;
        }

        return _parent?.HasValue(name) ?? false;
    }

    /// <summary>
    /// Получает родительскую область видимости.
    /// </summary>
    public Scope? GetParent()
    {
        return _parent;
    }

    /// <summary>
    /// Проверяет, является ли переменная константой.
    /// </summary>
    public bool IsConstant(string name)
    {
        if (_constants.Contains(name))
        {
            return true;
        }

        return _parent?.IsConstant(name) ?? false;
    }

    /// <summary>
    /// Проверяет, существует ли переменная или константа только в текущей области видимости (без подъема вверх).
    /// </summary>
    public bool HasValueLocal(string name)
    {
        return _variables.ContainsKey(name);
    }

    /// <summary>
    /// Определяет функцию в текущей области видимости.
    /// </summary>
    public void DefineFunction(string name)
    {
        if (_variables.ContainsKey(name) || _constants.Contains(name) || _functions.Contains(name))
        {
            throw new InvalidOperationException($"Function '{name}' is already declared");
        }

        _functions.Add(name);
    }

    /// <summary>
    /// Проверяет, является ли имя функцией.
    /// </summary>
    public bool IsFunction(string name)
    {
        if (_functions.Contains(name))
        {
            return true;
        }

        return _parent?.IsFunction(name) ?? false;
    }

    /// <summary>
    /// Проверяет, является ли имя функцией только в текущей области видимости (без подъема вверх).
    /// </summary>
    public bool IsFunctionLocal(string name)
    {
        return _functions.Contains(name);
    }
}

namespace Execution;

/// <summary>
/// Контекст выполнения программы - управляет областями видимости и значениями переменных/констант.
/// </summary>
public class Context
{
    private Scope? _currentScope;

    public Context()
    {
    _currentScope = new Scope();
    }

    public Context(Scope scope)
    {
    _currentScope = scope;
    }

    /// <summary>
    /// Добавляет новую область видимости в стек.
    /// </summary>
    public Scope PushScope()
    {
        _currentScope = new Scope(_currentScope);
        return _currentScope;
    }

    /// <summary>
    /// Добавляет указанную область видимости в стек.
    /// </summary>
    public void PushScope(Scope scope)
    {
        _currentScope = scope;
    }

    /// <summary>
    /// Удаляет текущую область видимости из стека (возвращается к родительской).
    /// </summary>
    public void PopScope()
    {
        if (_currentScope == null)
        {
            throw new InvalidOperationException("Cannot pop scope - no scope available");
        }

        _currentScope = _currentScope.GetParent();
    }

    /// <summary>
    /// Определяет переменную в текущей области видимости.
    /// </summary>
    public void DefineVariable(string name, double value)
    {
        if (_currentScope == null)
        {
            throw new InvalidOperationException("No scope available");
        }

        _currentScope.DefineVariable(name, value);
    }

    /// <summary>
    /// Присваивает значение переменной.
    /// </summary>
    public void AssignVariable(string name, double value)
    {
        if (_currentScope == null)
        {
            throw new InvalidOperationException("No scope available");
        }

        _currentScope.AssignVariable(name, value);
    }

    /// <summary>
    /// Определяет константу в текущей области видимости.
    /// </summary>
    public void DefineConstant(string name, double value)
    {
        if (_currentScope == null)
        {
            throw new InvalidOperationException("No scope available");
        }

        _currentScope.DefineConstant(name, value);
    }

    /// <summary>
    /// Получает значение переменной или константы.
    /// </summary>
    public double GetValue(string name)
    {
        if (_currentScope == null)
        {
            throw new InvalidOperationException("No scope available");
        }

        return _currentScope.GetValue(name);
    }

    /// <summary>
    /// Проверяет, существует ли переменная или константа.
    /// </summary>
    public bool HasValue(string name)
    {
        if (_currentScope == null)
        {
            return false;
        }

        return _currentScope.HasValue(name);
    }

    /// <summary>
    /// Проверяет, является ли переменная константой.
    /// </summary>
    public bool IsConstant(string name)
    {
        if (_currentScope == null)
        {
            return false;
        }

        return _currentScope.IsConstant(name);
    }

    /// <summary>
    /// Проверяет, существует ли переменная или константа только в текущей области видимости (без подъема вверх).
    /// </summary>
    public bool HasValueLocal(string name)
    {
        if (_currentScope == null)
        {
            return false;
        }

        return _currentScope.HasValueLocal(name);
    }

    /// <summary>
    /// Получает текущую область видимости.
    /// </summary>
    public Scope GetCurrentScope()
    {
        if (_currentScope == null)
        {
            throw new InvalidOperationException("No scope available");
        }

        return _currentScope;
    }

    /// <summary>
    /// Определяет функцию в текущей области видимости.
    public void DefineFunction(string name)
    {
        if (_currentScope == null)
        {
            throw new InvalidOperationException("No scope available");
        }

        _currentScope.DefineFunction(name);
    }

    /// <summary>
    /// Проверяет, является ли имя функцией.
    /// </summary>
    public bool IsFunction(string name)
    {
        if (_currentScope == null)
        {
            return false;
        }

        return _currentScope.IsFunction(name);
    }

    /// <summary>
    /// Проверяет, является ли имя функцией только в текущей области видимости (без подъема вверх).
    /// </summary>
    public bool IsFunctionLocal(string name)
    {
        if (_currentScope == null)
        {
            return false;
        }

        return _currentScope.IsFunctionLocal(name);
    }
}

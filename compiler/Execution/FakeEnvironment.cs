namespace Execution;

/// <summary>
/// Симулирует ввод-вывод в тестах.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly Queue<double> _inputQueue = new();
    private readonly List<double> _outputList = new();

    /// <summary>
    /// Добавляет значение в очередь ввода.
    /// </summary>
    public void AddInput(double value)
    {
        _inputQueue.Enqueue(value);
    }

    /// <summary>
    /// Читает число из очереди ввода.
    /// </summary>
    public double ReadNumber()
    {
        if (_inputQueue.Count == 0)
        {
            return 0;
        }

        return _inputQueue.Dequeue();
    }

    /// <summary>
    /// Записывает число в список вывода.
    /// </summary>
    public void WriteNumber(double value)
    {
        _outputList.Add(value);
    }

    /// <summary>
    /// Получает список всех выведенных значений.
    /// </summary>
    public IReadOnlyList<double> GetOutput()
    {
        return _outputList.AsReadOnly();
    }

    /// <summary>
    /// Очищает список вывода.
    /// </summary>
    public void ClearOutput()
    {
        _outputList.Clear();
    }

    /// <summary>
    /// Очищает очередь ввода.
    /// </summary>
    public void ClearInput()
    {
        _inputQueue.Clear();
    }
}

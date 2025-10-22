using System.Text;
using System.Xml;

namespace ExampleLib;

public static class FileUtil
{
    /// <summary>
    /// Сортирует строки в указанном файле.
    /// Перезаписывает файл, но не атомарно: ошибка ввода-вывода при записи приведёт к потере данных.
    /// </summary>
    public static void SortFileLines(string path)
    {
        // Читаем и сортируем строки файла.
        List<string> lines = File.ReadLines(path, Encoding.UTF8).ToList();
        lines.Sort();

        // Перезаписываем файл с нуля (режим Truncate).
        using FileStream file = File.Open(path, FileMode.Truncate, FileAccess.Write);
        for (int i = 0, iMax = lines.Count; i < iMax; ++i)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(lines[i]);
            file.Write(bytes);
            if (i != iMax - 1)
            {
                file.Write("\n"u8);
            }
        }
    }

    /// <summary>
    /// Выравнивает все строки файла по правому краю,
    /// добавляя символ indentCharacter слева до длины строки = width
    /// </summary>
    public static void AlignRightAllLines(string path, int width)
    {
        string indentCharacter = " ";

        List<string> lines = File.ReadLines(path, Encoding.UTF8).ToList();

        using FileStream file = File.Open(path, FileMode.Truncate, FileAccess.Write);
        for (int i = 0, iMax = lines.Count; i < iMax; i++)
        {
            int spaceCount = width - lines[i].Length;
            string newLine = "";
            if (spaceCount > 0)
            {
                for (int j = 0, jMax = spaceCount; j < jMax; j++)
                {
                    newLine += indentCharacter;
                }
            }
            newLine += lines[i];
            file.Write(Encoding.UTF8.GetBytes(newLine));
            if (i != iMax - 1)
            {
                file.Write("\n"u8);
            }
        }
    }
}

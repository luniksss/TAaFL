using ExampleLib.UnitTests.Helpers;

using Xunit;

namespace ExampleLib.UnitTests;

public class FileUtilTests
{
    [Fact]
    public void CanSortTextFile()
    {
        const string unsorted = """
                                Играют волны — ветер свищет,
                                И мачта гнется и скрыпит…
                                Увы! он счастия не ищет
                                И не от счастия бежит!
                                """;
        const string sorted = """
                              И мачта гнется и скрыпит…
                              И не от счастия бежит!
                              Играют волны — ветер свищет,
                              Увы! он счастия не ищет
                              """;

        using TempFile file = TempFile.Create(unsorted);
        FileUtil.SortFileLines(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal(sorted.Replace("\r\n", "\n"), actual);
    }

    [Fact]
    public void CanSortOneLineFile()
    {
        using TempFile file = TempFile.Create("Играют волны — ветер свищет,");
        FileUtil.SortFileLines(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal("Играют волны — ветер свищет,", actual);
    }

    [Fact]
    public void CanSortEmptyFile()
    {
        using TempFile file = TempFile.Create("");

        FileUtil.SortFileLines(file.Path);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal("", actual);
    }

    [Fact]
    public void CanAlignTextFile()
    {
        const int width = 28;
        const string unaligned = """
            Играют волны — ветер свищет,
            И мачта гнется и скрыпит…
            Увы! он счастия не ищет
            И не от счастия бежит!
            """;
        const string aligned = """
            Играют волны — ветер свищет,
               И мачта гнется и скрыпит…
                 Увы! он счастия не ищет
                  И не от счастия бежит!
            """;

        using TempFile file = TempFile.Create(unaligned);
        FileUtil.AlignRightAllLines(file.Path, width);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal(aligned.Replace("\r\n", "\n"), actual);
    }

    [Fact]
    public void CanAlignEmptyFile()
    {
        using TempFile file = TempFile.Create("");

        FileUtil.AlignRightAllLines(file.Path, 10);

        string actual = File.ReadAllText(file.Path);
        Assert.Equal("", actual);
    }
}
using Execution;

namespace Interpreter.Specs;

public class InterpreterSpecs
{
    [Theory]
    [MemberData(nameof(GetBoolTests))]
    public void TestInterpreterWithIfElse(double input, double expected)
    {
        string program = @"лапкапомощи()
{
  мур цыфорка вводноеЧисло мяу
  клацать(вводноеЧисло) мяу
  триппитроппа (вводноеЧисло >= 0)
  {
    мурлыкать(кринж) мяу
  }
  троппатриппа
  {
    мурлыкать(некринж) мяу
  } мяу
}";

        IReadOnlyList<double> output = ExecuteProgram(program, input);
        Assert.Single(output);
        Assert.Equal(expected, output[0]);
    }

    public static TheoryData<double, double> GetBoolTests()
    {
        return new TheoryData<double, double>
        {
            { 5, 1 },
            { -5, 0 },
            { 0, 1 },
            { 100, 1 },
            { -100, 0 },
        };
    }

    private static IReadOnlyList<double> ExecuteProgram(string program, params double[] inputs)
    {
        FakeEnvironment fakeEnv = new();
        foreach (double input in inputs)
        {
            fakeEnv.AddInput(input);
        }

        Context context = new Context();
        Interpreter interpreter = new Interpreter(context, fakeEnv);
        interpreter.Execute(program);
        return fakeEnv.GetOutput();
    }
}

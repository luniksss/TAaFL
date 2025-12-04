using System.Collections.Generic;
using System.Text;

static void Task1()
{
    string[] inputString = Console.ReadLine().Split('M');
    int numberOfM = inputString.Length - 1;
    int res = 0;
    int mathProgress = 1;
    while (numberOfM >= mathProgress)
    {
        numberOfM -= mathProgress;
        mathProgress++;
        res++;
    }
    Console.WriteLine(res);
}

static void Task2()
{
    string nString = Console.ReadLine();
    List<string> nStringList = new List<string>();
    int.TryParse(nString, out int n);
    for (int i = 0; i != n; i++)
    {
        string element = Console.ReadLine();
        nStringList.Add(element);
    }
    nStringList.Sort((a, b) => (b + a).CompareTo(a + b));
    string sb = "";
    for (int i = 0; i != nStringList.Count; i++)
    {
        sb += nStringList[i];
    }
    Console.WriteLine(sb);
}

string nString = Console.ReadLine();
int.TryParse(nString, out int n);
List<(int Cats, int Coast, int Number)> churches = new List<(int Cats, int Coast, int Number)>();
for (int i = 0; i < n; i++)
{
    string[] element = Console.ReadLine().Split();
    churches.Add((int.Parse(element[0]), int.Parse(element[1]), i + 1));
}
int bestTemple = -1;
long bestCoast = long.MaxValue;
int[] bestCats = new int[n];
for (int i = 0; i < n; i++)
{
    (int Cats, int Coast, int Number) target = churches[i];
    if (target.Coast == -1)
    {
        continue;
    }
    int maxOther = 0;
    for (int j = 0; j < n; j++)
    {
        if (j != i)
        {
            maxOther = Math.Max(maxOther, churches[j].Cats);
        }
    }
    int need = Math.Max(0, maxOther + 1 - target.Cats);
    long totalCoast = target.Coast + need;
    if (totalCoast < bestCoast)
    {
        bestCoast = totalCoast;
        bestTemple = i;
        for (int j = 0; j < n; j++)
        {
            bestCats[j] = churches[j].Cats;
        }
        bestCats[i] += need;
        if (need > 0)
        {
            int donor = churches.OrderByDescending(x => x.Cats).First(x => x.Number - 1 != i).Number - 1;
            bestCats[donor] -= need;
        }
    }
}
Console.WriteLine(bestCoast);
Console.WriteLine(bestTemple + 1);
Console.WriteLine(string.Join(" ", bestCats));
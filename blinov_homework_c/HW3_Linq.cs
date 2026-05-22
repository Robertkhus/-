namespace HW3Linq;

record Point(int X, int Y);

class Program
{
    static void Main()
    {
        Console.WriteLine("=== LQ1: Циклический сдвиг массива влево на K ===");
        int[] arr = { 1, 2, 3, 4, 5, 6, 7 };
        int k = 3;
        int[] shifted = arr.Skip(k % arr.Length).Concat(arr.Take(k % arr.Length)).ToArray();
        Console.WriteLine($"Исходный: [{string.Join(", ", arr)}]");
        Console.WriteLine($"Сдвиг на {k}: [{string.Join(", ", shifted)}]");

        Console.WriteLine("\n=== LQ2: 1-окрестность точек ===");
        var points = new[] { new Point(0, 0), new Point(0, 1) };
        int[] deltas = { -1, 0, 1 };
        var neighborhood = points
            .SelectMany(p => deltas.SelectMany(dx => deltas.Select(dy => new Point(p.X + dx, p.Y + dy))))
            .Distinct()
            .Except(points)
            .ToList();
        Console.WriteLine($"Точки: [{string.Join(", ", points)}]");
        Console.WriteLine($"Окрестность ({neighborhood.Count} точек): [{string.Join(", ", neighborhood)}]");

        Console.WriteLine("\n=== LQ3: Строки без повторений букв более 2 раз ===");
        var strings = new[] { "hello", "world", "abcdef", "aabbcc", "aaabbb", "mississippi" };
        var filtered = strings
            .Where(s => s.GroupBy(c => c).All(g => g.Count() <= 2))
            .ToList();
        Console.WriteLine($"Все: [{string.Join(", ", strings)}]");
        Console.WriteLine($"Подходящие: [{string.Join(", ", filtered)}]");

        Console.WriteLine("\n=== LQ4: Поиск слов с заменой одной буквы ===");
        var dictionary = new[] { "cat", "bat", "hat", "car", "cut", "can", "map", "mat" };
        string query = "cat";

        var index = BuildIndex(dictionary);
        var matches = FindWithOneDiff(query, index, dictionary[0].Length);
        Console.WriteLine($"Запрос: '{query}'");
        Console.WriteLine($"Совпадения: [{string.Join(", ", matches)}]");
    }

    static Dictionary<(int pos, char ch), List<string>> BuildIndex(IEnumerable<string> words)
    {
        return words
            .SelectMany(w => w.Select((c, i) => (key: (i, c), word: w)))
            .GroupBy(x => x.key, x => x.word)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    static IEnumerable<string> FindWithOneDiff(string query, Dictionary<(int, char), List<string>> index, int k)
    {
        return Enumerable.Range(0, k)
            .SelectMany(i =>
                Enumerable.Range(0, 26)
                    .Select(c => (char)('a' + c))
                    .Where(c => c != query[i])
                    .SelectMany(c => index.TryGetValue((i, c), out var list) ? list : Enumerable.Empty<string>())
                    .Where(w => w.Where((ch, idx) => idx != i && ch == query[idx]).Count() == k - 1))
            .Distinct();
    }
}

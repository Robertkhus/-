using System.Diagnostics;

namespace HW5;

class Program
{
    static void Main()
    {
        const string filePath = "bigdata.txt";
        const int bufferSize = 65536;

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Генерация файла...");
            using var sw = new StreamWriter(filePath);
            for (int i = 0; i < 50_000_000; i++)
                sw.WriteLine("Data line with some A symbols and other chars");
            Console.WriteLine("Файл создан.");
        }

        var sw2 = Stopwatch.StartNew();
        long count = 0;
        var buffer = new byte[bufferSize];

        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize))
        {
            int bytesRead;
            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                    if (buffer[i] == 65)
                        count++;
            }
        }

        sw2.Stop();

        Console.WriteLine($"Количество символов 'A': {count}");
        Console.WriteLine($"Время выполнения: {sw2.Elapsed.TotalSeconds:F2} сек");

        using var proc = Process.GetCurrentProcess();
        Console.WriteLine($"Потребление памяти: {proc.WorkingSet64 / 1024 / 1024} МБ");
    }
}

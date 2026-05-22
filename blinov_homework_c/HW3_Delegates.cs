using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace HW3;

delegate void LogHandler(string message);

class OrderProcessor
{
    public LogHandler? Logger;

    public void Process()
    {
        Logger?.Invoke("Заказ принят");
        Logger?.Invoke("Платеж прошел");
    }
}

class Employee
{
    public string Name { get; init; } = "";
    public decimal Salary { get; init; }
    public int Experience { get; init; }

    public override string ToString() => $"{Name}, зарплата: {Salary}, стаж: {Experience} лет";
}

static class EmployeeFilter
{
    public static List<Employee> FilterEmployees(List<Employee> employees, Predicate<Employee> predicate)
    {
        var result = new List<Employee>();
        foreach (var e in employees)
            if (predicate(e)) result.Add(e);
        return result;
    }
}

class User
{
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";
}

static class UserNotifications
{
    public static void SendEmail(User user) =>
        Console.WriteLine($"[Email] Отправлено письмо на {user.Email}");

    public static void SaveToDb(User user)
    {
        if (user.Name == "error") throw new Exception("Ошибка БД");
        Console.WriteLine($"[DB] Пользователь {user.Name} сохранен");
    }

    public static void UpdateStats(User user) =>
        Console.WriteLine($"[Stats] Счетчик обновлен для {user.Name}");

    public static void SafeInvoke(Action<User> chain, User user)
    {
        foreach (var handler in chain.GetInvocationList().Cast<Action<User>>())
        {
            try { handler(user); }
            catch (Exception ex) { Console.WriteLine($"[Ошибка] {ex.Message}"); }
        }
    }
}

static class Extensions
{
    public static void ForEachWithIndex<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        int i = 0;
        foreach (var item in source)
            action(item, i++);
    }
}

class SecuritySensor
{
    public event Action<string, DateTime>? OnAlert;

    public void Trigger(string message) => OnAlert?.Invoke(message, DateTime.Now);
}

class Siren
{
    public void OnAlert(string message, DateTime time) =>
        Console.WriteLine($"ВКЛЮЧЕНА СИРЕНА: {message}");
}

class SensorLogger
{
    public ObservableCollection<string> Logs { get; } = new();

    public void OnAlert(string message, DateTime time) =>
        Logs.Add($"[{time:HH:mm:ss}] {message}");
}

static class LogAnalyzer
{
    public static void AnalyzeLog<T>(IEnumerable<T> logs, Predicate<T> filter)
    {
        foreach (var log in logs)
            if (filter(log)) Console.WriteLine($"  [Анализ] {log}");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Уровень 1: Логирование ===");
        var processor = new OrderProcessor();
        processor.Logger += msg => { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(msg); Console.ResetColor(); };
        processor.Logger += msg => Console.WriteLine(msg);
        processor.Process();

        Console.WriteLine("\n=== Уровень 2: Фильтрация сотрудников ===");
        var employees = new List<Employee>
        {
            new() { Name = "Иван",   Salary = 60000, Experience = 7 },
            new() { Name = "Петр",   Salary = 40000, Experience = 3 },
            new() { Name = "Анна",   Salary = 75000, Experience = 6 },
            new() { Name = "Мария",  Salary = 45000, Experience = 2 },
            new() { Name = "Сергей", Salary = 55000, Experience = 8 },
        };
        Console.WriteLine("Зарплата > 50 000:");
        EmployeeFilter.FilterEmployees(employees, e => e.Salary > 50000).ForEach(e => Console.WriteLine("  " + e));
        Console.WriteLine("Стаж > 5 лет:");
        EmployeeFilter.FilterEmployees(employees, e => e.Experience > 5).ForEach(e => Console.WriteLine("  " + e));

        Console.WriteLine("\n=== Уровень 3: Цепочка уведомлений ===");
        Action<User> chain = UserNotifications.SendEmail;
        chain += UserNotifications.SaveToDb;
        chain += UserNotifications.UpdateStats;
        UserNotifications.SafeInvoke(chain, new User { Name = "error", Email = "error@test.com" });

        Console.WriteLine("\n=== Уровень 4: ForEachWithIndex ===");
        var names = new[] { "Иван", "Петр", "Анна" };
        names.ForEachWithIndex((name, i) => Console.WriteLine($"{i + 1}. {name}"));

        Console.WriteLine("\n=== Система безопасности склада ===");
        var sensor = new SecuritySensor();
        var siren = new Siren();
        var logger = new SensorLogger();

        logger.Logs.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                Console.WriteLine("Запись добавлена в БД");
        };

        sensor.OnAlert += siren.OnAlert;
        sensor.OnAlert += logger.OnAlert;

        sensor.Trigger("Движение обнаружено");
        sensor.Trigger("Критично: взлом двери");

        Console.WriteLine("\nАнализ логов (содержат 'Критично'):");
        LogAnalyzer.AnalyzeLog(logger.Logs, log => log.Contains("Критично"));
    }
}

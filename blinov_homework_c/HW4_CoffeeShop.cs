using System.Text.Json;
using System.Text.Json.Serialization;

namespace HW4;

class Ingredients
{
    [JsonPropertyName("water")] public int Water { get; set; }
    [JsonPropertyName("milk")] public int Milk { get; set; }
    [JsonPropertyName("beans")] public int Beans { get; set; }
}

class DrinkPrice
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("price")] public decimal Price { get; set; }
    [JsonPropertyName("waterCost")] public int WaterCost { get; set; }
    [JsonPropertyName("milkCost")] public int MilkCost { get; set; }
    [JsonPropertyName("beansCost")] public int BeansCost { get; set; }
}

class Config
{
    [JsonPropertyName("ingredients")] public Ingredients Ingredients { get; set; } = new();
    [JsonPropertyName("drinks")] public List<DrinkPrice> Drinks { get; set; } = new();
}

class CoffeeMachine
{
    private Config _config;
    private readonly string _configPath;
    private readonly string _historyPath;

    public CoffeeMachine(string configPath, string historyPath)
    {
        _configPath = configPath;
        _historyPath = historyPath;
        _config = LoadConfig();
    }

    private Config LoadConfig()
    {
        if (!File.Exists(_configPath))
        {
            var defaultConfig = new Config
            {
                Ingredients = new Ingredients { Water = 1000, Milk = 500, Beans = 300 },
                Drinks = new List<DrinkPrice>
                {
                    new() { Name = "Эспрессо",  Price = 100, WaterCost = 50,  MilkCost = 0,   BeansCost = 20 },
                    new() { Name = "Капучино",  Price = 150, WaterCost = 60,  MilkCost = 80,  BeansCost = 20 },
                    new() { Name = "Латте",     Price = 160, WaterCost = 60,  MilkCost = 120, BeansCost = 20 },
                    new() { Name = "Американо", Price = 120, WaterCost = 120, MilkCost = 0,   BeansCost = 20 },
                }
            };
            File.WriteAllText(_configPath, JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true }));
            return defaultConfig;
        }
        return JsonSerializer.Deserialize<Config>(File.ReadAllText(_configPath))!;
    }

    private void SaveConfig()
    {
        File.WriteAllText(_configPath, JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true }));
    }

    public void ShowMenu()
    {
        Console.WriteLine("\n=== Меню ===");
        foreach (var d in _config.Drinks)
            Console.WriteLine($"  {d.Name} — {d.Price} руб.");
        Console.WriteLine($"\nЗапасы: вода={_config.Ingredients.Water}мл, молоко={_config.Ingredients.Milk}мл, зерна={_config.Ingredients.Beans}г");
    }

    public bool Sell(string drinkName)
    {
        var drink = _config.Drinks.FirstOrDefault(d => d.Name.Equals(drinkName, StringComparison.OrdinalIgnoreCase));
        if (drink == null) { Console.WriteLine("Напиток не найден."); return false; }

        if (_config.Ingredients.Water < drink.WaterCost ||
            _config.Ingredients.Milk < drink.MilkCost ||
            _config.Ingredients.Beans < drink.BeansCost)
        {
            Console.WriteLine("Недостаточно ингредиентов.");
            return false;
        }

        _config.Ingredients.Water -= drink.WaterCost;
        _config.Ingredients.Milk -= drink.MilkCost;
        _config.Ingredients.Beans -= drink.BeansCost;
        SaveConfig();

        File.AppendAllText(_historyPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Продано: {drink.Name}, Цена: {drink.Price}\n");
        Console.WriteLine($"Приготовлено: {drink.Name}. Стоимость: {drink.Price} руб.");
        return true;
    }

    public void EndOfShift()
    {
        if (!File.Exists(_historyPath))
        {
            Console.WriteLine("История продаж пуста.");
            return;
        }

        var lines = File.ReadAllLines(_historyPath);
        var today = DateTime.Today.ToString("yyyy-MM-dd");
        var todaySales = lines.Where(l => l.StartsWith($"[{today}")).ToList();

        var summary = todaySales
            .Select(l => {
                var name = l.Split("Продано: ")[1].Split(", Цена: ")[0];
                var price = decimal.Parse(l.Split("Цена: ")[1]);
                return (name, price);
            })
            .GroupBy(x => x.name)
            .Select(g => new { Drink = g.Key, Count = g.Count(), Total = g.Sum(x => x.price) })
            .ToList();

        decimal revenue = summary.Sum(s => s.Total);
        var report = new
        {
            Date = today,
            TotalRevenue = revenue,
            Sales = summary
        };

        string reportPath = $"report_{DateTime.Today:yyyy_MM_dd}.json";
        File.WriteAllText(reportPath, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"\n=== Отчет за {today} ===");
        Console.WriteLine($"Итоговая выручка: {revenue} руб.");
        Console.WriteLine($"Отчет сохранен: {reportPath}");
    }
}

class Program
{
    static void Main()
    {
        var machine = new CoffeeMachine("config.json", "sales_history.txt");
        machine.ShowMenu();

        machine.Sell("Капучино");
        machine.Sell("Эспрессо");
        machine.Sell("Латте");

        machine.EndOfShift();
    }
}

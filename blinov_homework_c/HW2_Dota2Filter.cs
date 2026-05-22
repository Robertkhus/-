namespace HW2;

public enum Role { Carry, Support, Offlane, Midlane, Jungler }

public class Hero
{
    public string Name { get; init; } = "";
    public Role Role { get; init; }
    public int WinRate { get; init; }
    public double Kda { get; init; }
    public string PrimaryAttribute { get; init; } = "";
}

public static class HeroFilter
{
    public static IEnumerable<Hero> FilterByRole(IEnumerable<Hero> heroes, Role role)
    {
        foreach (var hero in heroes)
            if (hero.Role == role)
                yield return hero;
    }

    public static IEnumerable<Hero> FilterByMinWinRate(IEnumerable<Hero> heroes, int minWinRate)
    {
        foreach (var hero in heroes)
            if (hero.WinRate >= minWinRate)
                yield return hero;
    }

    public static IEnumerable<Hero> FilterByMinKda(IEnumerable<Hero> heroes, double minKda)
    {
        foreach (var hero in heroes)
            if (hero.Kda >= minKda)
                yield return hero;
    }

    public static IEnumerable<Hero> FilterByAttribute(IEnumerable<Hero> heroes, string attribute)
    {
        foreach (var hero in heroes)
            if (hero.PrimaryAttribute.Equals(attribute, StringComparison.OrdinalIgnoreCase))
                yield return hero;
    }

    public static IEnumerable<Hero> FilterByRoleAndWinRate(IEnumerable<Hero> heroes, Role role, int minWinRate)
    {
        foreach (var hero in FilterByRole(heroes, role))
            if (hero.WinRate >= minWinRate)
                yield return hero;
    }
}

class Program
{
    static void Main()
    {
        var heroes = new List<Hero>
        {
            new() { Name = "Juggernaut",  Role = Role.Carry,   WinRate = 52, Kda = 3.1, PrimaryAttribute = "Agility" },
            new() { Name = "Crystal Maiden", Role = Role.Support, WinRate = 50, Kda = 2.5, PrimaryAttribute = "Intelligence" },
            new() { Name = "Pudge",       Role = Role.Support, WinRate = 48, Kda = 2.0, PrimaryAttribute = "Strength" },
            new() { Name = "Invoker",     Role = Role.Midlane, WinRate = 47, Kda = 4.2, PrimaryAttribute = "Intelligence" },
            new() { Name = "Axe",         Role = Role.Offlane, WinRate = 51, Kda = 2.8, PrimaryAttribute = "Strength" },
            new() { Name = "Anti-Mage",   Role = Role.Carry,   WinRate = 53, Kda = 3.5, PrimaryAttribute = "Agility" },
            new() { Name = "Tinker",      Role = Role.Midlane, WinRate = 46, Kda = 3.9, PrimaryAttribute = "Intelligence" },
            new() { Name = "Sniper",      Role = Role.Carry,   WinRate = 50, Kda = 3.0, PrimaryAttribute = "Agility" },
        };

        Console.WriteLine("=== Carry-герои ===");
        foreach (var h in HeroFilter.FilterByRole(heroes, Role.Carry))
            Console.WriteLine($"  {h.Name}");

        Console.WriteLine("\n=== Win Rate >= 51% ===");
        foreach (var h in HeroFilter.FilterByMinWinRate(heroes, 51))
            Console.WriteLine($"  {h.Name} ({h.WinRate}%)");

        Console.WriteLine("\n=== KDA >= 3.5 ===");
        foreach (var h in HeroFilter.FilterByMinKda(heroes, 3.5))
            Console.WriteLine($"  {h.Name} (KDA {h.Kda})");

        Console.WriteLine("\n=== Intelligence-герои ===");
        foreach (var h in HeroFilter.FilterByAttribute(heroes, "Intelligence"))
            Console.WriteLine($"  {h.Name}");

        Console.WriteLine("\n=== Carry + WinRate >= 52% ===");
        foreach (var h in HeroFilter.FilterByRoleAndWinRate(heroes, Role.Carry, 52))
            Console.WriteLine($"  {h.Name} ({h.WinRate}%)");
    }
}

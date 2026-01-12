using madake1.Content.Items.Weapons;
using Terraria.ModLoader;

public static class ModItemID
{
    public static int Jinxi => ModContent.ItemType<Jinxi>();
    public static int Changli => ModContent.ItemType<Changli>();


    public static int GetID(string name)
    {
        return name switch
        {
            "Jinxi" => Jinxi,
            "Changli" => Changli,

            _ => 0
        };
    }
}

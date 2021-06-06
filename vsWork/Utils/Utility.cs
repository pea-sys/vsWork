using System.Text.Json;

public static class Utility
{
    public static T DeepCopy<T>(this T src)
    {
        string jsonStr = JsonSerializer.Serialize<T>(src);

        return (T)JsonSerializer.Deserialize<T>(jsonStr);
    }
}


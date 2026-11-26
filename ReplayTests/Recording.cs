using System.Text.Json;

namespace ReplayTests;

// One captured request and the response the old system gave for it.
public sealed record Recording(string Name, string Method, string Path, int Status, string ContentType, string Body)
{
    public static IEnumerable<object[]> LoadAll()
    {
        var dir = System.IO.Path.Combine(AppContext.BaseDirectory, "recordings");
        foreach (var file in Directory.GetFiles(dir, "*.json").OrderBy(f => f))
        {
            var rec = JsonSerializer.Deserialize<Recording>(File.ReadAllText(file),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            yield return new object[] { rec };
        }
    }

    public override string ToString() => Name;
}

using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ReplayTests;

public class ReplayTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ReplayTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    [Theory]
    [MemberData(nameof(Recording.LoadAll), MemberType = typeof(Recording))]
    public async Task Response_matches_what_the_old_system_returned(Recording rec)
    {
        var response = await _client.SendAsync(new HttpRequestMessage(new HttpMethod(rec.Method), rec.Path));
        var body = (await response.Content.ReadAsStringAsync()).Replace("\r\n", "\n");

        Assert.Equal(rec.Status, (int)response.StatusCode);
        Assert.Equal(rec.ContentType, response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(rec.Body, body);
    }
}

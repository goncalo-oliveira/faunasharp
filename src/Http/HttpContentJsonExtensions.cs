using System.Text.Json;

internal static class HttpContentJsonExtensions
{
    public static async Task<T?> ReadAsJsonAsync<T>( this HttpContent content, JsonSerializerOptions? options = null )
    {
        using ( var stream = await content.ReadAsStreamAsync() )
        {
            return await JsonSerializer.DeserializeAsync<T>( stream, options );
        }
    }
}

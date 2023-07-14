using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaunaDB;

public static class FaunaResponsePaginationExtensions
{
    /// <summary>
    /// Deserializes the response data as a <see cref="Page"/> instance.
    /// </summary>
    public static Page? ToPage( this FaunaResponse response )
    {
        if ( response.Data == null )
        {
            throw new ArgumentNullException( nameof( response.Data ) );
        }

        var page = JsonSerializer.Deserialize<PrivatePage>( response.Data );

        if ( page == null )
        {
            return ( null );
        }

        return ( new Page( page.Data, page.After ) );
    }

    /// <summary>
    /// Deserializes the response data as a <see cref="Page{T}"/> instance.
    /// </summary>
    public static object? ToPage( this FaunaResponse response, Type dataType, JsonSerializerOptions? jsonOptions = null )
    {
        if ( response.Data == null )
        {
            throw new ArgumentNullException( nameof( response.Data ) );
        }

        // first we need to get the binary page
        var page = response.ToPage();

        if ( page == null )
        {
            return default;
        }

        // deserialize the data as array
        var data = JsonSerializer.Deserialize(
            page.Data,
            dataType.MakeArrayType(),
            jsonOptions
        );

        return Activator.CreateInstance(
            typeof( Page<> ).MakeGenericType( dataType ),
            System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.DeclaredOnly
                | System.Reflection.BindingFlags.Instance,
            null,
            new object?[] { data, page.After },
            null
        );
    }

    /// <summary>
    /// Deserializes the response data as a <see cref="Page{T}"/> instance.
    /// </summary>
    public static Page<T>? ToPage<T>( this FaunaResponse response, JsonSerializerOptions? jsonOptions = null )
        => (Page<T>?)response.ToPage( typeof( T ), jsonOptions );

    private class PrivatePage
    {
        [JsonPropertyName( "data" )]
        public byte[] Data { get; set; } = Array.Empty<byte>();

        [JsonPropertyName( "after" )]
        public string? After { get; set; }
    }
}

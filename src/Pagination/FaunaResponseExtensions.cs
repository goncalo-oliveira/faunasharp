using System.Text;
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

        // the contents are inside the data property
        JsonElement obj = JsonSerializer.Deserialize<JsonElement>( response.Data );

        if ( !obj.TryGetProperty( "data", out JsonElement data ) )
        {
            // no data property found
            return default;
        }

        byte[]? bytes = Array.Empty<byte>();
        //byte[]? bytes = data.SerializeToUtf8Bytes();
        if ( data.ValueKind == JsonValueKind.String )
        {
            bytes = Convert.FromBase64String( data.GetString()! );
        }
        else
        {
            bytes = data.SerializeToUtf8Bytes();
        }

        // retrieve after token
        string? after = null;
        if ( obj.TryGetProperty( "after", out JsonElement afterElement ))
        {
            after = afterElement.GetString();
        }

        return new Page( bytes, after );
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

using System.Text.Json;

internal static class JsonElementExtensions
{
    public static byte[] SerializeToUtf8Bytes( this JsonElement source, JsonSerializerOptions? jsonOptions = null )
    {
        if ( source.IsDocumentArray() )
        {
            // an array of documents contains a single "data" property
            return JsonSerializer.SerializeToUtf8Bytes( source.GetProperty( "data" ), jsonOptions );
        }

        if ( source.IsPage() )
        {
            // a page contains a "data" property and an "after" property
            var data = JsonSerializer.SerializeToUtf8Bytes( source.GetProperty( "data" ), jsonOptions );
            var after = source.GetProperty( "after" ).GetString();

            var page = new Dictionary<string, object?>
            {
                { "data", data },
                { "after", after }
            };

            return JsonSerializer.SerializeToUtf8Bytes( page, jsonOptions );
        }

        // otherwise it's a single document or a reference error (or a page)
        return JsonSerializer.SerializeToUtf8Bytes( source, jsonOptions );
    }

    public static bool IsPage( this JsonElement source )
        => ( source.ValueKind == JsonValueKind.Object )
        &&
        ( source.EnumerateObject().Count() == 2 )
        &&
        ( source.TryGetProperty( "data", out _ ) )
        &&
        ( source.TryGetProperty( "after", out _ ) );

        //=> ( dictionary.Count == 2 ) && dictionary.ContainsKey( "data" ) && dictionary.ContainsKey( "after" );

    public static bool IsDocumentArray( this JsonElement source )
        => ( source.ValueKind == JsonValueKind.Array )
        &&
        ( source.GetArrayLength() == 1 )
        &&
        ( source[0].TryGetProperty( "data", out _ ) );
}

using System.Text.Json;

internal static class DictionaryJsonExtensions
{
    public static byte[] SerializeToUtf8Bytes( this Dictionary<string, object> dictionary, JsonSerializerOptions? jsonOptions = null )
    {
        if ( dictionary.IsDocumentArray() )
        {
            // an array of documents contains a single "data" property
            return JsonSerializer.SerializeToUtf8Bytes( dictionary["data"], jsonOptions );
        }

        if ( dictionary.IsPage() )
        {
            // a page contains a "data" property and an "after" property
            var data = JsonSerializer.SerializeToUtf8Bytes( dictionary["data"], jsonOptions );
            var after = dictionary["after"].ToString();

            var page = new Dictionary<string, object?>
            {
                { "data", data },
                { "after", after }
            };

            return JsonSerializer.SerializeToUtf8Bytes( page, jsonOptions );
        }

        // otherwise it's a single document or a reference error (or a page)
        return JsonSerializer.SerializeToUtf8Bytes( dictionary, jsonOptions );
    }

    public static bool IsPage( this Dictionary<string, object> dictionary )
        => ( dictionary.Count == 2 ) && dictionary.ContainsKey( "data" ) && dictionary.ContainsKey( "after" );

    public static bool IsDocumentArray( this Dictionary<string, object> dictionary )
        => ( dictionary.Count == 1 ) && dictionary.ContainsKey( "data" );
}

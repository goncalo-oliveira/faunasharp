using System.Text.Json;

internal static class DictionaryJsonExtensions
{
    public static byte[] SerializeToUtf8Bytes( this Dictionary<string, object> dictionary, JsonSerializerOptions? jsonOptions = null )
    {
        if ( ( dictionary.Count == 1 ) && dictionary.ContainsKey( "data" ) )
        {
            // an array of documents contains a single "data" property
            return JsonSerializer.SerializeToUtf8Bytes( dictionary["data"], jsonOptions );
        }

        // otherwise it's a single document or a reference error
        return JsonSerializer.SerializeToUtf8Bytes( dictionary, jsonOptions );
    }
}

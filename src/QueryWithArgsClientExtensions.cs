using System.Text.Json;
using FaunaDB;

namespace FaunaDB;

public static class ClientQueryWithJsonArgs
{
    public static Task<FaunaResponse> QueryAsync( this FaunaClient client, object arg1, Func<string, string> query, JsonSerializerOptions? jsonOptions = null )
    {
        var jsonArg1 = JsonSerializer.Serialize( arg1, jsonOptions );

        return client.QueryAsync( query( jsonArg1 ) );
    }

    public static Task<FaunaResponse> QueryAsync( this FaunaClient client, object arg1, object arg2, Func<string, string, string> query, JsonSerializerOptions? jsonOptions = null )
    {
        var jsonArg1 = JsonSerializer.Serialize( arg1, jsonOptions );
        var jsonArg2 = JsonSerializer.Serialize( arg2, jsonOptions );

        return client.QueryAsync( query( jsonArg1, jsonArg2 ) );
    }

    public static Task<FaunaResponse> QueryAsync( this FaunaClient client, object arg1, object arg2, object arg3, Func<string, string, string, string> query, JsonSerializerOptions? jsonOptions = null )
    {
        var jsonArg1 = JsonSerializer.Serialize( arg1, jsonOptions );
        var jsonArg2 = JsonSerializer.Serialize( arg2, jsonOptions );
        var jsonArg3 = JsonSerializer.Serialize( arg3, jsonOptions );

        return client.QueryAsync( query( jsonArg1, jsonArg2, jsonArg3 ) );
    }
}

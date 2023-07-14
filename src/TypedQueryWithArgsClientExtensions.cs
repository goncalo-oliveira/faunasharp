using System.Text.Json;
using FaunaDB;

namespace FaunaDB;

public static class ClientTypedQueryWithJsonArgs
{
    public static Task<T?> QueryAsync<T>( this FaunaClient client, Func<string, string> query, object arg1, JsonSerializerOptions? jsonOptions = null )
    {
        var jsonArg1 = JsonSerializer.Serialize( arg1, jsonOptions );

        return client.QueryAsync<T>( query( jsonArg1 ) );
    }

    public static Task<T?> QueryAsync<T>( this FaunaClient client, Func<string, string, string> query, object arg1, object arg2, JsonSerializerOptions? jsonOptions = null )
    {
        var jsonArg1 = JsonSerializer.Serialize( arg1, jsonOptions );
        var jsonArg2 = JsonSerializer.Serialize( arg2, jsonOptions );

        return client.QueryAsync<T>( query( jsonArg1, jsonArg2 ) );
    }

    public static Task<T?> QueryAsync<T>( this FaunaClient client, Func<string, string, string, string> query, object arg1, object arg2, object arg3, JsonSerializerOptions? jsonOptions = null )
    {
        var jsonArg1 = JsonSerializer.Serialize( arg1, jsonOptions );
        var jsonArg2 = JsonSerializer.Serialize( arg2, jsonOptions );
        var jsonArg3 = JsonSerializer.Serialize( arg3, jsonOptions );

        return client.QueryAsync<T>( query( jsonArg1, jsonArg2, jsonArg3 ) );
    }
}

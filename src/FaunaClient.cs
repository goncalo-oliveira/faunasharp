using System.Net;
using System.Text;
using System.Text.Json;

namespace FaunaDB;

/// <summary>
/// A minimal FaunaDB client instance
/// </summary>
public sealed class FaunaClient
{
    private readonly string url = "https://db.fauna.com";
    private readonly HttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="FaunaClient"/> class.
    /// <param name="httpClient">The <see cref="HttpClient"/> instance to use for requests</param>
    /// </summary>
    public FaunaClient( HttpClient httpClient )
    {
        this.httpClient = httpClient;
    }

    /// <summary>
    /// Executes a query written in FQL over the FaunaDB API.
    /// <returns>Returns a generic <see cref="FaunaResponse"/> instance</returns>
    /// </summary>
    public async Task<FaunaResponse> QueryAsync( string query )
    {
        var request = new HttpRequestMessage( HttpMethod.Post, $"{url}/query/1" )
        {
            Content = new StringContent(
                JsonSerializer.Serialize( new { query } ),
                Encoding.UTF8, "application/json"
            ),
            Headers =
            {
                { "Accept", "application/json" },
                { "Accept-Encoding", "gzip, deflate, br" },
                { "User-Agent", "faunasharp/0.1" },
                { "X-Format", "simple" }
            },
            Version = HttpVersion.Version20,
            VersionPolicy = HttpVersionPolicy.RequestVersionExact
        };

        using ( var response = await httpClient.SendAsync( request ) )
        {
            response.EnsureSuccessStatusCode();

            var queryResponse = await response.Content.ReadAsJsonAsync<QueryResponse>();

            if ( queryResponse == null )
            {
                throw new FormatException( "Failed to deserialize response" );
            }

            /*
            We don't want to make assumptions on the data returned
            so we just serialize the data back as a byte array
            This handles scenarios where the data is a collection of
            documents or a single document.
            */
            byte[] data = queryResponse.Data?.SerializeToUtf8Bytes()
                ?? Array.Empty<byte>();

            // return a generic response with the binary data
            return new FaunaResponse
            {
                Data = data,
                Summary = queryResponse.Summary,
                Timestamp = DateTime.UnixEpoch.AddTicks( queryResponse.Timestamp * 10 ),
                Stats = ( queryResponse.Stats != null )
                    ? new FaunaStats
                    {
                        ComputeOps = queryResponse.Stats.ComputeOps,
                        ReadOps = queryResponse.Stats.ReadOps,
                        WriteOps = queryResponse.Stats.WriteOps,
                        QueryTime = queryResponse.Stats.QueryTime,
                        ContentionRetries = queryResponse.Stats.ContentionRetries,
                        StorageBytesRead = queryResponse.Stats.StorageBytesRead,
                        StorageBytesWrite = queryResponse.Stats.StorageBytesWrite
                    }
                    : null,
                SchemaVersion = queryResponse.SchemaVersion
            };
        }
    }

    /// <summary>
    /// Executes a query written in FQL over the FaunaDB API and deserializes the response data to the specified type.
    /// <returns>Returns the deserialized response data</returns>
    /// </summary>
    public async Task<T?> QueryAsync<T>( string query, JsonSerializerOptions? options = null )
    {
        var response = await QueryAsync( query );

        if ( !response.Data.Any() )
        {
            return default!;
        }

        return JsonSerializer.Deserialize<T>( response.Data, options );
    }
}

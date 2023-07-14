namespace FaunaDB;

/// <summary>
/// A generic query response from FaunaDB
/// </summary>
public sealed class FaunaResponse
{
    /// <summary>
    /// The binary contents of the response data
    /// </summary>
    public byte[] Data { get; set; } = Array.Empty<byte>();

    public string? Summary { get; set; }

    /// <summary>
    /// The transaction timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }
    public FaunaStats? Stats { get; set; }
    public long SchemaVersion { get; set; }

    public string ToJson( System.Text.Json.JsonSerializerOptions? jsonOptions = null ) => System.Text.Json.JsonSerializer.Serialize(
        System.Text.Json.JsonSerializer.Deserialize<object>( Data ),
        jsonOptions ?? new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
        }
    );
}

public sealed class FaunaStats
{
    public long ComputeOps { get; set; }
    public long ReadOps { get; set; }
    public long WriteOps { get; set; }

    /// <summary>
    /// The query time in milliseconds
    /// </summary>
    public int QueryTime { get; set; }
    public int ContentionRetries { get; set; }
    public int StorageBytesRead { get; set; }
    public int StorageBytesWrite { get; set; }
}

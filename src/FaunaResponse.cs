namespace FaunaDB;

/// <summary>
/// A generic query response from FaunaDB
/// </summary>
public sealed class FaunaResponse
{
    /// <summary>
    /// The binary contents of the response data
    /// </summary>
    public byte[]? Data { get; set; }

    /// <summary>
    /// A readable summary of any warnings emmited by the query
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// The last transaction timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Statistics on query cost and performance
    public QueryStats? Stats { get; set; }

    public long SchemaVersion { get; set; }

    /// <summary>
    /// The error returned from FaunaDB.
    /// </summary>
    public QueryError? Error { get; set; }

    /// <summary>
    /// Gets if the response is a failure
    /// </summary>
    public bool IsFailure() => ( Error != null );

    /// <summary>
    /// Throws a FaunaQueryException if the response is a failure
    /// </summary>
    public void ThrowIfFailure()
    {
        if ( IsFailure() )
        {
            throw new FaunaQueryException( this );
        }
    }

    /// <summary>
    /// Serializes the response data to a JSON string
    /// </summary>
    public string ToJson( System.Text.Json.JsonSerializerOptions? jsonOptions = null ) => System.Text.Json.JsonSerializer.Serialize(
        System.Text.Json.JsonSerializer.Deserialize<object>( Data ),
        jsonOptions ?? new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
        }
    );
}

/// <summary>
/// Statistics on query cost and performance
public sealed class QueryStats
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

/// <summary>
/// Represents an error returned from FaunaDB.
/// </summary>
public sealed class QueryError
{
    public string? Code { get; set; }
    public string? Message { get; set; }
    public IEnumerable<ConstraintFailure>? ConstraintFailures { get; set; }
}

public sealed class ConstraintFailure
{
    public string? Message { get; set; }
    public string? Name { get; set; }
    public string[][]? Paths { get; set; }
}

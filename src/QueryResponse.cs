using System.Text.Json.Serialization;

internal sealed class QueryResponse
{
    [JsonPropertyName( "data" )]
    public Dictionary<string, object>? Data { get; set; }

    [JsonPropertyName( "summary" )]
    public string? Summary { get; set; }

    [JsonPropertyName( "txn_ts" )]
    public long Timestamp { get; set; }

    [JsonPropertyName( "stats" )]
    public QueryStats? Stats { get; set; }

    [JsonPropertyName( "schema_version" )]
    public long SchemaVersion { get; set; }

    [JsonPropertyName( "error" )]
    public QueryError? Error { get; set; }

    internal sealed class QueryStats
    {
        [JsonPropertyName( "compute_ops" )]
        public long ComputeOps { get; set; }

        [JsonPropertyName( "read_ops" )]
        public long ReadOps { get; set; }

        [JsonPropertyName( "write_ops" )]
        public long WriteOps { get; set; }

        [JsonPropertyName( "query_time_ms" )]
        public int QueryTime { get; set; }

        [JsonPropertyName( "contention_retries" )]
        public int ContentionRetries { get; set; }

        [JsonPropertyName( "storage_bytes_read" )]
        public int StorageBytesRead { get; set; }

        [JsonPropertyName( "storage_bytes_write" )]
        public int StorageBytesWrite { get; set; }
    }

    public sealed class QueryError
    {
        [JsonPropertyName( "code" )]
        public string? Code { get; set; }

        [JsonPropertyName( "message" )]
        public string? Message { get; set; }

        [JsonPropertyName( "constraint_failures" )]
        public IEnumerable<ConstraintFailure>? ConstraintFailures { get; set; }
    }

    public sealed class ConstraintFailure
    {
        [JsonPropertyName( "message" )]
        public string? Message { get; set; }

        [JsonPropertyName( "name" )]
        public string? Name { get; set; }

        [JsonPropertyName( "paths" )]
        public string[][]? Paths { get; set; }
    }
}

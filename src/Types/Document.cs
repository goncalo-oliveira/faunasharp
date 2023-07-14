using System.Text.Json.Serialization;

namespace FaunaDB;

/// <summary>
/// An abstract class that represents a FaunaDB document.
/// </summary>
public abstract class Document
{
    [JsonPropertyName( "id" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Id { get; set; }

    [JsonPropertyName( "coll" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Collection { get; set; }

    [JsonPropertyName( "ts" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public DateTime? Timestamp { get; set; }
}

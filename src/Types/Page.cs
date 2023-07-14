namespace FaunaDB;

/// <summary>
/// Represents a page of data returned from FaunaDB.
/// </summary>
public sealed class Page
{
    internal Page( byte[] data, string? after )
    {
        Data = data;
        After = after;
    }

    /// <summary>
    /// The binary contents of the response data
    /// </summary>
    public byte[] Data { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// The cursor to use for the next page of data
    /// </summary>
    public string? After { get; set; }
}

/// <summary>
/// Represents a page of data returned from FaunaDB.
/// </summary>
public sealed class Page<T>
{
    internal Page( T[] data, string? after )
    {
        Data = data;
        After = after;
    }

    /// <summary>
    /// The typed contents of the response data
    /// </summary>
    public IEnumerable<T> Data { get; set; } = Array.Empty<T>();

    /// <summary>
    /// The cursor to use for the next page of data
    /// </summary>
    public string? After { get; set; }
}

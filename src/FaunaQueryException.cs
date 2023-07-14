namespace FaunaDB;

/// <summary>
/// Represents an error returned from FaunaDB.
/// </summary>
public sealed class FaunaQueryException : Exception
{
    internal FaunaQueryException( FaunaResponse response )
    : base( response.Summary ?? response.Error?.Message )
    {
        Error = response.Error;
    }

    /// <summary>
    /// The error returned from FaunaDB.
    /// </summary>
    public QueryError? Error { get; }
}

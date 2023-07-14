public struct JsonString
{
    private readonly string value;

    internal JsonString( string json )
    {
        value = json;
    }

    public override string ToString() => value;
}

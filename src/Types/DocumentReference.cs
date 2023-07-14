using System.Text.Json.Serialization;
using FaunaDB.Serialization;

namespace FaunaDB;

[JsonConverter( typeof( DocumentReferenceJsonConverter ))]
public sealed class DocumentReference
{
    public string? Id { get; set; }
    public string? Collection { get; set; }

    public override string ToString()
        => $"{Collection}/{Id})";
}

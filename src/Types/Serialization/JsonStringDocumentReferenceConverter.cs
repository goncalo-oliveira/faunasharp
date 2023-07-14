using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaunaDB.Serialization;

public sealed class JsonStringDocumentReferenceConverter : JsonConverter<DocumentReference>
{
    public override DocumentReference? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        if ( reader.TokenType != JsonTokenType.String )
        {
            throw new JsonException();
        }

        var value = reader.GetString();

        if ( string.IsNullOrEmpty( value ) )
        {
            return ( null );
        }

        // plain references are in the format of "collection/id"
        var parts = value.Split( '/' );

        if ( parts.Length != 2 )
        {
            throw new JsonException();
        }

        return ( new DocumentReference { Collection = parts[0], Id = parts[1] } );
    }

    public override void Write( Utf8JsonWriter writer, DocumentReference value, JsonSerializerOptions options )
    {
        if ( value == null )
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue( $"{value.Collection}/{value.Id}" );
        }
    }
}

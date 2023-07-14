using System.Text.Json;
using System.Text.Json.Serialization;

namespace FaunaDB.Serialization;

public sealed class DocumentReferenceJsonConverter : JsonConverter<DocumentReference>
{
    public override DocumentReference? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        if ( reader.TokenType != JsonTokenType.StartObject )
        {
            throw new JsonException();
        }

        var startDepth = reader.CurrentDepth;

        reader.Read();
        if ( reader.GetString() != "@ref" )
        {
            throw new JsonException();
        }

        reader.Read();

        if ( reader.TokenType != JsonTokenType.StartObject )
        {
            throw new JsonException();
        }

        var docReference = new DocumentReference();

        var propertyName = string.Empty;
        while ( reader.Read() )
        {
            if ( ( reader.TokenType == JsonTokenType.EndObject ) && ( reader.CurrentDepth == startDepth ) )
            {
                break;
            }

            if ( reader.TokenType == JsonTokenType.PropertyName )
            {
                propertyName = reader.GetString() ?? string.Empty;
            }
            else if ( reader.TokenType == JsonTokenType.String )
            {
                switch ( propertyName )
                {
                    case "id":
                        docReference.Id = reader.GetString();
                        break;
                    case "coll":
                        docReference.Collection = reader.GetString();
                        break;
                }
            }
        }

        return ( docReference );
    }

    public override void Write( Utf8JsonWriter writer, DocumentReference value, JsonSerializerOptions options )
    {
        writer.WriteStartObject();

        writer.WritePropertyName( "@ref" );
        writer.WriteStartObject();

        writer.WriteString( "id", value.Id );
        writer.WriteString( "coll", value.Collection );
        writer.WriteEndObject();

        writer.WriteEndObject();
    }
}

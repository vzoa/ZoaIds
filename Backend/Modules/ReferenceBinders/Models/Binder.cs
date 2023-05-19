using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZoaIdsBackend.Modules.ReferenceBinders.Models;

public interface INode
{
    public string Path { get; set; }
}

public class Binder : INode
{
    public string Path { get; set; }
    public ICollection<INode> Children { get; set; } = new List<INode>();

    public Binder(string path)
    {
        Path = path;
    }
}

public class Document : INode
{
    public string Path { get; set; }

    public Document(string path)
    {
        Path = path;
    }
}

public class BinderConverter : JsonConverter<Binder>
{
    public override Binder? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Binder value, JsonSerializerOptions options)
    {
        var split = value.Path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });

        writer.WriteStartObject();
        writer.WriteString("name", split[^1]);
        writer.WriteString("path", value.Path);
        writer.WriteString("type", "binder");

        writer.WritePropertyName("children");
        writer.WriteStartArray();
        foreach (var child in value.Children)
        {
            if (child is Binder binder)
            {
                JsonSerializer.Serialize(writer, binder, options);
            }
            else if (child is Document document)
            {
                JsonSerializer.Serialize(writer, document, options);
            }
            else
            {
                throw new JsonException();
            }
        }
        writer.WriteEndArray();       
        writer.WriteEndObject();
    }
}

public class DocumentConverter: JsonConverter<Document>
{
    public override Document? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Document value, JsonSerializerOptions options)
    {
        var split = value.Path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });

        writer.WriteStartObject();
        writer.WriteString("name", split[^1]);
        writer.WriteString("path", value.Path);
        writer.WriteString("type", "document");
        writer.WriteEndObject();
    }
}
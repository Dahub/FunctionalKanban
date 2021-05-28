[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("FunctionalKanban.Infrastructure.SqlServer.Test")]
namespace FunctionalKanban.Infrastructure.SqlServer
{
    using System.IO;
    using System.Text.Json;
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    internal static class BsonHelper
    {
        public static byte[] ToBson<T>(T obj) where T: notnull, Event
        {
            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms);
            JsonSerializer.Serialize(writer, obj);
            return ms.ToArray();
        }

        public static Option<T> FromBson<T>(byte[] objBytes) where T : notnull, Event
        {
            var result = JsonSerializer.Deserialize<T>(objBytes);
            return result == null
                ? None
                : Some(result);
        }
    }
}

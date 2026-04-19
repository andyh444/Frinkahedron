using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Template
{
    public sealed class TransformTemplate
    {
        public Vector3 Translation { get; set; }

        public Vector3 RotationEulerAngles { get; set; }

        public Vector3 Scale { get; set; } = Vector3.One;

        public Matrix4x4 ToMatrix()
        {
            Quaternion rotation = Quaternion.CreateFromYawPitchRoll(
                RotationEulerAngles.Y,
                RotationEulerAngles.X,
                RotationEulerAngles.Z);
            return Matrix4x4.CreateScale(Scale)
                * Matrix4x4.CreateFromQuaternion(rotation)
                * Matrix4x4.CreateTranslation(Translation);
        }
    }

    public sealed class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return StringToVector3(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(Vector3ToString(value));
        }

        private string Vector3ToString(Vector3 v)
        {
            return $"{v.X:#0.0#####},{v.Y:#0.0######},{v.Z:#0.0######}";
        }

        private Vector3 StringToVector3(string s)
        {
            string[] split = s.Split(',');
            return new Vector3(
                float.Parse(split[0]),
                float.Parse(split[1]),
                float.Parse(split[2]));
        }
    }

    internal sealed class TransformTemplateConverter : JsonConverter<TransformTemplate>
    {
        public override TransformTemplate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, TransformTemplate value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Translation", Vector3ToString(value.Translation));
            writer.WriteString("RotationEulerAngles", Vector3ToString(value.RotationEulerAngles));
            writer.WriteString("Scale", Vector3ToString(value.Scale));
            writer.WriteEndObject();
        }

        private string Vector3ToString(Vector3 v)
        {
            return $"{v.X:#0.0#},{v.Y:#0.0#},{v.Z:#0.0#}";
        }
    }
}

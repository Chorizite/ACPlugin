using Chorizite.Common.Enums;
using AC.API;
using AC.Lib;
using AC.Lib.Screens;
using System.Text.Json.Serialization;

namespace AC {
    [JsonSourceGenerationOptions(WriteIndented = true, AllowTrailingCommas = true, UseStringEnumConverter = true)]
    [JsonSerializable(typeof(PluginState))]
    [JsonSerializable(typeof(GameScreen))]
    [JsonSerializable(typeof(Game))]
    [JsonSerializable(typeof(Character))]
    [JsonSerializable(typeof(AttributeId), TypeInfoPropertyName = "AttributeIdCommon")]
    [JsonSerializable(typeof(SkillFormula), TypeInfoPropertyName = "SkillFormulaCommon")]
    internal partial class SourceGenerationContext : JsonSerializerContext {
    }
}

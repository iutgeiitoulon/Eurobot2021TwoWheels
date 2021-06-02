using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Globalization;

namespace WorldMap
{
    public class WorldStateMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("teamName")]
        public string TeamName { get; set; }

        [JsonProperty("intention")]
        public string Intention { get; set; }

        [JsonProperty("robots")]
        public List<Robot> Robots { get; set; }

        [JsonProperty("balls")]
        public List<Ball> Balls { get; set; }

        [JsonProperty("obstacles")]
        public List<Obstacle> Obstacles { get; set; }

        [JsonProperty("ageMs")]
        public long AgeMs { get; set; }
        public static WorldStateMessage FromJson(string json) => JsonConvert.DeserializeObject<WorldStateMessage>(json, Converter.Settings);
    }

    public partial class Ball
    {
        [JsonProperty("position")]
        public List<double?> Position { get; set; }

        [JsonProperty("velocity")]
        public List<double?> Velocity { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }

    public partial class Obstacle
    {
        [JsonProperty("position")]
        public List<double> Position { get; set; }

        [JsonProperty("velocity")]
        public List<double> Velocity { get; set; }

        [JsonProperty("radius")]
        public double Radius { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }

    public partial class Robot
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("pose")]
        public List<double> Pose { get; set; }

        [JsonProperty("targetPose")]
        public List<double> TargetPose { get; set; }

        [JsonProperty("velocity")]
        public List<double> Velocity { get; set; }

        [JsonProperty("intention")]
        public string Intention { get; set; }

        [JsonProperty("batteryLevel")]
        public long BatteryLevel { get; set; }

        [JsonProperty("ballEngaged")]
        public long BallEngaged { get; set; }
    }

    public static class Serialize
    {
        public static string ToJson(this WorldStateMessage self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}

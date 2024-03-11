using MathGame_Domain.Enums;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MathGame_Domain.EntityModels
{
    public class GameExpression : BaseEntity
    {
        [JsonPropertyName("mathExpression")]
        public string? MathExpression { get; set; }

        [JsonPropertyName("result")]
        public double Result { get; set; }

        [JsonPropertyName("gameSessionId")]
        public int GameSessionId { get; set; }

        [JsonPropertyName("gameSession")]
        public GameSession GameSession { get; set; }

        [JsonPropertyName("answerStatus")]
        public AnswerStatus AnswerStatus { get; set; }

        [JsonPropertyName("answeredFromPlayer")]
        public string? AnsweredFromPlayer { get; set; }

        [JsonPropertyName("answer")]
        public double Answer { get; set; }
    }
}

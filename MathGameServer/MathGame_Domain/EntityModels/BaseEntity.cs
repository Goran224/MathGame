using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MathGame_Domain.EntityModels
{
    public class BaseEntity
    {
        [JsonPropertyName("id")]
        [Key]
        public int Id { get; set; }
    }
}

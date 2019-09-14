using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace VDMP.DBmodel
{
    public class Genre
    {
        [NotMapped] [JsonProperty("id")] public int IdGenre { get; set; }
    }
}
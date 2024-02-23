using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GG.Core.Entities;

public class Log
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Message { get; set; }

    public string? MessageTemplate { get; set; }

    public string? Level { get; set; }

    public DateTime? TimeStamp { get; set; }

    public string? Exception { get; set; }

    public string? Properties { get; set; }

    public string? EventType { get; set; }

    public string? UserName { get; set; }

    [NotMapped]
    public Dictionary<string, object>? PropertiesDictionary
    {
        get { return JsonConvert.DeserializeObject<Dictionary<string, object>>(Properties); }
    }
}

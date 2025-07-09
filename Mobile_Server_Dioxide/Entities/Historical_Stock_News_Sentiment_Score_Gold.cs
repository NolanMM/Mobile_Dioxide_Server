using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mobile_Server_Dioxide.Entities
{
    [Table("Historical_Stock_News_Sentiment_Score", Schema = "Gold")]
    public class Historical_Stock_News_Sentiment_Score_Gold
    {
        [Key]
        public int id { get; set; }

        [MaxLength(255)]
        public string? category { get; set; }

        [MaxLength(50)]
        public string? datetime { get; set; }

        public string? headline { get; set; }

        [MaxLength(2083)]
        public string? image { get; set; }

        [MaxLength(255)]
        public string? related { get; set; }

        [MaxLength(255)]
        public string? source { get; set; }

        public string? summary { get; set; }

        [MaxLength(2083)]
        public string? url { get; set; }

        [MaxLength(50)]
        public string? symbol { get; set; }

        public double? positive_value { get; set; }

        public double? negative_value { get; set; }

        public double? neutral_value { get; set; }
    }
}

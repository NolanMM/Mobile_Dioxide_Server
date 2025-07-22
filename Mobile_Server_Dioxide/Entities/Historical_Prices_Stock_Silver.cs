using System.ComponentModel.DataAnnotations.Schema;

namespace Mobile_Server_Dioxide.Entities
{
    [Table("Historical_Prices", Schema = "Silver")]
    public class Historical_Prices_Stock_Silver
    {
        public double? Open { get; set; }

        public double? High { get; set; }

        public double? Low { get; set; }

        public double? Close { get; set; }

        public int? Volume { get; set; }

        public double? Dividends { get; set; }

        public string? Stock_Symbol { get; set; }

        public double? Stock_Splits { get; set; }
        public string? Date { get; set; }
    }
}

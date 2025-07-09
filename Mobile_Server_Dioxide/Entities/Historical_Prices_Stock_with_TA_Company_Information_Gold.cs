using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mobile_Server_Dioxide.Entities
{
    public class Historical_Prices_Stock_with_TA_Company_Information_Gold
    {
        [MaxLength(40)]
        public string Stock_Symbol { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }

        public double? Close { get; set; }

        public double? Open { get; set; }

        public double? High { get; set; }

        public double? Low { get; set; }

        public int? Volume { get; set; }

        public double? Dividends { get; set; }

        public double? Stock_Splits { get; set; }

        public double? RSI_2 { get; set; }

        public double? RSI_3 { get; set; }

        public double? RSI_4 { get; set; }

        public double? RSI_5 { get; set; }

        public double? RSI_6 { get; set; }

        public double? RSI_7 { get; set; }

        public double? RSI_8 { get; set; }

        public double? RSI_9 { get; set; }

        public double? RSI_10 { get; set; }

        public double? RSI_11 { get; set; }

        public double? RSI_12 { get; set; }

        public double? RSI_13 { get; set; }

        public double? RSI_14 { get; set; }

        public double? RSI_15 { get; set; }

        public double? RSI_16 { get; set; }

        public double? RSI_17 { get; set; }

        public double? RSI_18 { get; set; }

        public double? RSI_19 { get; set; }

        public double? RSI_20 { get; set; }

        public double? RSI_21 { get; set; }

        public double? RSI_22 { get; set; }

        public double? RSI_23 { get; set; }

        public double? RSI_24 { get; set; }

        public double? RSI_25 { get; set; }

        public double? RSI_26 { get; set; }

        public double? RSI_27 { get; set; }

        public double? RSI_28 { get; set; }

        public double? RSI_29 { get; set; }

        public double? gain_2 { get; set; }

        public double? gain_3 { get; set; }

        public double? gain_4 { get; set; }

        public double? gain_5 { get; set; }

        public double? gain_6 { get; set; }

        public double? gain_7 { get; set; }

        public double? gain_8 { get; set; }

        public double? gain_9 { get; set; }

        public double? gain_10 { get; set; }

        public double? gain_11 { get; set; }

        public double? gain_12 { get; set; }

        public double? gain_13 { get; set; }

        public double? gain_14 { get; set; }

        public double? gain_15 { get; set; }

        public double? gain_16 { get; set; }

        public double? gain_17 { get; set; }

        public double? gain_18 { get; set; }

        public double? gain_19 { get; set; }

        public double? gain_20 { get; set; }

        public double? gain_21 { get; set; }

        public double? gain_22 { get; set; }

        public double? gain_23 { get; set; }

        public double? gain_24 { get; set; }

        public double? gain_25 { get; set; }

        public double? gain_26 { get; set; }

        public double? gain_27 { get; set; }

        public double? gain_28 { get; set; }

        public double? gain_29 { get; set; }

        public double? loss_2 { get; set; }

        public double? loss_3 { get; set; }

        public double? loss_4 { get; set; }

        public double? loss_5 { get; set; }

        public double? loss_6 { get; set; }

        public double? loss_7 { get; set; }

        public double? loss_8 { get; set; }

        public double? loss_9 { get; set; }

        public double? loss_10 { get; set; }

        public double? loss_11 { get; set; }

        public double? loss_12 { get; set; }

        public double? loss_13 { get; set; }

        public double? loss_14 { get; set; }

        public double? loss_15 { get; set; }

        public double? loss_16 { get; set; }

        public double? loss_17 { get; set; }

        public double? loss_18 { get; set; }

        public double? loss_19 { get; set; }

        public double? loss_20 { get; set; }

        public double? loss_21 { get; set; }

        public double? loss_22 { get; set; }

        public double? loss_23 { get; set; }

        public double? loss_24 { get; set; }

        public double? loss_25 { get; set; }

        public double? loss_26 { get; set; }

        public double? loss_27 { get; set; }

        public double? loss_28 { get; set; }

        public double? loss_29 { get; set; }

        public double? MARD_2 { get; set; }

        public double? MARD_3 { get; set; }

        public double? MARD_4 { get; set; }

        public double? MARD_5 { get; set; }

        public double? MARD_6 { get; set; }

        public double? MARD_7 { get; set; }

        public double? MARD_8 { get; set; }

        public double? MARD_9 { get; set; }

        public double? MARD_10 { get; set; }

        public double? MARD_11 { get; set; }

        public double? MARD_12 { get; set; }

        public double? MARD_13 { get; set; }

        public double? MARD_14 { get; set; }

        public double? MARD_15 { get; set; }

        public double? MARD_16 { get; set; }

        public double? MARD_17 { get; set; }

        public double? MARD_18 { get; set; }

        public double? MARD_19 { get; set; }

        public double? MARD_20 { get; set; }

        public double? MARD_21 { get; set; }

        public double? MARD_22 { get; set; }

        public double? MARD_23 { get; set; }

        public double? MARD_24 { get; set; }

        public double? MARD_25 { get; set; }

        public double? MARD_26 { get; set; }

        public double? MARD_27 { get; set; }

        public double? MARD_28 { get; set; }

        public double? MARD_29 { get; set; }

        public double? price { get; set; }

        public double? beta { get; set; }

        public int? volAvg { get; set; }

        public long? mktCap { get; set; }

        public double? lastDiv { get; set; }

        public string? range { get; set; }

        public double? changes { get; set; }

        public string? companyName { get; set; }

        public string? currency { get; set; }

        public int? cik { get; set; }

        public string? isin { get; set; }

        public string? cusip { get; set; }

        public string? exchange { get; set; }

        public string? exchangeShortName { get; set; }

        public string? industry { get; set; }

        public string? website { get; set; }

        public string? description { get; set; }

        public string? ceo { get; set; }

        public string? sector { get; set; }

        public string? country { get; set; }

        public int? fullTimeEmployees { get; set; }

        public string? phone { get; set; }

        public string? address { get; set; }

        public string? city { get; set; }

        public string? state { get; set; }

        public string? zip { get; set; }

        public double? dcfDiff { get; set; }

        public double? dcf { get; set; }

        public string? image { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ipoDate { get; set; }

        public bool? defaultImage { get; set; }

        public bool? isEtf { get; set; }

        public bool? isActivelyTrading { get; set; }

        public bool? isAdr { get; set; }

        public bool? isFund { get; set; }

        [Column(TypeName = "date")]
        public DateTime? updated_Date { get; set; }
    }
}

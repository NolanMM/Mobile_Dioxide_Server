﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mobile_Server_Dioxide.Entities
{
    [Table("Company_Information", Schema = "Silver")]
    public class Company_Information_Silver
    {
        public string? symbol { get; set; }

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

        public DateTime? updated_date { get; set; }
    }
}

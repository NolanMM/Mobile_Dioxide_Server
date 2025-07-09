using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mobile_Server_Dioxide.Entities
{
    [Table("Macro_Historical", Schema = "Silver")]
    public class Macro_Historical_Silver
    {
        [Key]
        [Column(TypeName = "date")]
        public DateTime date { get; set; }

        public double? gdp_value { get; set; }

        public double? real_gdp_value { get; set; }

        public double? ferfed_funds_effective_rate_value { get; set; }

        public double? labor_force_participant_rate_value { get; set; }

        public double? cpi_value { get; set; }

        public double? unemployment_value { get; set; }

        public double? interest_rate_value { get; set; }

        public double? job_openning_non_farm_value { get; set; }

        public double? hires_total_non_farm_value { get; set; }

        public double? quit_total_non_farm_value { get; set; }

        public double? layoff_discharge_non_farm_value { get; set; }

        public double? layoffs_and_discharges_professional_and_business_services_value { get; set; }

        public double? layoffs_and_discharges_manufacturing_value { get; set; }

        public double? layoffs_and_discharges_fiance_and_insurance_value { get; set; }

        public double? layoffs_and_discharges_construction_value { get; set; }

        public double? layoffs_and_discharges_total_private_value { get; set; }

        public double? layoffs_and_discharges_retail_trade_value { get; set; }

        public double? layoffs_and_discharges_real_estate_and_rental_and_leasing_value { get; set; }

        public double? layoffs_and_discharges_accommodation_and_food_services_value { get; set; }

        public double? real_estate_loans_all_commercial_bank_value { get; set; }

        public double? commercial_real_estate_prices_for_US_rate_value { get; set; }
    }
}

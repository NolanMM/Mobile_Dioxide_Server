using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mobile_Server_Dioxide.Migrations
{
    /// <inheritdoc />
    public partial class Connect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.EnsureSchema(
                name: "Silver");

            migrationBuilder.EnsureSchema(
                name: "Gold");

            migrationBuilder.CreateTable(
                name: "auth_user",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    password = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    last_login = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    is_superuser = table.Column<bool>(type: "bit", nullable: false),
                    username = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    is_staff = table.Column<bool>(type: "bit", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    date_joined = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Company_Information",
                schema: "Silver",
                columns: table => new
                {
                    symbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<double>(type: "float", nullable: true),
                    beta = table.Column<double>(type: "float", nullable: true),
                    volAvg = table.Column<int>(type: "int", nullable: true),
                    mktCap = table.Column<long>(type: "bigint", nullable: true),
                    lastDiv = table.Column<double>(type: "float", nullable: true),
                    range = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    changes = table.Column<double>(type: "float", nullable: true),
                    companyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cik = table.Column<int>(type: "int", nullable: true),
                    isin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cusip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    exchange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    exchangeShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    industry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ceo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fullTimeEmployees = table.Column<int>(type: "int", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    city = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    state = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    zip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dcfDiff = table.Column<double>(type: "float", nullable: true),
                    dcf = table.Column<double>(type: "float", nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ipoDate = table.Column<DateTime>(type: "date", nullable: true),
                    defaultImage = table.Column<bool>(type: "bit", nullable: true),
                    isEtf = table.Column<bool>(type: "bit", nullable: true),
                    isActivelyTrading = table.Column<bool>(type: "bit", nullable: true),
                    isAdr = table.Column<bool>(type: "bit", nullable: true),
                    isFund = table.Column<bool>(type: "bit", nullable: true),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Historical_Prices_Stock_with_TA_Company_Information",
                schema: "Gold",
                columns: table => new
                {
                    Stock_Symbol = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: true),
                    Close = table.Column<double>(type: "float", nullable: true),
                    Open = table.Column<double>(type: "float", nullable: true),
                    High = table.Column<double>(type: "float", nullable: true),
                    Low = table.Column<double>(type: "float", nullable: true),
                    Volume = table.Column<int>(type: "int", nullable: true),
                    Dividends = table.Column<double>(type: "float", nullable: true),
                    Stock_Splits = table.Column<double>(type: "float", nullable: true),
                    RSI_2 = table.Column<double>(type: "float", nullable: true),
                    RSI_3 = table.Column<double>(type: "float", nullable: true),
                    RSI_4 = table.Column<double>(type: "float", nullable: true),
                    RSI_5 = table.Column<double>(type: "float", nullable: true),
                    RSI_6 = table.Column<double>(type: "float", nullable: true),
                    RSI_7 = table.Column<double>(type: "float", nullable: true),
                    RSI_8 = table.Column<double>(type: "float", nullable: true),
                    RSI_9 = table.Column<double>(type: "float", nullable: true),
                    RSI_10 = table.Column<double>(type: "float", nullable: true),
                    RSI_11 = table.Column<double>(type: "float", nullable: true),
                    RSI_12 = table.Column<double>(type: "float", nullable: true),
                    RSI_13 = table.Column<double>(type: "float", nullable: true),
                    RSI_14 = table.Column<double>(type: "float", nullable: true),
                    RSI_15 = table.Column<double>(type: "float", nullable: true),
                    RSI_16 = table.Column<double>(type: "float", nullable: true),
                    RSI_17 = table.Column<double>(type: "float", nullable: true),
                    RSI_18 = table.Column<double>(type: "float", nullable: true),
                    RSI_19 = table.Column<double>(type: "float", nullable: true),
                    RSI_20 = table.Column<double>(type: "float", nullable: true),
                    RSI_21 = table.Column<double>(type: "float", nullable: true),
                    RSI_22 = table.Column<double>(type: "float", nullable: true),
                    RSI_23 = table.Column<double>(type: "float", nullable: true),
                    RSI_24 = table.Column<double>(type: "float", nullable: true),
                    RSI_25 = table.Column<double>(type: "float", nullable: true),
                    RSI_26 = table.Column<double>(type: "float", nullable: true),
                    RSI_27 = table.Column<double>(type: "float", nullable: true),
                    RSI_28 = table.Column<double>(type: "float", nullable: true),
                    RSI_29 = table.Column<double>(type: "float", nullable: true),
                    gain_2 = table.Column<double>(type: "float", nullable: true),
                    gain_3 = table.Column<double>(type: "float", nullable: true),
                    gain_4 = table.Column<double>(type: "float", nullable: true),
                    gain_5 = table.Column<double>(type: "float", nullable: true),
                    gain_6 = table.Column<double>(type: "float", nullable: true),
                    gain_7 = table.Column<double>(type: "float", nullable: true),
                    gain_8 = table.Column<double>(type: "float", nullable: true),
                    gain_9 = table.Column<double>(type: "float", nullable: true),
                    gain_10 = table.Column<double>(type: "float", nullable: true),
                    gain_11 = table.Column<double>(type: "float", nullable: true),
                    gain_12 = table.Column<double>(type: "float", nullable: true),
                    gain_13 = table.Column<double>(type: "float", nullable: true),
                    gain_14 = table.Column<double>(type: "float", nullable: true),
                    gain_15 = table.Column<double>(type: "float", nullable: true),
                    gain_16 = table.Column<double>(type: "float", nullable: true),
                    gain_17 = table.Column<double>(type: "float", nullable: true),
                    gain_18 = table.Column<double>(type: "float", nullable: true),
                    gain_19 = table.Column<double>(type: "float", nullable: true),
                    gain_20 = table.Column<double>(type: "float", nullable: true),
                    gain_21 = table.Column<double>(type: "float", nullable: true),
                    gain_22 = table.Column<double>(type: "float", nullable: true),
                    gain_23 = table.Column<double>(type: "float", nullable: true),
                    gain_24 = table.Column<double>(type: "float", nullable: true),
                    gain_25 = table.Column<double>(type: "float", nullable: true),
                    gain_26 = table.Column<double>(type: "float", nullable: true),
                    gain_27 = table.Column<double>(type: "float", nullable: true),
                    gain_28 = table.Column<double>(type: "float", nullable: true),
                    gain_29 = table.Column<double>(type: "float", nullable: true),
                    loss_2 = table.Column<double>(type: "float", nullable: true),
                    loss_3 = table.Column<double>(type: "float", nullable: true),
                    loss_4 = table.Column<double>(type: "float", nullable: true),
                    loss_5 = table.Column<double>(type: "float", nullable: true),
                    loss_6 = table.Column<double>(type: "float", nullable: true),
                    loss_7 = table.Column<double>(type: "float", nullable: true),
                    loss_8 = table.Column<double>(type: "float", nullable: true),
                    loss_9 = table.Column<double>(type: "float", nullable: true),
                    loss_10 = table.Column<double>(type: "float", nullable: true),
                    loss_11 = table.Column<double>(type: "float", nullable: true),
                    loss_12 = table.Column<double>(type: "float", nullable: true),
                    loss_13 = table.Column<double>(type: "float", nullable: true),
                    loss_14 = table.Column<double>(type: "float", nullable: true),
                    loss_15 = table.Column<double>(type: "float", nullable: true),
                    loss_16 = table.Column<double>(type: "float", nullable: true),
                    loss_17 = table.Column<double>(type: "float", nullable: true),
                    loss_18 = table.Column<double>(type: "float", nullable: true),
                    loss_19 = table.Column<double>(type: "float", nullable: true),
                    loss_20 = table.Column<double>(type: "float", nullable: true),
                    loss_21 = table.Column<double>(type: "float", nullable: true),
                    loss_22 = table.Column<double>(type: "float", nullable: true),
                    loss_23 = table.Column<double>(type: "float", nullable: true),
                    loss_24 = table.Column<double>(type: "float", nullable: true),
                    loss_25 = table.Column<double>(type: "float", nullable: true),
                    loss_26 = table.Column<double>(type: "float", nullable: true),
                    loss_27 = table.Column<double>(type: "float", nullable: true),
                    loss_28 = table.Column<double>(type: "float", nullable: true),
                    loss_29 = table.Column<double>(type: "float", nullable: true),
                    MARD_2 = table.Column<double>(type: "float", nullable: true),
                    MARD_3 = table.Column<double>(type: "float", nullable: true),
                    MARD_4 = table.Column<double>(type: "float", nullable: true),
                    MARD_5 = table.Column<double>(type: "float", nullable: true),
                    MARD_6 = table.Column<double>(type: "float", nullable: true),
                    MARD_7 = table.Column<double>(type: "float", nullable: true),
                    MARD_8 = table.Column<double>(type: "float", nullable: true),
                    MARD_9 = table.Column<double>(type: "float", nullable: true),
                    MARD_10 = table.Column<double>(type: "float", nullable: true),
                    MARD_11 = table.Column<double>(type: "float", nullable: true),
                    MARD_12 = table.Column<double>(type: "float", nullable: true),
                    MARD_13 = table.Column<double>(type: "float", nullable: true),
                    MARD_14 = table.Column<double>(type: "float", nullable: true),
                    MARD_15 = table.Column<double>(type: "float", nullable: true),
                    MARD_16 = table.Column<double>(type: "float", nullable: true),
                    MARD_17 = table.Column<double>(type: "float", nullable: true),
                    MARD_18 = table.Column<double>(type: "float", nullable: true),
                    MARD_19 = table.Column<double>(type: "float", nullable: true),
                    MARD_20 = table.Column<double>(type: "float", nullable: true),
                    MARD_21 = table.Column<double>(type: "float", nullable: true),
                    MARD_22 = table.Column<double>(type: "float", nullable: true),
                    MARD_23 = table.Column<double>(type: "float", nullable: true),
                    MARD_24 = table.Column<double>(type: "float", nullable: true),
                    MARD_25 = table.Column<double>(type: "float", nullable: true),
                    MARD_26 = table.Column<double>(type: "float", nullable: true),
                    MARD_27 = table.Column<double>(type: "float", nullable: true),
                    MARD_28 = table.Column<double>(type: "float", nullable: true),
                    MARD_29 = table.Column<double>(type: "float", nullable: true),
                    price = table.Column<double>(type: "float", nullable: true),
                    beta = table.Column<double>(type: "float", nullable: true),
                    volAvg = table.Column<int>(type: "int", nullable: true),
                    mktCap = table.Column<long>(type: "bigint", nullable: true),
                    lastDiv = table.Column<double>(type: "float", nullable: true),
                    range = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    changes = table.Column<double>(type: "float", nullable: true),
                    companyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cik = table.Column<int>(type: "int", nullable: true),
                    isin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cusip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    exchange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    exchangeShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    industry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ceo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fullTimeEmployees = table.Column<int>(type: "int", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    city = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    state = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    zip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dcfDiff = table.Column<double>(type: "float", nullable: true),
                    dcf = table.Column<double>(type: "float", nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ipoDate = table.Column<DateTime>(type: "date", nullable: true),
                    defaultImage = table.Column<bool>(type: "bit", nullable: true),
                    isEtf = table.Column<bool>(type: "bit", nullable: true),
                    isActivelyTrading = table.Column<bool>(type: "bit", nullable: true),
                    isAdr = table.Column<bool>(type: "bit", nullable: true),
                    isFund = table.Column<bool>(type: "bit", nullable: true),
                    updated_Date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Historical_Stock_News_Sentiment_Score",
                schema: "Gold",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    datetime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    headline = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image = table.Column<string>(type: "nvarchar(2083)", maxLength: 2083, nullable: true),
                    related = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    source = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    url = table.Column<string>(type: "nvarchar(2083)", maxLength: 2083, nullable: true),
                    symbol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    positive_value = table.Column<double>(type: "float", nullable: true),
                    negative_value = table.Column<double>(type: "float", nullable: true),
                    neutral_value = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historical_Stock_News_Sentiment_Score", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Macro_Historical",
                schema: "Silver",
                columns: table => new
                {
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    gdp_value = table.Column<double>(type: "float", nullable: true),
                    real_gdp_value = table.Column<double>(type: "float", nullable: true),
                    ferfed_funds_effective_rate_value = table.Column<double>(type: "float", nullable: true),
                    labor_force_participant_rate_value = table.Column<double>(type: "float", nullable: true),
                    cpi_value = table.Column<double>(type: "float", nullable: true),
                    unemployment_value = table.Column<double>(type: "float", nullable: true),
                    interest_rate_value = table.Column<double>(type: "float", nullable: true),
                    job_openning_non_farm_value = table.Column<double>(type: "float", nullable: true),
                    hires_total_non_farm_value = table.Column<double>(type: "float", nullable: true),
                    quit_total_non_farm_value = table.Column<double>(type: "float", nullable: true),
                    layoff_discharge_non_farm_value = table.Column<double>(type: "float", nullable: true),
                    layoffs_and_discharges_professional_and_business_services_value = table.Column<double>(type: "float", nullable: true),
                    layoffs_and_discharges_manufacturing_value = table.Column<double>(type: "float", nullable: true),
                    layoffs_and_discharges_fiance_and_insurance_value = table.Column<double>(type: "float", nullable: true),
                    layoffs_and_discharges_construction_value = table.Column<double>(type: "float", nullable: true),
                    layoffs_and_discharges_total_private_value = table.Column<double>(type: "float", nullable: true),
                    layoffs_and_discharges_retail_trade_value = table.Column<double>(type: "float", nullable: true),
                    layoffs_and_discharges_real_estate_and_rental_and_leasing_value = table.Column<double>(type: "float", nullable: true),
                    layoffs_and_discharges_accommodation_and_food_services_value = table.Column<double>(type: "float", nullable: true),
                    real_estate_loans_all_commercial_bank_value = table.Column<double>(type: "float", nullable: true),
                    commercial_real_estate_prices_for_US_rate_value = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auth_user",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Company_Information",
                schema: "Silver");

            migrationBuilder.DropTable(
                name: "Historical_Prices_Stock_with_TA_Company_Information",
                schema: "Gold");

            migrationBuilder.DropTable(
                name: "Historical_Stock_News_Sentiment_Score",
                schema: "Gold");

            migrationBuilder.DropTable(
                name: "Macro_Historical",
                schema: "Silver");
        }
    }
}

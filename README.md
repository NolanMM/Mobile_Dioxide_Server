# Mobile Server Dioxide

This project is an ASP.NET Core Web API that provides financial data, including stock prices, news sentiment, and macroeconomic data. It also includes user management features, such as registration, login, and user profile updates.

**Link: [Mobile Server Dioxide](https://capstonedioxiemobileserver-cfgqfudtbea6crd2.canadacentral-01.azurewebsites.net/swagger/index.html)**

## Features

-   **Stock Data:** Get historical stock prices for various symbols, with different tiers of data available (gold and silver).
-   **News Sentiment:** Get news sentiment data for specific stock symbols or for a specified number of days.
-   **Macroeconomic Data:** Get historical macroeconomic data.
-   **User Management:** Register new users, log in, and update user information.
-   **Caching:** The API uses in-memory caching to improve performance.

## API Endpoints

Here is a list of the available API endpoints:

### Stock Data

-   `GET /api/MobileDioxie/Stock/Symbols/Available`: Get a list of available stock symbols.
-   `GET /api/MobileDioxie/Get_Stock_Price/{stockSymbol}`: Get historical stock prices for a given symbol (gold tier).
-   `GET /api/MobileDioxie/Get_Stock_Price_Silver/{stockSymbol}/{start_time}/{end_time}/{type}`: Get historical stock prices for a given symbol within a date range and for a specific data type (e.g., open, high, low, close) (silver tier).
-   `GET /api/MobileDioxie/Get_Stock_Price_Silver/{stockSymbol}/{start_time}/{end_time}`: Get all historical stock price data for a given symbol within a date range (silver tier).
-   `GET /api/MobileDioxie/Get_Stock_Price_Silver/{stockSymbol}`: Get all historical stock price data for a given symbol (silver tier).
-   `GET /api/MobileDioxie/Get_Stock_Price/{stockSymbol}/{start_time}/{end_time}`: Get historical stock prices for a given symbol within a date range (gold tier).

### News Sentiment

-   `GET /api/MobileDioxie/Get_News_Sentiment_by_Days/{number_of_days}`: Get news sentiment data for a specified number of days.
-   `GET /api/MobileDioxie/Get_News_Sentiment_by_Symbol/{symbol}`: Get news sentiment data for a specific stock symbol.

### User Management

-   `GET /api/MobileDioxie/Get_All_Users`: Get a list of all users.
-   `POST /api/MobileDioxie/Register_User/Request`: Initiate a user registration request and sends an OTP to the user's email.
-   `GET /api/MobileDioxie/Register_User/{OTP_Number}/{SessionID}`: Complete the user registration process by verifying the OTP.
-   `POST /api/MobileDioxie/Login_User`: Log in a user.
-   `POST /api/MobileDioxie/user/{id}/username`: Update a user's username.
-   `POST /api/MobileDioxie/user/{id}/name`: Update a user's first and last name.

### Macroeconomic Data

-   `GET /api/MobileDioxie/Get_Macro_Historical_by_Days/{number_of_days}`: Get historical macroeconomic data for a specified number of days.

## Database Schema

The application uses Entity Framework Core to interact with a database. The database contains the following tables (which are actually views):

-   `Historical_Prices_Stock_with_TA_Company_Information_Gold`
-   `Historical_Stock_News_Sentiment_Score_Gold`
-   `Macro_Historical_Silver`
-   `Company_Information_Silver`
-   `User_DBO`
-   `Historical_Prices_Stock_Silver`

## Getting Started

To get started with this project, you will need to have the following installed:

-   .NET 8 SDK
-   Visual Studio 2022 or later (optional)

To run the project, follow these steps:

1.  Clone the repository.
2.  Open the solution in Visual Studio or use the `dotnet` CLI.
3.  Set up the database connection string in `appsettings.json`.
4.  Run the application.

## Technologies Used

-   .NET 8
-   ASP.NET Core
-   Entity Framework Core
-   Swagger/OpenAPI
-   xUnit
-   Moq

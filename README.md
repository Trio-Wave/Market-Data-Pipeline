tock Market Data Application

## Technology Stack

| Category           | Technology            |
| ------------------ | --------------------- |
| **Backend**        | .NET 10               |
| **ORM**            | Entity Framework Core |
| **Database**       | SQL Server            |
| **Job Scheduling** | Hangfire              |
| **Deployment**     | Windows Service       |
| **External API**   | Alpha Vantage         |

## Features

### Sync Job

* Hangfire job executes daily after the market closes.
* Pulls stock market data for **26 symbols**.
* Stores the retrieved data in the local SQL database.

### Backfill Sync Job

* Pulls stock market quotes for a specified symbol.
* Retrieves data for the **past 100 days**.
* Stores the retrieved data in the local SQL database.

### Visuals

* Displays quote data in a table.
* Allows the user to select a symbol.
* Updates the table to display data for the selected symbol.

---

# Developer Notes

## Entity Framework Scaffold Command

```bash
dotnet ef dbcontext scaffold "Server=LivTop\TRIOWAVEDEV;Database=GeneralDW;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --table Symbols --table StockPrice --context GeneralDWContext --data-annotations -f
```

---

# To Do

* [ ] **Add graph to UI**

  * Add functionality similar to the existing quote data table.
    * Allow the user to select between symbols.

    * [ ] **Tidy up `GeneralDW` service functions**

      * Consolidate similar functions.
        * Reorganize functions where appropriate.

        * [ ] **Tidy up `MarketDataController`**

          * Remove unnecessary functions, particularly CSV-related functionality.
            * Consider redesigning `Index()`.

            * [ ] **Set up a fallback model for the Alpha Vantage API**

              * Implement a fallback when the daily API rate limit has been reached.

              * [ ] **Improve logging**

                * Add more detailed logging for each step of the daily sync.
                  * Remove redundant logs.

                  * [ ] **Rework the Backfill Sync UI**

                    * Improve how the Backfill Sync button is displayed.
                      * Consider reworking the underlying functionality.

                      * [ ] **Add SQL table for sync runs**

                        * Track individual sync executions and their status.

                        * [ ] **Add SQL table for audit logging**

                          * Track relevant application and data changes.


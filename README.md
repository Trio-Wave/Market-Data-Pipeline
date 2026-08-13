# Market-Data-Pipeline

Entity Framework Scaffold Command:

dotnet ef dbcontext scaffold "Server=LivTop\TRIOWAVEDEV;Database=GeneralDW;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --table Symbols --table StockPrice --context GeneralDWContext --data-annotations -f

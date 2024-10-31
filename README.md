# CampusFest

## About


## Getting started

The following prerequisites are required to build and run the solution:
+ NET 8.0 SDK (latest)
+ SQL Server


#### Using .NET Core CLI:

Navigate to the solution folder, then open command prompt.
Use the following command to generate database migration and snapshot.

```bash
dotnet ef migrations add YourMigrationName --output-dir "Infrastructures/Migrations"
```

Then update the database using

```bash
dotnet ef database update
```

Alternatively, generate and execute a generated sql script using

```
dotnet ef migrations script
```


#### Nuget Package Manager Console

To create migration:

```bash
 Add-Migration YourMigrationName -OutputDir Infrastructures/Migrations
```

To update database:
```bash
 Update-Database
```

## License
This project is licensed under the MIT License.
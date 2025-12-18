# Entity Framework Core Migrations

This directory contains EF Core migrations for the Partner Management System database.

## About Code First Migrations

Entity Framework Core uses a Code First approach where:
1. You define your entity classes in C# code
2. EF Core generates database schema from your models
3. Migrations track changes to your models over time
4. You can update the database schema without losing data

## Current Migrations

- **InitialCreate** (20251218032610) - Initial database schema
  - AspNetCore Identity tables (Users, Roles, etc.)
  - Partners table
  - Events table
  - EventPartners junction table
  - All indexes and constraints

## Applying Migrations

### Update Database

Apply all pending migrations to your database:

```bash
cd PartnerManagement.Api
dotnet ef database update
```

### Update to Specific Migration

```bash
dotnet ef database update <MigrationName>
```

### Rollback to Previous Migration

```bash
dotnet ef database update <PreviousMigrationName>
```

### Drop Database

```bash
dotnet ef database drop
```

## Creating New Migrations

When you modify entity classes or configurations:

### 1. Add a New Migration

```bash
cd PartnerManagement.Api
dotnet ef migrations add <MigrationName> --project ../PartnerManagement.Infrastructure --output-dir Data/Migrations
```

Example:
```bash
dotnet ef migrations add AddPartnerCategory --project ../PartnerManagement.Infrastructure --output-dir Data/Migrations
```

### 2. Review Generated Migration

Check the generated migration file in this directory to ensure it matches your intended changes.

### 3. Apply Migration

```bash
dotnet ef database update
```

## Removing Last Migration

If you haven't applied a migration yet and want to remove it:

```bash
cd PartnerManagement.Api
dotnet ef migrations remove --project ../PartnerManagement.Infrastructure
```

**Note**: You can only remove the last migration if it hasn't been applied to any database.

## Database Initialization

The `DbInitializer` class handles:
- Automatic migration application on startup
- Seeding default roles (Admin, User)
- Creating default admin user

Default admin credentials:
- Email: `admin@partnermanagement.com`
- Password: `Admin@123`

**Important**: Change the default admin password in production!

## Connection String

Configure your PostgreSQL connection in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=partnermanagement;Username=postgres;Password=yourpassword;Pooling=true;MinPoolSize=10;MaxPoolSize=200;"
  }
}
```

## Migration Best Practices

1. **Always review migrations** before applying them
2. **Test migrations** on a development database first
3. **Backup production data** before running migrations
4. **Use descriptive names** for migrations (e.g., AddUserPhoneNumber)
5. **One logical change** per migration
6. **Don't modify** existing migration files after they're applied
7. **Version control** all migration files

## Troubleshooting

### Migration Build Errors

If you get build errors when creating migrations:

```bash
# Build the solution first
dotnet build

# Then create the migration
dotnet ef migrations add <MigrationName> --project ../PartnerManagement.Infrastructure
```

### Connection Errors

Ensure PostgreSQL is running and connection string is correct:

```bash
# Test connection
psql -h localhost -U postgres -d partnermanagement
```

### Pending Changes Warning

If EF Core detects model changes that aren't in a migration:

```bash
# Create a new migration for the changes
dotnet ef migrations add <DescriptiveName> --project ../PartnerManagement.Infrastructure --output-dir Data/Migrations
```

## Entity Configurations

Entity configurations are defined in separate configuration classes:
- `PartnerConfiguration.cs` - Partner entity mapping
- `EventConfiguration.cs` - Event entity mapping
- `EventPartnerConfiguration.cs` - EventPartner relationship
- `ApplicationUserConfiguration.cs` - User entity extensions

These configurations are automatically applied via:
```csharp
builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
```

## Useful Commands Reference

```bash
# List all migrations
dotnet ef migrations list --project ../PartnerManagement.Infrastructure

# Generate SQL script for a migration
dotnet ef migrations script --project ../PartnerManagement.Infrastructure

# Generate SQL script from one migration to another
dotnet ef migrations script FromMigration ToMigration --project ../PartnerManagement.Infrastructure

# Get database information
dotnet ef dbcontext info --project ../PartnerManagement.Infrastructure

# Scaffold DbContext from existing database (reverse engineering)
dotnet ef dbcontext scaffold "Host=localhost;Database=mydb;Username=postgres;Password=pass" Npgsql.EntityFrameworkCore.PostgreSQL --project ../PartnerManagement.Infrastructure
```

## Additional Resources

- [EF Core Migrations Documentation](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [EF Core CLI Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
- [Npgsql EF Core Provider](https://www.npgsql.org/efcore/)

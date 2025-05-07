# EAVPipeline

An end-to-end prototype pipeline that transforms structured relational data into a dynamic Entity-Attribute-Value (EAV) schema using C#, SQLite, and Dapper. The project simulates customers, products, and purchase interactions, and demonstrates how to derive custom attributes efficiently from large-scale raw data.

---

## Project Structure

```
EAVPipeline/
├── EAVPipeline.App/                  # Entry point: seeds data & runs pipeline
│   └── Program.cs
├── EAVPipeline.Core/                # Core logic and models
│   ├── Attributes/                  # Attribute derivation modules
│   │   └── product_count.cs
│   ├── Engine/                      # EntityEngine & PipelineProcessor
│   ├── Infrastructure/             # Schema setup & simulators
│   ├── Models/                     # EAV models (AttributeDefinition, etc.)
│   ├── Repositories/               # Database access logic
│   └── Utilities/                  # Optional SQL utility tools
├── EAVPipeline.Utilities/          # Project for executing adhoc admin/cleanup tasks
│   └── Program.cs
├── eav.db                          # SQLite database file (auto-generated)
└── README.md                       # You are here
```

---

## Schema Overview

### Core Tables

* `MasterEntities (EntityId, Name)` — main list of customers
* `Products (ProductId, Name, Category, BasePrice, ReleaseYear)`
* `RawInputData (EntityId, PropertyKey, PropertyValue)` — raw wide-table style
* `AttributeDefinitions (AttributeId, Slug, ...)`
* `AttributeValues (EntityId, AttributeId, Value, ArchivedAt)` — core EAV store

### Derivation Source (Example)

* `CustomerPurchases (CustomerId, ProductId, PricePaid, PurchasedOn)`

---

## Setup & Execution

### 1. Build & Run

```
dotnet build EAVPipeline.sln
dotnet run --project EAVPipeline.App
```

This will:

* Ensure database schema
* Simulate 1M customers, 100 products
* Generate 500k purchase interactions
* Derive `product_count` attribute for each customer

### 2. Inspect DB

Use [DB Browser for SQLite](https://sqlitebrowser.org/) to explore `eav.db`.

---

## Deriving Attributes

### Adding New Attributes

Add a new file in `EAVPipeline.Core/Attributes/`, for example:

```csharp
public class average_spend {
    public static void Compute(IDbConnection conn) {
        // Load aggregate, write to AttributeValues via temp table
    }
}
```

Call the compute method in `Program.cs`.

### Attribute Storage Strategy

* Archived values (via `ArchivedAt`) for full history
* Efficient overwriting via `TempProductCounts`

---

## Utilities

You can execute admin scripts via the `EAVPipeline.Utilities` project:

```bash
dotnet run --project EAVPipeline.Utilities
```

Example use:

* Cleanup obsolete tables
* Mass archiving
* Manual inserts

---

## Future Directions

* Add metadata for `AttributeDefinition` (e.g., source, transform logic)
* Build web interface for tracking entities/attributes
* Support attribute dependencies
* Integrate Python (via script runner or embedded engine) for generating or validating data

---

## Author

Built with ❤️ using C#, SQLite, and a lot of simulated data.

Feel free to extend this project to fit domain-specific use cases (retail, IoT, CRM).

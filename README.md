# EAVPipeline

EAVPipeline

An end-to-end prototype pipeline that transforms structured relational data into a dynamic Entity-Attribute-Value (EAV) schema using C#, SQLite, and Dapper. The project simulates customers, products, and purchase interactions, and demonstrates how to derive custom attributes efficiently from large-scale raw data.


## Features

- **Entity-Attribute-Value Data Handling**: Efficiently manage EAV data models.
- **Pipeline Processing**: Streamlined data processing pipelines for EAV structures.
- **Database Integration**: Supports SQLite databases (`eav.db`) for storing and querying EAV data.
- **Customizable**: Easily extendable to fit specific use cases.

## Project structure
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
└── README.md 


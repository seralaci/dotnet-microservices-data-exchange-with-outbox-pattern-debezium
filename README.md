# True Atomic Microservices Implementation with Debezium and Outbox Pattern to Ensure Data Consistency

# SQL - Enables change data capture

```sql
USE TrueAtomicMicroservices-Db
GO

EXEC sys.sp_cdc_enable_db
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'dbo',
    @source_name   = N'OutboxEvents',
    @role_name     = N'Admin',
    @supports_net_changes = 1
```

# SQL - Disable change data capture

```sql
EXEC sys.sp_cdc_disable_db
```

## Debezium Connectors
Use /http/debezium.http to register the debezium connector

## Kafdrop
http://localhost:9000/


## smtp4dev
http://localhost:5000/

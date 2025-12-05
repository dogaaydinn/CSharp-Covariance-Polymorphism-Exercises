# Database Migration

## Problem
Migrating 100GB database from SQL Server to PostgreSQL with zero downtime.

## Solutions
1. **Basic**: Backup/Restore with downtime (maintenance window)
2. **Advanced**: Dual-Write pattern (write to both DBs)
3. **Enterprise**: CDC (Change Data Capture) + streaming replication

## Migration Strategies
- **Big Bang**: Switch all at once (high risk)
- **Strangler Fig**: Migrate table-by-table
- **Shadow Mode**: Run new DB in parallel, verify results
- **Blue-Green**: Swap databases after full sync

## Steps
1. Schema migration (convert DDL)
2. Data migration (bulk copy)
3. Incremental sync (capture changes)
4. Validation (compare data)
5. Cutover (switch application)
6. Cleanup (decommission old DB)

See PROBLEM.md for detailed migration plans and rollback strategies.

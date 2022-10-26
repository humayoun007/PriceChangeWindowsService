SELECT is_broker_enabled,* FROM sys.databases


USE master;
GO

alter database [FundScreeningTest] set enable_broker with rollback immediate;
GO
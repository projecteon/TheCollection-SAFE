module Config

open TeaCollection.Infrastructure.MsSql

type Config = {
  Database: DbContext.DbConfig
  AzureStorage: AzureStorage.StorageAccount
}
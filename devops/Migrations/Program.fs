open System.Reflection
open Microsoft.Data.SqlClient;

open SimpleMigrations
open SimpleMigrations.DatabaseProvider

let createDatabaseIfNotExists() =
  printfn "Initializing database..."
  let connectionString = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=TRUE;";
  use db = new SqlConnection(connectionString)
  use command = db.CreateCommand();
  db.Open() |> ignore;
  command.CommandText <- @"
IF NOT EXISTS 
   (
     SELECT name FROM master.dbo.sysdatabases 
     WHERE name = N'the_collection'
    )
CREATE DATABASE the_collection;
  ";
  command.ExecuteNonQuery() |> ignore;

[<EntryPoint>]
let main argv =
  printfn "Migrations start..."

  createDatabaseIfNotExists() |> ignore

  let connectionString = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=the_collection;Integrated Security=TRUE;";
  let migrationsAssembly = Assembly.GetExecutingAssembly();
  use db = new SqlConnection(connectionString)
  let databaseProvider = new MssqlDatabaseProvider(db);
  let migrator = new SimpleMigrator(migrationsAssembly, databaseProvider);

  migrator.Load();
  //printfn "Rollback to beginning..."
  //migrator.MigrateTo(0L);

  printfn "Migrate to latest..."
  migrator.MigrateToLatest();

  printfn "Migrations complete..."
  0
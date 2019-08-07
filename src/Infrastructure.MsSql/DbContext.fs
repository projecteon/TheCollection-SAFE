namespace TeaCollection.Infrastructure.MsSql

module DbContext =
  [<Literal>]
  //let  DevConnectionString = "Server=tcp:collection.database.windows.net,1433;Initial Catalog=the_collection;Persist Security Info=False;User ID=collectiondb;Password=Zs9O0PjaVm8il65alfOP;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  let  DevConnectionString = "Server=(LocalDB)\MSSQLLocalDB;Database=the_collection;Trusted_Connection=True;MultipleActiveResultSets=true"

  type PageSize = PageSize of int64  
    with
    member this.int64 = let (PageSize s) = this in s
    member this.pageStart pageNumber = this.int64 * pageNumber

  type ConnectionString = ConnectionString of string  
    with
    member this.String = let (ConnectionString s) = this in s
    
  type DbConfig = {
    PageSize: PageSize
    Default: ConnectionString 
    ReadOnly: ConnectionString
  }
  
module AzureStorage =
  // $"DefaultEndpointsProtocol={scheme};AccountName={name};AccountKey={key};{endpoints}"
  type Scheme = Scheme of string
    with
    member this.String = let (Scheme s) = this in s

  type AccountName = AccountName of string
    with
    member this.String = let (AccountName s) = this in s

  type AccountKey = AccountKey of string
    with
    member this.String = let (AccountKey s) = this in s

  type BlobEndpoint = BlobEndpoint of string
    with
    member this.String = let (BlobEndpoint s) = this in s

  type TableEndpoint = TableEndpoint of string
    with
    member this.String = let (TableEndpoint s) = this in s

  type QueueEndpoint = QueueEndpoint of string
    with
    member this.String = let (QueueEndpoint s) = this in s

  type Endpoint = {
    BlobEndpoint: BlobEndpoint
    TableEndpoint: TableEndpoint
    QueueEndpoint: QueueEndpoint
  }
  with
  member this.String = sprintf "BlobEndpoint=%s;TableEndpoint=%s;QueueEndpoint=%s;" this.BlobEndpoint.String this.TableEndpoint.String this.QueueEndpoint.String
  end

  type StorageAccount = {
    Scheme: Scheme
    AccountName: AccountName
    AccountKey: AccountKey
    Endpoints: Endpoint
  }
  with
  member this.ConnectionString = sprintf "DefaultEndpointsProtocol=%s;AccountName=%s;AccountKey=%s;%s" this.Scheme.String this.AccountName.String this.AccountKey.String this.Endpoints.String
  end

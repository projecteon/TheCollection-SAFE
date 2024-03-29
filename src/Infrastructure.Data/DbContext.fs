namespace Infrastructure.Data

module DbContext =
  [<Literal>]
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

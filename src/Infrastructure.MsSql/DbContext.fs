namespace TeaCollection.Infrastructure.MsSql

module DbContext =
    [<Literal>]
    let  ConnectionString = "Server=(LocalDB)\MSSQLLocalDB;Database=the_collection;Trusted_Connection=True;MultipleActiveResultSets=true"

    let pageSize = (int64 100)
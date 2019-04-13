namespace TeaCollection.Infrastructure.MsSql

open FSharp.Control.Tasks.V2

open FSharp.Data
open DbContext
open Domain.SharedTypes

module FileRepository =
    [<Literal>]
    let  InsertSQL = "
        INSERT INTO tcd_files (s_path, s_filename)
        OUTPUT Inserted.ID
        VALUES (@0, @1)
    "

    type Insert = SqlCommandProvider<InsertSQL, ConnectionString, SingleRow = true>
    let insert (connectiongString: string) (path, filename) =
      task {
        let cmd = new Insert(connectiongString)
        return cmd.Execute(path, filename)
                |> function
                  | Some x -> x |> DbId |> Some
                  | _ -> None
      }

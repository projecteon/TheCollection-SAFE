namespace TeaCollection.Infrastructure.MsSql

open FSharp.Control.Tasks.V2

open FSharp.Data
open DbContext
open Domain.SharedTypes
open Domain.Tea

module FileRepository =
    open Domain.Types

    [<Literal>]
    let  InsertSQL = "
        INSERT INTO tcd_files (s_uri, s_filename)
        OUTPUT Inserted.ID
        VALUES (@0, @1)
    "

    type Insert = SqlCommandProvider<InsertSQL, DevConnectionString, SingleRow = true>
    let insert (config: DbConfig) (file: Domain.Tea.File) =
      task {
        let cmd = new Insert(config.Default.String)
        return cmd.Execute(file.uri, file.filename)
                |> function
                  | Some x -> x |> DbId |> Some
                  | _ -> None
      }

    [<Literal>]
    let  ByIdSQL = "
        SELECT a.*
          FROM tcd_files a
        WHERE id = @id
    "

    type FileById = SqlCommandProvider<ByIdSQL, DevConnectionString, SingleRow = true>

    let mapByIdData (record: FileById.Record): File = {
        id = DbId record.id
        importId = record.i_import_id
        uri = record.s_uri
        filename = record.s_filename
        created = (Mapping.toInstant <| record.dt_created) |> CreatedDate
        modified = (Mapping.toInstant <| record.dt_modified) |> ModifiedDate
      }

    let getById (config: DbConfig) id =
      task {
        let cmd = new FileById(config.ReadOnly.String)
        let! teabag = cmd.AsyncExecute(id)
        return match teabag with
                | Some x -> x |> mapByIdData |> Some
                | _ -> None
      }

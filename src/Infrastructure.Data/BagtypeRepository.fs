namespace Infrastructure.Data

open System.Threading.Tasks
open FSharp.Control.Tasks.V2

open FSharp.Data
open DbContext
open Domain.SharedTypes
open Domain.Types
open Domain.Tea
open Search
open Util

module BagtypeRepository =
    [<Literal>]
    let  QrySQL = "
        DECLARE @sName varchar(50)
        SET @sName = @name

        SELECT a.id, a.s_name
        FROM tco_bagtype a
        WHERE 1=1
          AND (LEN(@sName) = 0 OR a.s_name LIKE @sName)
    "

    type BagtypeQry = SqlCommandProvider<QrySQL, DevConnectionString>

    let getAll (config: DbConfig) (nameFilter: SearchParams) : Task<Bagtype list> =
      task {
        let cmd = new BagtypeQry(config.ReadOnly.String)
        return cmd.Execute(nameFilter.Term |> extractSearchTerm)
        |> List.ofSeq
        |> Seq.map (fun x -> {
            id = DbId x.id
            Bagtype.name = x.s_name |> BagtypeName
          })
        |> Seq.toList
      }


    [<Literal>]
    let  ByIdSQL = "
        SELECT a.id, a.s_name
        FROM tco_bagtype a
        WHERE id = @id
    "

    type BagtypeById = SqlCommandProvider<ByIdSQL, DevConnectionString, SingleRow = true>

    let getById (config: DbConfig) (id: int) : Task<Bagtype option> =
      task {
        let cmd = new BagtypeById(config.ReadOnly.String)
        return cmd.Execute(id)
        |> function
          | Some x -> Some {
              id = DbId x.id
              Bagtype.name = x.s_name |> BagtypeName
            }
          | _ -> None
      }

    [<Literal>]
    let  InsertSQL = "
        INSERT INTO tco_bagtype (s_name, dt_created, dt_modified)
        OUTPUT Inserted.ID
        VALUES(@name, GETDATE(), GETDATE());
    "

    type InsertBagtype = SqlCommandProvider<InsertSQL, DevConnectionString, SingleRow = true>

    let insert (config: DbConfig) (bagtype : Bagtype) =
      task {
        let cmd = new InsertBagtype(config.Default.String)
        return cmd.AsyncExecute(bagtype.name.String)
      }

    [<Literal>]
    let  UpdateSQL = "
        UPDATE tco_bagtype SET
          s_name = @s_name
          ,dt_modified = GETDATE()
        WHERE id = @id;
    "

    type UpdateBagtype = SqlCommandProvider<UpdateSQL, DevConnectionString, SingleRow = true>
    let update (config: DbConfig) (bagtype : Bagtype) =
      task {
        let cmd = new UpdateBagtype(config.Default.String)
        return! cmd.AsyncExecute(bagtype.name.String, bagtype.id.Int)
      }
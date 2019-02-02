namespace TeaCollection.Infrastructure.MsSql

open System.Threading.Tasks
open FSharp.Control.Tasks.V2

open FSharp.Data
open DbContext
open Domain.SharedTypes
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

    type BagtypeQry = SqlCommandProvider<QrySQL, ConnectionString>

    let getAll (connectiongString: string) (nameFilter: SearchTerm) : Task<Bagtype list> =
      task {
        let cmd = new BagtypeQry(connectiongString)
        return cmd.Execute(nameFilter |> extractSearchTerm)
        |> List.ofSeq
        |> Seq.map (fun x -> {
            id = DbId x.id
            Bagtype.name = x.s_name
          })
        |> Seq.toList
      }


    [<Literal>]
    let  ByIdSQL = "
        SELECT a.id, a.s_name
        FROM tco_bagtype a
        WHERE id = @id
    "

    type BagtypeById = SqlCommandProvider<ByIdSQL, ConnectionString, SingleRow = true>

    let getById (connectiongString: string) (id: int) : Task<Bagtype option> =
      task {
        let cmd = new BagtypeById(connectiongString)
        return cmd.Execute(id)
        |> function
          | Some x -> Some {
              id = DbId x.id
              Bagtype.name = x.s_name
            }
          | _ -> None
      }

    [<Literal>]
    let  InsertSQL = "
        INSERT INTO tco_bagtype (s_name)
        OUTPUT Inserted.ID
        VALUES(@name);
    "

    type InsertBagtype = SqlCommandProvider<InsertSQL, ConnectionString, SingleRow = true>

    let insert (connectiongString: string) (bagtype : Bagtype) =
      let cmd = new InsertBagtype(connectiongString)
      cmd.Execute(bagtype.name)
      |> function
        | Some x -> Some { bagtype with id = DbId x }
        | _ -> None

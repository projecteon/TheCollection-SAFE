namespace TeaCollection.Infrastructure.MsSql

open System.Threading.Tasks
open FSharp.Control.Tasks.V2

open FSharp.Data
open DbContext
open Domain.SharedTypes
open Domain.Tea
open Search
open Util

module CountryRepository =
    [<Literal>]
    let  QrySQL = "
        DECLARE @sName varchar(50)
        SET @sName = @name

        SELECT a.id, a.s_name
        FROM tcs_country a
        WHERE 1=1
          AND (LEN(@sName) = 0 OR a.s_name LIKE @sName)
    "

    type CountryQry = SqlCommandProvider<QrySQL, ConnectionString>

    let getAll (connectiongString: string) (nameFilter: SearchTerm) : Task<Country list> =
      task {
        let cmd = new CountryQry(connectiongString)
        return cmd.Execute(nameFilter |> extractSearchTerm)
        |> List.ofSeq
        |> Seq.map (fun x -> {
            id = DbId x.id
            Country.name = x.s_name
          })
        |> Seq.toList
      }


    [<Literal>]
    let  ByIdSQL = "
        SELECT a.id, a.s_name
        FROM tcs_country a
        WHERE id = @id
    "

    type CountryById = SqlCommandProvider<ByIdSQL, ConnectionString, SingleRow = true>

    let getById (connectiongString: string)  (id: int) : Task<Country option> =
      task {
        let cmd = new CountryById(connectiongString)
        return cmd.Execute(id)
        |> function
          | Some x -> Some {
              id = DbId x.id
              Country.name = x.s_name
            }
          | _ -> None
      }

    [<Literal>]
    let  InsertSQL = "
        INSERT INTO tcs_country (s_name)
        OUTPUT Inserted.ID
        VALUES(@name);
    "

    type InsertCountry = SqlCommandProvider<InsertSQL, ConnectionString, SingleRow = true>

    let insert (connectiongString: string) (country : Country) =
        let cmd = new InsertCountry(connectiongString)
        cmd.Execute(country.name)
        |> function
            | Some x -> Some { country with id = DbId x }
            | _ -> None

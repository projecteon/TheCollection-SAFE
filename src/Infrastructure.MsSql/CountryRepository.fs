namespace TeaCollection.Infrastructure.MsSql

open System.Threading.Tasks
open FSharp.Control.Tasks.V2

open FSharp.Data
open DbContext
open Domain.SharedTypes
open Domain.Types
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

    type CountryQry = SqlCommandProvider<QrySQL, DevConnectionString>

    let getAll (connectiongString: string) (nameFilter: SearchParams) : Task<Country list> =
      task {
        let cmd = new CountryQry(connectiongString)
        return cmd.Execute(nameFilter.Term |> extractSearchTerm)
        |> List.ofSeq
        |> Seq.map (fun x -> {
            id = DbId x.id
            Country.name = x.s_name |> CountryName
          })
        |> Seq.toList
      }


    [<Literal>]
    let  ByIdSQL = "
        SELECT a.id, a.s_name
        FROM tcs_country a
        WHERE id = @id
    "

    type CountryById = SqlCommandProvider<ByIdSQL, DevConnectionString, SingleRow = true>

    let getById (connectiongString: string)  (id: int) : Task<Country option> =
      task {
        let cmd = new CountryById(connectiongString)
        return cmd.Execute(id)
        |> function
          | Some x -> Some {
              id = DbId x.id
              Country.name = x.s_name |> CountryName
            }
          | _ -> None
      }


    [<Literal>]
    let  InsertSQL = "
        INSERT INTO tcs_country (s_name)
        OUTPUT Inserted.ID
        VALUES(@name);
    "

    type InsertCountry = SqlCommandProvider<InsertSQL, DevConnectionString, SingleRow = true>
    let insert (connectiongString: string) (country : Country) =
      task {
        let cmd = new InsertCountry(connectiongString)
        return cmd.AsyncExecute(country.name.String)
      }

    [<Literal>]
    let  UpdateSQL = "
        UPDATE tcs_country SET
          s_name = @s_name
        WHERE id = @id;
    "

    type UpdateCountry = SqlCommandProvider<UpdateSQL, DevConnectionString, SingleRow = true>
    let update (connectiongString: string) (country : Country) =
      task {
        let cmd = new UpdateCountry(connectiongString)
        return! cmd.AsyncExecute(country.name.String, country.id.Int)
      }

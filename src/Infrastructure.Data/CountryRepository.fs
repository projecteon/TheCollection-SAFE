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

module CountryRepository =
    [<Literal>]
    let  QrySQL = "
        DECLARE @sName varchar(50)
        SET @sName = @name

        SELECT a.id, a.s_name, a.s_tld_code, a.s_name_nl
        FROM tcs_country a
        WHERE 1=1
          AND (LEN(@sName) = 0 OR a.s_name LIKE @sName)
    "

    type CountryQry = SqlCommandProvider<QrySQL, DevConnectionString>

    let getAll (config: DbConfig) (nameFilter: SearchParams) : Task<Country list> =
      task {
        let cmd = new CountryQry(config.ReadOnly.String)
        return cmd.Execute(nameFilter.Term |> extractSearchTerm)
        |> List.ofSeq
        |> Seq.map (fun x -> {
            id = DbId x.id
            Country.name = x.s_name |> CountryName
            Country.tlp_code = x.s_tld_code |> function
                                                | Some x -> x |> CountryCodeTLP
                                                | None -> "" |> CountryCodeTLP
            Country.name_nl = x.s_name_nl |> function
                                              | Some x -> x |> CountryName
                                              | None -> "" |> CountryName
          })
        |> Seq.toList
      }


    [<Literal>]
    let  ByIdSQL = "
        SELECT a.id, a.s_name, a.s_tld_code, a.s_name_nl
        FROM tcs_country a
        WHERE id = @id
    "

    type CountryById = SqlCommandProvider<ByIdSQL, DevConnectionString, SingleRow = true>

    let getById (config: DbConfig)  (id: int) : Task<Country option> =
      task {
        let cmd = new CountryById(config.ReadOnly.String)
        return cmd.Execute(id)
        |> function
          | Some x -> Some {
              id = DbId x.id
              Country.name = x.s_name |> CountryName
              Country.tlp_code = x.s_tld_code |> function
                                                  | Some x -> x |> CountryCodeTLP
                                                  | None -> "" |> CountryCodeTLP
              Country.name_nl = x.s_name_nl |> function
                                                | Some x -> x |> CountryName
                                                | None -> "" |> CountryName
            }
          | _ -> None
      }


    [<Literal>]
    let  InsertSQL = "
        INSERT INTO tcs_country (s_name, dt_created, dt_modified)
        OUTPUT Inserted.ID
        VALUES(@name, GETDATE(), GETDATE());
    "

    type InsertCountry = SqlCommandProvider<InsertSQL, DevConnectionString, SingleRow = true>
    let insert (config: DbConfig) (country : Country) =
      task {
        let cmd = new InsertCountry(config.Default.String)
        return cmd.AsyncExecute(country.name.String)
      }

    [<Literal>]
    let  UpdateSQL = "
        UPDATE tcs_country SET
          s_name = @s_name
          ,dt_modified = GETDATE()
        WHERE id = @id;
    "

    type UpdateCountry = SqlCommandProvider<UpdateSQL, DevConnectionString, SingleRow = true>
    let update (config: DbConfig) (country : Country) =
      task {
        let cmd = new UpdateCountry(config.Default.String)
        return! cmd.AsyncExecute(country.name.String, country.id.Int)
      }

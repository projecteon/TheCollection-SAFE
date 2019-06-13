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

module BrandRepository =

    [<Literal>]
    let  QrySQL = "
        DECLARE @sName varchar(50)
        SET @sName = @name

        SELECT a.id, a.s_name
        FROM tco_brand a
        WHERE 1=1
          AND (LEN(@sName) = 0 OR a.s_name LIKE @sName)
    "

    type BrandQry = SqlCommandProvider<QrySQL, DevConnectionString>
    let getAll (connectiongString: string) (nameFilter: SearchParams) : Task<Brand list> =
      task {
        let cmd = new BrandQry(connectiongString)
        return cmd.Execute(nameFilter.Term |> extractSearchTerm)
        |> List.ofSeq
        |> Seq.map (fun x -> {
            id = DbId x.id
            Brand.name = x.s_name |> BrandName
        })
        |> Seq.toList
      }

    [<Literal>]
    let  ByIdSQL = "
        SELECT a.id, a.s_name
        FROM tco_brand a
        WHERE id = @id
    "

    type BrandById = SqlCommandProvider<ByIdSQL, DevConnectionString, SingleRow = true>
    let getById (connectiongString: string) id =
      task {
        let cmd = new BrandById(connectiongString)
        let! brand = cmd.AsyncExecute(id)
        return match brand with
                | Some x -> Some {
                    id = DbId x.id
                    Brand.name = x.s_name |> BrandName
                  }
                | _ -> None
      }

    [<Literal>]
    let  InsertSQL = "
        INSERT INTO tco_brand (s_name)
        OUTPUT Inserted.ID
        VALUES(@name);
    "

    type InsertBrand = SqlCommandProvider<InsertSQL, DevConnectionString, SingleRow = true>
    let insert (connectiongString: string) (brand : Brand) =
      task {
        let cmd = new InsertBrand(connectiongString)
        return cmd.Execute(brand.name.String)
      }

    [<Literal>]
    let  UpdateSQL = "
        UPDATE tco_brand SET
          s_name = @s_name
        WHERE id = @id;
    "

    type UpdateBrand = SqlCommandProvider<UpdateSQL, DevConnectionString, SingleRow = true>
    let update (connectiongString: string) (brand : Brand) =
      task {
        let cmd = new UpdateBrand(connectiongString)
        return! cmd.AsyncExecute(brand.name.String, brand.id.Int)
      }


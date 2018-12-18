namespace TeaCollection.Infrastructure.MsSql

open FSharp.Data
open DbContext
open Domain.Tea
open Search
open Util

module BrandRepository =
    [<Literal>]
    let  QrySQL = "
        DECLARE @sName varchar(50)
        SET @sName = @name

        SELECT a.*
        FROM tco_brand a
        WHERE 1=1
          AND (LEN(@sName) = 0 OR a.s_name LIKE @sName)
    "

    type BrandQry = SqlCommandProvider<QrySQL, ConnectionString>

    let getAll (connectiongString: string) (nameFilter: SearchTerm) =
        let cmd = new BrandQry(connectiongString)
        cmd.Execute(nameFilter |> extractSearchTerm)
        |> List.ofSeq
        |> Seq.map (fun x -> {
            id = x.id
            name = x.s_name
        })
        |> Seq.toArray


    [<Literal>]
    let  ByIdSQL = "
        SELECT a.*
        FROM tco_brand a
        WHERE id = @id
    "

    type BrandById = SqlCommandProvider<ByIdSQL, ConnectionString, SingleRow = true>

    let getById (connectiongString: string) id =
        let cmd = new BrandById(connectiongString)
        cmd.Execute(id)
        |> function
            | Some x -> Some {
                    id = x.id
                    name = x.s_name
                }
            | _ -> None

    [<Literal>]
    let  InsertSQL = "
        INSERT INTO tco_brand (s_name)
        OUTPUT Inserted.ID
        VALUES(@name);
    "

    type InsertBrand = SqlCommandProvider<InsertSQL, ConnectionString, SingleRow = true>

    let insert (connectiongString: string) (brand : Brand) =
        let cmd = new InsertBrand(connectiongString)
        cmd.Execute(brand.name)
        |> function
            | Some x -> Some { brand with id = x }
            | _ -> None

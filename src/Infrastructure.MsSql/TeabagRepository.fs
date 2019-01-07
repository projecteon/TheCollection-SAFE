namespace TeaCollection.Infrastructure.MsSql

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open FSharp.Data
open NodaTime

open DbContext
open Domain.Tea
open Domain.Types
open Search
open Util
open Mapping

module TeabagRepository =
    [<Literal>]
    let  ByIdSQL = "
        SELECT a.*
            , dbo.xf_get_brand_text(a.ro_brand) as t_ro_brand
            , dbo.xf_get_bagtype_text(a.ro_bagtype) as t_ro_bagtype
            , dbo.xf_get_country_text(a.rs_country) as t_rs_country
            , i_total_count = COUNT(*) OVER()
          FROM tcd_teabag a
        WHERE id = @id
    "

    type TeabagById = SqlCommandProvider<ByIdSQL, ConnectionString, SingleRow = true>

    let mapByIdData (record: TeabagById.Record) = {
        id = record.id
        brand =  (mapRefValue record.ro_brand record.t_ro_brand)
        bagtype = (mapRefValue record.ro_bagtype record.t_ro_bagtype)
        country =  (mapRefValue record.rs_country record.t_rs_country)
        flavour = record.s_flavour
        hallmark = record.s_hallmark
        serie = record.s_serie
        serialnumber = record.s_serialnumber
        imageid = record.rf_image
        created = mapInstant <| record.d_created
      }

    let getById (connectiongString: string) id =
      let cmd = new TeabagById(connectiongString)
      cmd.Execute(id)
      |> function
        | Some x -> x |> mapByIdData |> Some
        | _ -> None

    // https://docs.microsoft.com/en-us/previous-versions/sql/compact/sql-server-compact-4.0/gg699618(v=sql.110)
    // https://sqlperformance.com/2015/01/t-sql-queries/pagination-with-offset-fetch
    [<Literal>]
    let  SQLQry = "
        DECLARE @json NVARCHAR(4000)
        SET @json = @jsonTerms

        SELECT a.*
            , dbo.xf_get_brand_text(a.ro_brand) as t_ro_brand
            , dbo.xf_get_bagtype_text(a.ro_bagtype) as t_ro_bagtype
            , dbo.xf_get_country_text(a.rs_country) as t_rs_country
            , i_total_count = COUNT(*) OVER()
          FROM tcd_teabag a
         WHERE a.s_search_terms = ALL (
        	SELECT
        		CASE WHEN a.s_search_terms like ('%' + value + '%')
        			THEN a.s_search_terms
        			ELSE '0'
        		END
        	FROM OPENJSON(@json))
        ORDER BY t_ro_brand, s_serie
        OFFSET @pageOffset ROWS
        FETCH NEXT @pageSize ROWS ONLY;
    "

    type TeabagQry = SqlCommandProvider<SQLQry, ConnectionString>

    let mapData (record: TeabagQry.Record) = {
        id = record.id
        brand =  (mapRefValue record.ro_brand record.t_ro_brand)
        bagtype = (mapRefValue record.ro_bagtype record.t_ro_bagtype)
        country =  (mapRefValue record.rs_country record.t_rs_country)
        flavour = record.s_flavour
        hallmark = record.s_hallmark
        serie = record.s_serie
        serialnumber = record.s_serialnumber
        imageid = record.rf_image
        created = mapInstant <| record.d_created
      }

    let mapTotalCount (record: TeabagQry.Record option) =
      match record with
      | Some x -> match x.i_total_count with
                  | Some x -> x
                  | None -> 0
      | None -> 0

    let getAll (connectiongString: string) (searchFilter: SearchTerm) page =
      let cmd = new TeabagQry(connectiongString)
      cmd.Execute((searchFilter |> extractSearchTerm3), pageSize * page, pageSize)
      // |> List.ofSeq
      |> Seq.map mapData
      |> Seq.toList

    let searchAll (connectiongString: string) (searchFilter: SearchTerm) page =
      let cmd = new TeabagQry(connectiongString)
      let result = cmd.Execute((searchFilter |> extractSearchTerm3), pageSize * page, pageSize)
      (result |> Seq.map mapData |> Seq.toList, result |> Seq.tryHead |> mapTotalCount)

    [<Literal>]
    let  SQLBrandCountQry = "
        SELECT
        	COUNT(a.ro_brand) as i_count
        	, dbo.xf_get_brand_text(a.ro_brand) as t_ro_brand
          FROM tcd_teabag a
         GROUP BY a.ro_brand
         ORDER BY i_count DESC;
    "

    type BrandCountQry = SqlCommandProvider<SQLBrandCountQry, ConnectionString>

    let brandCount (connectiongString: string) : Task<CountBy<string> list> =
      task {
        let cmd = new BrandCountQry(connectiongString)
        return cmd.Execute()
        |> List.ofSeq
        |> Seq.map (fun x -> {
            count = mapOptionalIntValue(x.i_count)
            description = mapOptionalStringValue(x.t_ro_brand)
          })
        |> Seq.toList
      }

    [<Literal>]
    let  SQLBagtypeCountQry = "
        SELECT
        	COUNT(a.ro_bagtype) as i_count
        	, dbo.xf_get_bagtype_text(a.ro_bagtype) as t_ro_bagtype
          FROM tcd_teabag a
         GROUP BY a.ro_bagtype
         ORDER BY i_count DESC
    "

    type BagtypeCountQry = SqlCommandProvider<SQLBagtypeCountQry, ConnectionString>

    let bagtypeCount (connectiongString: string) : Task<CountBy<string> list> =
      task {
        let cmd = new BagtypeCountQry(connectiongString)
        return cmd.Execute()
        |> List.ofSeq
        |> Seq.map (fun x -> {
            count = mapOptionalIntValue(x.i_count)
            description = mapOptionalStringValue(x.t_ro_bagtype)
          })
        |> Seq.toList
      }
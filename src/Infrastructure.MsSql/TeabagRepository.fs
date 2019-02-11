namespace TeaCollection.Infrastructure.MsSql

open System
open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open FSharp.Data
open NodaTime

open DbContext
open Domain.Tea
open Domain.Types
open Domain.SharedTypes
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
        id = DbId record.id
        brand =  (mapRefValue record.ro_brand record.t_ro_brand)
        bagtype = (mapRefValue record.ro_bagtype record.t_ro_bagtype)
        country =  (mapRefValue record.rs_country record.t_rs_country)
        flavour = record.s_flavour
        hallmark = record.s_hallmark
        serie = record.s_serie
        serialnumber = record.s_serialnumber
        imageid = ImageId (mapDbId record.rf_image)
        created = mapInstant <| record.d_created
      }

    let getById (connectiongString: string) id =
      task {
        let cmd = new TeabagById(connectiongString)
        let! teabag = cmd.AsyncExecute(id)
        return match teabag with
                | Some x -> x |> mapByIdData |> Some
                | _ -> None
      }

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
        id = DbId record.id
        brand =  (mapRefValue record.ro_brand record.t_ro_brand)
        bagtype = (mapRefValue record.ro_bagtype record.t_ro_bagtype)
        country =  (mapRefValue record.rs_country record.t_rs_country)
        flavour = record.s_flavour
        hallmark = record.s_hallmark
        serie = record.s_serie
        serialnumber = record.s_serialnumber
        imageid = ImageId (mapDbId record.rf_image)
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
      cmd.Execute((searchFilter |> createJsonTermArray), pageSize * page, pageSize)
      // |> List.ofSeq
      |> Seq.map mapData
      |> Seq.toList

    
    let pageNumberOrDefault queryFilter =
      match queryFilter.Page with
      | Some x -> int64 x
      | None -> int64 0

    let searchAll (connectiongString: string) (searchParams: SearchParams) =
      let cmd = new TeabagQry(connectiongString)
      let result = cmd.Execute((searchParams.Term |> createJsonTermArray), pageSize * (pageNumberOrDefault searchParams), pageSize)
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

    [<Literal>]
    let  SQLInsertedCountQry = "
        DECLARE @MIN_YEAR DATETIME;
        DECLARE @MAX_YEAR DATETIME;
        SELECT @MIN_YEAR = MIN(d_created), @MAX_YEAR = MAX(d_created) FROM tcd_teabag; 

        DECLARE @DateFrom DATETIME;
        DECLARE @DateTo DATETIME;

        SELECT
           @DateFrom = DATEADD(yy, DATEDIFF(yy, 0, @MIN_YEAR), 0),
           @DateTo = DATEADD(yy, DATEDIFF(yy, 0, @MAX_YEAR) + 1, -1);

        WITH MonthYearCalendar(date)
        AS
        ( 
        SELECT @DateFrom 
        UNION ALL
        SELECT DATEADD(month,1,MonthYearCalendar.date) FROM MonthYearCalendar WHERE MonthYearCalendar.date < DATEADD(month, -1, @DateTo)
        ), teabagCalCTE (d_inserted, i_count) as (
	        SELECT
		        d_inserted = DATEADD(MONTH, DATEDIFF(MONTH, 0, d_created), 0),
		        COUNT(a.id) as i_count
	        FROM tcd_teabag a
	        GROUP BY DATEADD(MONTH, DATEDIFF(MONTH, 0, d_created), 0)
        )

        --SELECT * FROM MonthYearCalendar OPTION (MAXRECURSION 5000);
        --SELECT  * FROM teabagCalCTE

        SELECT a.date as d_inserted, ISNULL(b.i_count, 0) AS i_count
        FROM MonthYearCalendar a
        LEFT JOIN teabagCalCTE b ON a.date = b.d_inserted
        OPTION (MAXRECURSION 5000)
    "
    
    type InsertedCountQry = SqlCommandProvider<SQLInsertedCountQry, ConnectionString>

    let insertedCount (connectiongString: string) : Task<CountBy<Instant> list> =
      task {
        let cmd = new InsertedCountQry(connectiongString)
        return cmd.Execute()
        |> List.ofSeq
        |> Seq.map (fun x -> {
            count = x.i_count
            description = mapInstant <| (mapOptionalDateTimeValue <| x.d_inserted)
          })
        |> Seq.toList
      }

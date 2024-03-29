namespace Infrastructure.Data

open System
open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open FSharp.Data
open NodaTime

open Mapping
open DbContext
open Domain.Tea
open Domain.Types
open Domain.Searchable
open Domain.SearchStringGenerator
open Domain.SharedTypes
open Search
open Util

module TeabagRepository =
    [<Literal>]
    let  InsertSQL = "
        INSERT INTO tcd_teabag
           (ro_brand
           ,ro_bagtype
           ,rs_country
           ,rf_image
           ,i_archive
           ,s_flavour
           ,s_hallmark
           ,s_serie
           ,s_serialnumber
           ,s_search_terms
           ,d_created
           ,dt_created
           ,dt_modified)
        OUTPUT Inserted.id
        VALUES
           (@ro_brand
           ,@ro_bagtype
           ,@rs_country
           ,@rf_image
           ,(SELECT MAX(i_archive) + 1 FROM tcd_teabag)
           ,@s_flavour
           ,@s_hallmark
           ,@s_serie
           ,@s_serialnumber
           ,@s_search_terms
           ,GETDATE()
           ,GETDATE()
           ,GETDATE())
    "

    type InsertTeabag = SqlCommandProvider<InsertSQL, DevConnectionString, SingleRow = true>

    let insert (config: DbConfig) (teabag: Domain.Tea.Teabag) =
      task {
        let cmd = new InsertTeabag(config.Default.String)
        let searchString = teabag |> getSearchStrings |> String.concat " " |> GenerateSearchString |> String.concat " "
        let! id = cmd.AsyncExecute( teabag.brand.id.Int
                                    , teabag.bagtype.id.Int
                                    , teabag.country |> ToDbValue.RefValueOption |> toDbIntValue
                                    , teabag.imageid.Option |> ToDbValue.DbIdOption|> toDbIntValue
                                    , teabag.flavour.String
                                    , teabag.hallmark |> toDbStringValue
                                    , teabag.serie |> toDbStringValue
                                    , teabag.serialnumber  |> toDbStringValue
                                    , searchString)
        return id
      }

    [<Literal>]
    let  UpdateSQL = "
        UPDATE tcd_teabag SET
           ro_brand = @ro_brand
           ,ro_bagtype = @ro_bagtype
           ,rs_country = @rs_country
           ,rf_image = @rf_image
           ,s_flavour = @s_flavour
           ,s_hallmark = @s_hallmark
           ,s_serie = @s_serie
           ,s_serialnumber = @s_serialnumber
           ,s_search_terms = @s_search_terms
           ,dt_modified = GETDATE()
        WHERE id = @id
    "

    type UpdateTeabag = SqlCommandProvider<UpdateSQL, DevConnectionString, SingleRow = true>

    let update (config: DbConfig) (teabag: Domain.Tea.Teabag) =
      task {
        let cmd = new UpdateTeabag(config.Default.String)
        let searchString = teabag |> getSearchStrings |> String.concat " " |> GenerateSearchString |> String.concat " "
        printf "Updating %s" searchString
        let! id = cmd.AsyncExecute( teabag.brand.id.Int
                                    , teabag.bagtype.id.Int
                                    , teabag.country |> ToDbValue.RefValueOption |> toDbIntValue
                                    , teabag.imageid.Option |> ToDbValue.DbIdOption|> toDbIntValue
                                    , teabag.flavour.String
                                    , teabag.hallmark |> toDbStringValue
                                    , teabag.serie |> toDbStringValue
                                    , teabag.serialnumber  |> toDbStringValue
                                    , searchString
                                    , teabag.id.Int)
        return teabag.id.Int
      }

    [<Literal>]
    let  ByIdSQL = "
        SELECT a.*
            , dbo.xf_get_brand_text(a.ro_brand) as t_ro_brand
            , dbo.xf_get_bagtype_text(a.ro_bagtype) as t_ro_bagtype
            , dbo.xf_get_country_text(a.rs_country) as t_rs_country
          FROM tcd_teabag a
        WHERE id = @id
    "

    type TeabagById = SqlCommandProvider<ByIdSQL, DevConnectionString, SingleRow = true>

    let mapByIdData (record: TeabagById.Record) = {
        id = DbId record.id
        brand =  {id = DbId record.ro_brand; description = record.t_ro_brand.Value}
        bagtype = {id = DbId record.ro_bagtype; description = record.t_ro_bagtype.Value}
        country =  (toRefValue record.rs_country record.t_rs_country)
        flavour = record.s_flavour |> Flavour
        hallmark = record.s_hallmark |> Hallmark.From
        serie = record.s_serie |> Serie.From
        serialnumber = record.s_serialnumber |> SerialNumber.From
        archiveNumber = record.i_archive
        imageid = ImageId (toDbId record.rf_image)
        created = (toInstant <| record.d_created) |> CreatedDate
      }

    let getById (config: DbConfig) id =
      task {
        let cmd = new TeabagById(config.ReadOnly.String)
        let! teabag = cmd.AsyncExecute(id)
        return match teabag with
                | Some x -> x |> mapByIdData |> Some
                | _ -> None
      }

    // https://docs.microsoft.com/en-us/previous-versions/sql/compact/sql-server-compact-4.0/gg699618(v=sql.110)
    // https://sqlperformance.com/2015/01/t-sql-queries/pagination-with-offset-fetch
    // https://stackoverflow.com/questions/20475663/fulltext-search-with-contains-on-multiple-columns-and-predicate-and
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
        ORDER BY t_ro_brand, s_serie, s_hallmark, t_ro_bagtype, s_flavour
        OFFSET @pageOffset ROWS
        FETCH NEXT @pageSize ROWS ONLY;
    "

    type TeabagQry = SqlCommandProvider<SQLQry, DevConnectionString>

    let mapData (record: TeabagQry.Record) = {
        id = DbId record.id
        brand =  {id = DbId record.ro_brand; description = record.t_ro_brand.Value}
        bagtype = {id = DbId record.ro_bagtype; description = record.t_ro_bagtype.Value}
        country =  (toRefValue record.rs_country record.t_rs_country)
        flavour = record.s_flavour |> Flavour
        hallmark = record.s_hallmark |> Hallmark.From
        serie = record.s_serie |> Serie.From
        serialnumber = record.s_serialnumber |> SerialNumber.From
        archiveNumber = record.i_archive
        imageid = ImageId (toDbId record.rf_image)
        created = (toInstant <| record.d_created) |> CreatedDate
      }

    let mapTotalCount (record: TeabagQry.Record option) =
      match record with
      | Some x -> match x.i_total_count with
                  | Some x -> x
                  | None -> 0
      | None -> 0

    let getAll (config: DbConfig) (searchFilter: SearchTerm) page =
      let cmd = new TeabagQry(config.ReadOnly.String)
      cmd.Execute((searchFilter |> createJsonTermArray), config.PageSize.pageStart(page), config.PageSize.int64)
      // |> List.ofSeq
      |> Seq.map mapData
      |> Seq.toList


    let pageNumberOrDefault queryFilter =
      match queryFilter.Page with
      | Some x -> int64 x
      | None -> int64 0

    let searchAll (config: DbConfig) (searchParams: SearchParams) =
      let cmd = new TeabagQry(config.ReadOnly.String)
      let result = cmd.Execute((searchParams.Term |> createJsonTermArray), config.PageSize.pageStart(pageNumberOrDefault searchParams), config.PageSize.int64) |> Seq.toList // Make sure we only do one request to DB
      (result |> List.map mapData, result |> List.tryHead |> mapTotalCount)


    [<Literal>]
    let  SQLBrandCountQry = "
        SELECT
        	COUNT(a.ro_brand) as i_count
        	, dbo.xf_get_brand_text(a.ro_brand) as t_ro_brand
          FROM tcd_teabag a
         GROUP BY a.ro_brand
         ORDER BY i_count DESC;
    "

    type BrandCountQry = SqlCommandProvider<SQLBrandCountQry, DevConnectionString>

    let brandCount (config: DbConfig) (): Task<CountBy<string> list> =
      task {
        let cmd = new BrandCountQry(config.ReadOnly.String)
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

    type BagtypeCountQry = SqlCommandProvider<SQLBagtypeCountQry, DevConnectionString>

    let bagtypeCount (config: DbConfig) (): Task<CountBy<string> list> =
      task {
        let cmd = new BagtypeCountQry(config.ReadOnly.String)
        return cmd.Execute()
        |> List.ofSeq
        |> Seq.map (fun x -> {
            count = mapOptionalIntValue(x.i_count)
            description = mapOptionalStringValue(x.t_ro_bagtype)
          })
        |> Seq.toList
      }

    [<Literal>]
    let  SQLCountryTLDCountQry = "
        SELECT
        	COUNT(a.rs_country) as i_count
        	, dbo.xf_get_country_tlp_code(a.rs_country) as t_rs_country_tlp_code
          FROM tcd_teabag a
         GROUP BY a.rs_country
         ORDER BY i_count DESC
    "

    type CountryTLDCountQry = SqlCommandProvider<SQLCountryTLDCountQry, DevConnectionString>

    let countryTLDCount (config: DbConfig) (): Task<CountBy<string> list> =
      task {
        let cmd = new CountryTLDCountQry(config.ReadOnly.String)
        return cmd.Execute()
        |> List.ofSeq
        |> Seq.map (fun x -> {
            count = mapOptionalIntValue(x.i_count)
            description = mapOptionalStringValue(x.t_rs_country_tlp_code)
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

    type InsertedCountQry = SqlCommandProvider<SQLInsertedCountQry, DevConnectionString>

    let insertedCount (config: DbConfig): Task<CountBy<Instant> list> =
      task {
        let cmd = new InsertedCountQry(config.ReadOnly.String)
        return cmd.Execute()
        |> List.ofSeq
        |> Seq.map (fun x -> {
            count = x.i_count
            description = toInstant <| (mapOptionalDateTimeValue <| x.d_inserted)
          })
        |> Seq.toList
      }

    [<Literal>]
    let  SQLStatistcsQry = "
         SELECT
          	COUNT(*) as teabagcount,
          	COUNT(DISTINCT ro_brand) as brandcount,
          	COUNT(DISTINCT rs_country) as countrycount
          FROM tcd_teabag
      "
    type StatistcsQry = SqlCommandProvider<SQLStatistcsQry, DevConnectionString, SingleRow = true>

    let mapStatiscs (record: StatistcsQry.Record) = {
        TeabagCount = mapOptionalIntValue record.teabagcount
        BrandCount = mapOptionalIntValue record.brandcount
        CountryCount = mapOptionalIntValue record.countrycount
      }

    let statistics (config: DbConfig) (): Task<Statistics> =
      task {
        let cmd = new StatistcsQry(config.ReadOnly.String)
        let! statistcs = cmd.AsyncExecute()
        return match statistcs with
                | Some x -> x |> mapStatiscs
                | _ -> {
                          TeabagCount = 0
                          BrandCount = 0
                          CountryCount = 0
                        }
      }

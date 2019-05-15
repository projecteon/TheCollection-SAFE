module Server.Api.Generic

open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2
open Giraffe

open TeaCollection.Infrastructure.MsSql
open Domain.SharedTypes
open TeaCollection.Infrastructure.MsSql.Search

//[<CLIMutable>]
//type QueryParams = {
//    Term: SearchTerm
//    Page: int option
//}

type SearchResult<'a> = {
  data: 'a
  count: int
}

let pageNumberOrDefault queryFilter =
  match queryFilter.Page with
  | Some x -> int64 x
  | None -> int64 0

let handleGetAllSearch (getAll: ('a -> Task<'b list> )) next (ctx: HttpContext) =
  task {
    let filter = ctx.BindQueryString<'a>()
    let! data = getAll(filter)
    return! Successful.OK data next ctx
  }

let handleGetAllWithPaging (getAll: ('a -> 'b list*int )) (transform: ('b list -> 'c list )) next (ctx: HttpContext) =
  task {
    let filter = ctx.BindQueryString<'a>()
    let (data, count) = getAll(filter)
    return! Successful.OK {data=(transform data); count=count} next ctx
  }

let handleGet (get: ('a -> Task<Option<'b>> )) (transform: ('b -> 'c )) id next ctx =
  task {
    let! data = get id
    match data with
    | Some x -> return! Successful.OK (transform x) next ctx
    | _ -> return! Successful.NO_CONTENT next ctx
  }

let handleGetAll (get: Task<'a>) next ctx =
  task {
    let! data = get
    return! Successful.OK data next ctx
  }

let handleGetTransformAll (get: Task<'a>) (transform: ('a -> 'b )) next ctx =
  task {
    let! data = get
    return! Successful.OK (transform data) next ctx
  }

let handleSearchAndTransform (search: ('a -> Task<'b> )) (transform: ('b -> 'c )) next (ctx: HttpContext) =
  task {
    let filter = ctx.BindQueryString<'a>()
    let! data = search filter
    return! Successful.OK (transform data) next ctx
  }


// https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#binding-query-strings
// https://github.com/giraffe-fsharp/Giraffe/issues/121
let parsingErrorHandler (err : string) = RequestErrors.BAD_REQUEST err
let handleGetAllQuery (getAll: ('a -> Task<'b list> )) next (ctx: HttpContext) =
  task {
    let filter = ctx.TryBindQueryString<'a>()

    match filter with
    | Ok filter ->
      let! data = getAll(filter)
      return! Successful.OK data next ctx
    | Error err -> return! RequestErrors.BAD_REQUEST err next ctx
  }

let handlePost (insert: ('a -> Task<'b> )) (transform: ('c -> 'a )) (validate: ('a -> Result<'a, string>)) next (ctx: HttpContext) =
  task {
    let! postModel = ctx.BindJsonAsync<'c>()
    let data = transform postModel
    let validatedModel = validate data
    match validatedModel with
    | Success model ->
      let! result = insert model
      return! Successful.OK result next ctx
    | Failure err -> return! RequestErrors.BAD_REQUEST err next ctx
  }

let handlePut (get: ('a -> Task<Option<'b>> )) (update: ('b -> Task<'a>)) (transform: ('b*'c -> 'b )) (validate: ('b -> Result<'b, string>)) id next (ctx: HttpContext) =
  task {
    let! currentData = get id
    match currentData with
    | None -> return! RequestErrors.NOT_FOUND (sprintf "Entity with id: %O not found" id) next ctx
    | Some x ->
      let! postModel = ctx.BindJsonAsync<'c>()
      let data = transform (x, postModel)
      let validatedModel = validate data
      match validatedModel with
      | Success model ->
        let! result = update (model)
        return! Successful.OK result next ctx
      | Failure err -> return! RequestErrors.BAD_REQUEST err next ctx
  }

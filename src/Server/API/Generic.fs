module API.Generic

open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2
open Giraffe

open TeaCollection.Infrastructure.MsSql
open Search

[<CLIMutable>]
type QueryParams = {
    Term: SearchTerm
    Page: int option
}

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

let handleGetAllWithPaging (getAll: ('a -> 'b list*int )) next (ctx: HttpContext) =
  task {
    let filter = ctx.BindQueryString<'a>()
    let (data, count) = getAll(filter)
    return! Successful.OK {data=data; count=count} next ctx
  }

let handleGet (get: ('a -> Task<Option<'b>> )) id next ctx =
  task {
    let! data = get id
    match data with
    | Some x -> return! Successful.OK x next ctx
    | _ -> return! Successful.NO_CONTENT next ctx
  }

let handleGetAll (get: Task<'a>) next ctx =
  task {
    let! data = get
    return! Successful.OK data next ctx
  }

module API.Generic

open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2
open Giraffe

open TeaCollection.Infrastructure.MsSql
open Search

[<CLIMutable>]
type QuertyFilter = {
    Term: SearchTerm
    Page: int option
}

type SearchResult<'a> = {
  data: 'a
  count: int
}

let pageOrDefault queryFilter =
  match queryFilter.Page with
  | Some x -> int64 x
  | None -> int64 0

let handleGetAll (getAll: (SearchTerm -> Task<'a list> )) next (ctx: HttpContext) =
  task {
    let filter = ctx.BindQueryString<QuertyFilter>()
    let! data = getAll(filter.Term)
    return! Successful.OK data next ctx
  }

let handleGetAll2 (getAll: (SearchTerm -> int64 -> 'a list )) next (ctx: HttpContext) =
  task {
    let filter = ctx.BindQueryString<QuertyFilter>()
    let data = getAll(filter.Term) (pageOrDefault filter)
    return! Successful.OK data next ctx
  }

let handleGetAll3 (getAll: (SearchTerm -> int64 -> 'a list*int )) next (ctx: HttpContext) =
  task {
    let filter = ctx.BindQueryString<QuertyFilter>()
    let (data, count) = getAll(filter.Term) (pageOrDefault filter)
    return! Successful.OK {data=data; count=count} next ctx
  }

let handleGet (get: ('a -> Task<Option<'b>> )) id next ctx =
  task {
    let! data = get id
    match data with
    | Some x -> return! Successful.OK x next ctx
    | _ -> return! Successful.NO_CONTENT next ctx
  }

let handleGet2 (get: ('a -> Option<'b> )) id next ctx =
  task {
    let data = get id
    match data with
    | Some x -> return! Successful.OK x next ctx
    | _ -> return! Successful.NO_CONTENT next ctx
  }

let handleGetNoParam (get: Task<'a>) next ctx =
  task {
    let! data = get
    return! Successful.OK data next ctx
  }

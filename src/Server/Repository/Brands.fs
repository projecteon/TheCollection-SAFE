module Repository.Brands

open System.Linq
open System.Threading.Tasks

open FSharp.Control.Tasks.V2

open Domain.Tea

open TeaCollection.Infrastructure.MsSql
open Search

let db = [
    { id = 1; description = "Liption" }
    { id = 2; description = "Teekanne" }
    { id = 3; description = "Ahmad" }
    { id = 4; description = "Coop" }
    { id = 5; description = "Twinnings" }
]

let getAll(filter: SearchTerm) : Task<RefValue list> = task {
    return
        match filter with
        | Some x -> db |> List.filter (fun y -> y.description.Contains(x))
        | None -> db
}

let get id : Task<Option<RefValue>> = task { return Some <| db.First() }
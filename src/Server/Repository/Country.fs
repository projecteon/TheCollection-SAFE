
module Repository.Country

open System.Linq
open System.Threading.Tasks

open FSharp.Control.Tasks.V2

open Domain.Tea

open TeaCollection.Infrastructure.MsSql
open Search
let db = [
    { id = 1; description = "Netherlands" }
    { id = 2; description = "Norway" }
    { id = 3; description = "France" }
]

let getAll(filter: SearchTerm) : Task<RefValue list> = task {
    return
        match filter with
        | Some x -> db |> List.filter (fun y -> y.description.Contains(x))
        | None -> db
}

let get id : Task<Option<RefValue>> = task { return Some <| db.First() }
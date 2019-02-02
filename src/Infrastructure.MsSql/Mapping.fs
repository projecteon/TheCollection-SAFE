namespace TeaCollection.Infrastructure.MsSql

open NodaTime
open System

open Domain.SharedTypes
open Domain.Tea

module Mapping =
  let mapRefValue id text : RefValue option =
    match  id, text with
    | Some id, Some text -> Some {id = DbId id; description = text}
    | _, _ -> None

  let mapDbId id : DbId option =
    match  id with
    | Some id -> Some (DbId id)
    | _ -> None

  let mapInstant (utcDateTime: DateTime) : Instant =
    utcDateTime.ToUniversalTime()
    |> Instant.FromDateTimeUtc

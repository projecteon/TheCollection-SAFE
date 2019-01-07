namespace TeaCollection.Infrastructure.MsSql

open NodaTime
open System

open Domain.Tea

module Mapping =
  let mapRefValue id text : RefValue option =
    match  id, text with
    | Some id, Some text -> Some {id = id; description = text}
    | _, _ -> None

  let mapInstant (utcDateTime: DateTime) : Instant =
    utcDateTime.ToUniversalTime()
    |> Instant.FromDateTimeUtc

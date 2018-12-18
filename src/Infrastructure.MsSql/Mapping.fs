namespace TeaCollection.Infrastructure.MsSql

open Domain.Tea
open NodaTime

module Mapping =
  let mapRefValue id text : RefValue option =
    match  id, text with
    | Some id, Some text -> Some {id = id; description = text}
    | _, _ -> None

  let mapInstant utcDateTime : Instant =
    utcDateTime
    |> Instant.FromDateTimeUtc

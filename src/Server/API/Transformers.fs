module Transformers

open NodaTime
open Domain.SharedTypes
open Services.Dtos


let transform (data: CountBy<Instant> list): CountBy<UtcDateTimeString> list =
  data
  |> List.map (fun x -> { count = x.count; description = (NodaTimeHelper.toJsonString <| x.description) })
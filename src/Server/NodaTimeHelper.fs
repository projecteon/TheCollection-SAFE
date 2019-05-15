module NodaTimeHelper

open NodaTime
open NodaTime.Text
open Server.Api.Dtos

// https://stackoverflow.com/questions/23250129/how-to-parse-an-iso-string-value-to-a-nodatime-instant
// https://github.com/nodatime/nodatime.serialization/blob/edf88349e54ab63f06266a9c864edfa1ba6503cb/src/NodaTime.Serialization.JsonNet/NodaConverters.cs
// https://github.com/nodatime/nodatime.serialization/blob/4e0ff1385651ad73aceda137dc2a7d19db598804/src/NodaTime.Serialization.JsonNet/NodaPatternConverter.cs
let toJsonString (instant: Instant) =
  UtcDateTimeString <| InstantPattern.ExtendedIso.Format(instant)

let fromJsonString (utcDateTimeString: UtcDateTimeString) =
  InstantPattern.ExtendedIso.Parse(utcDateTimeString.String).Value
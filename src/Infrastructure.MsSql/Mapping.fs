namespace TeaCollection.Infrastructure.MsSql

open NodaTime
open System

open Domain.SharedTypes
open Domain.Tea

module Mapping =
  let toRefValue id text : RefValue option =
    match  id, text with
    | Some id, Some text -> Some {id = DbId id; description = text}
    | _, _ -> None

  let toDbId id : DbId option =
    match  id with
    | Some id -> Some (DbId id)
    | _ -> None

  let toInstant (utcDateTime: DateTime) : Instant =
    utcDateTime.ToUniversalTime()
    |> Instant.FromDateTimeUtc

  //let stringOptionToDbValue (value: string option): string =
  //  match value with
  //  | Some x -> x
  //  | None -> null

  let inline toDbStringValue (x : ^T option) =
    match x with
    | Some x -> (^T : (member String : string) (x))
    | None -> null
    
  let inline toDbDateTimeValue (x : ^T option) =
    match x with
    | Some x -> (^T : (member DateTime : DateTime) (x))
    | None -> DateTime.MinValue
    
  let inline instantToDbDateTimeValue (x : ^T option) =
    match x with
    | Some x -> (^T : (member instant : Instant) (x)).ToDateTimeUtc()
    | None -> Instant.MinValue.ToDateTimeUtc()

  type ToDbValue =
  | DbId of DbId
  | DbIdOption of DbId option
  | RefValue of RefValue
  | RefValueOption of RefValue option

  let rec toDbIntValue (value: ToDbValue) =
    match value with
    | DbId y -> y.Int
    | DbIdOption y ->
      match y with
      | Some x -> x |> ToDbValue.DbId |> toDbIntValue
      | None -> 0
    | RefValue y -> y.id |> ToDbValue.DbId |> toDbIntValue
    | RefValueOption y ->
      match y with
      | Some x -> x |> ToDbValue.RefValue |> toDbIntValue
      | None -> 0

  //let inline refValueOptionToDbValue (x : RefValue option) =
  //  match x with
  //  | Some x -> x.id.Int
  //  | None -> 0

  //let inline dbIdOptionToDbValue (x : DbId option) =
  //  match x with
  //  | Some x -> x.Int
  //  | None -> 0

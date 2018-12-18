namespace TeaCollection.Infrastructure.MsSql

module Search =
    type SearchTerm = string option
    type SearchTerms = string list

module Seq =
  let tryHead (seq: seq<'a>) =
    match (Seq.length seq) with
    | 0 -> None
    | _ -> Some (Seq.head seq)
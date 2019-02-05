namespace TeaCollection.Infrastructure.MsSql

module Search =
    type SearchTerm = string option
    type SearchTerms = string list

    [<CLIMutable>]
    type SearchParams = {
        Term: SearchTerm
        Page: int option
    }

module Seq =
  let tryHead (seq: seq<'a>) =
    match (Seq.length seq) with
    | 0 -> None
    | _ -> Some (Seq.head seq)
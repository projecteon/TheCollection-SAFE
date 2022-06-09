namespace Infrastructure.Data

open Domain.SharedTypes

module Search =
    type SearchTerm = string option
    type SearchTerms = string list

    [<CLIMutable>]
    type SearchParams = {
        Term: SearchTerm
        Page: int option
    }

    [<CLIMutable>]
    type RefValueSearchParams = {
        Term: SearchTerm
        RefValueType: RefValueTypes
    }

module Seq =
  let tryHead (seq: seq<'a>) =
    match (Seq.length seq) with
    | 0 -> None
    | _ -> Some (Seq.head seq)
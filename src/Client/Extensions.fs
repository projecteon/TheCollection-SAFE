module Client.Extensions
let canEdit (user: Server.Api.Dtos.UserData option) =
  user.IsSome && user.Value.UserName.String <> "mcw201@hotmail.com"

module Seq =
    // http://www.fssnip.net/es/title/Split-A-Seq-Into-Chunks
    // Returns a sequence that yields chunks of length n.
    // Each chunk is returned as a list.
    let split length (xs: seq<'T>) =
        let rec loop xs =
            [
                yield Seq.truncate length xs |> Seq.toList
                match Seq.length xs <= length with
                | false -> yield! loop (Seq.skip length xs)
                | true -> ()
            ]
        loop xs

    let tryHead (seq: seq<'a>) =
      match (Seq.length seq) with
      | 0 -> None
      | _ -> Some (Seq.head seq)


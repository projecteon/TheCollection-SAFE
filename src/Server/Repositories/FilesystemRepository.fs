namespace Server

open System.IO;
open FSharp.Control.Tasks.V2

module ImageFilesystemRepository =
  [<Literal>]
  let Path = @"C:\src\Theedatabase\Afbeeldingen Zakjes\"

  let get id =
    sprintf "%s%i.jpg" Path id
    |> File.ReadAllBytes

  let getAsync id = task { return get id }
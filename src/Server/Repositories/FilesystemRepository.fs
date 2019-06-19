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

  let insertAsync (filename, stream: Stream) = async {
    use fileStream = new FileStream(Path, FileMode.Create)
    do! stream.CopyToAsync(fileStream) |> Async.AwaitIAsyncResult |> Async.Ignore
    return (new System.Uri(Path), filename)
  }
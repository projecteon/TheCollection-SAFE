namespace Server

open System.IO;
open Azure.Storage.Blobs;

// https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
module AzureBlobRepository =
  [<Literal>]
  let ImagesContainerReferance = "images"

  [<Literal>]
  let ThumbnailsContainerReferance = "thumbnails"

  let private getContainer (connectionString: string) (containerReferance: string) = async {
      let container = BlobContainerClient(connectionString, containerReferance)
      do! container.CreateIfNotExistsAsync() |> Async.AwaitTask |> Async.Ignore
      return container
  }

  let getAsync2 (connectionString: string) containerReferance filename = async {
      let! container = getContainer connectionString containerReferance
      let blob = container.GetBlobClient(filename)
      use memoryStream = new MemoryStream()
      do! blob.DownloadToAsync(memoryStream) |> Async.AwaitTask |> Async.Ignore
      memoryStream.Position <- int64 0 // https://stackoverflow.com/questions/51247073/returning-an-image-with-memorystream-and-webapi
      return memoryStream.ToArray();
  }

  // https://stackoverflow.com/questions/14938606/how-do-i-upload-to-azure-blob-storage-without-overwriting
  let insertAsync2 (connectionString: string) containerReferance (filename: string, stream: Stream) = async {
      let! container = getContainer connectionString containerReferance
      let blob = container.GetBlobClient(filename)
      do! blob.UploadAsync(stream, false) |> Async.AwaitTask |> Async.Ignore
      return (blob.Uri, filename);
  }

namespace Server

open System.IO;
open Azure.Storage.Blobs;

// https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
module AzureBlobRepository =
  [<Literal>]
  let ImagesContainerReferance = "images"

  [<Literal>]
  let ThumbnailsContainerReferance = "thumbnails"

  // Azure.Storage.Blobs pinned to 12.26.x (see paket.dependencies): its default
  // wire version 2025-11-05 is the newest Azurite (<= 3.35) accepts, so no
  // per-client ServiceVersion override is needed. Newer SDKs default to a wire
  // version Azurite rejects ("The API version ... is not supported by Azurite").
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

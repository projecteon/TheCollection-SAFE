namespace Server

open System.IO;
open Microsoft.WindowsAzure.Storage;
open Microsoft.WindowsAzure.Storage.Blob;
open FSharp.Control.Tasks.V2
open TeaCollection.Infrastructure.MsSql

// https://docs.microsoft.com/en-us/dotnet/fsharp/using-fsharp-on-azure/blob-storage
// http://thorium.github.io/FSharpAzure/2-AzureStorage/AzureStorageEng.html
module AzureBlobRepository =
  [<Literal>]
  let ImagesContainerReferance = "images"

  [<Literal>]
  let ThumbnailsContainerReferance = "thumbnails"

  let CreateContainerIfNotExistsAsync (container: CloudBlobContainer) = async {
      do! container.CreateIfNotExistsAsync() |> Async.AwaitIAsyncResult |> Async.Ignore
  }

  let createContainer (config: AzureStorage.StorageAccount) containerReferance = async {
      let storageAccount = CloudStorageAccount.Parse(config.ConnectionString)
      let blobClient = storageAccount.CreateCloudBlobClient()
      let container = blobClient.GetContainerReference(containerReferance)
      do! CreateContainerIfNotExistsAsync container
      return container
  }

  // https://stackoverflow.com/questions/8022909/how-to-async-awaittask-on-plain-task-not-taskt
  let mapBlop (blockBlob: CloudBlockBlob) = async {
      use memoryStream = new MemoryStream()
      do! blockBlob.DownloadToStreamAsync(memoryStream) |> Async.AwaitIAsyncResult |> Async.Ignore
      memoryStream.Position <- int64 0 // https://stackoverflow.com/questions/51247073/returning-an-image-with-memorystream-and-webapi
      return memoryStream.ToArray();
  }

  let getAsync (container: CloudBlobContainer) filename = async {
      let blockBlob = container.GetBlockBlobReference(filename)
      return! mapBlop blockBlob
  }

  let insertAsync (container: CloudBlobContainer) stream id = async {
    let blockBlob = container.GetBlockBlobReference(id)
    do! blockBlob.UploadFromStreamAsync(stream) |> Async.AwaitIAsyncResult |> Async.Ignore
    return blockBlob.Uri;
  }
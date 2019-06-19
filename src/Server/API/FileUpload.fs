namespace Api

module FileUpload =
  open Microsoft.AspNetCore.Http
  open System.IO
  open FSharp.Control.Tasks.V2
  open Giraffe
  
  open Domain.SharedTypes
  open System.Threading.Tasks
  open Domain.Types

  // https://theburningmonk.com/2012/10/f-helper-functions-to-convert-between-asyncunit-and-task/
  let inline startAsPlainTask (work : Async<'a>) = System.Threading.Tasks.Task.Factory.StartNew(fun () -> work |> Async.RunSynchronously)

  // https://stackoverflow.com/questions/39322085/how-to-save-iformfile-to-disk
  let openSingle (file: IFormFile) =
    (file.FileName, file.OpenReadStream())

  //let uploadSingle (file: IFormFile) =
  //  task {
  //    let filePath = sprintf "c:\\temp\\%s" file.FileName
  //    use fileStream = new FileStream(filePath, FileMode.Create)
  //    do! file.CopyToAsync(fileStream) 
  //    return (filePath, file.FileName)
  //  }

  let uploadFiles (fileRepository: Domain.Tea.File -> Task<DbId option>) (blobRepository: string*Stream -> Async<System.Uri*string>) (files: IFormFileCollection) =
    task {
      if files.Count > 1 then
        return Failure "Only supports uploading one file at a time"
      else 
        let! (uri, filename) = files.[0] |> openSingle |> blobRepository
        let! id = fileRepository { id = DbId.Empty; importId = 0; uri = uri.AbsoluteUri; filename = filename; created = CreatedDate.Now; modified = ModifiedDate.Now }
        return Success id
    }

  let thumbnailHandler (getById: int -> Task<Domain.Tea.File option>) (getFile: string -> Async<byte[]>) imageId : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
      task {
        //let! bytes = ImageFilesystemRepository.getAsync imageId
        let! file = getById imageId
        match file with
        | None -> return! (RequestErrors.NOT_FOUND (sprintf "%i" imageId)) next ctx
        | Some x ->
          let! bytes = getFile x.filename
          return! ctx.WriteBytesAsync bytes
      }

  let fileUploadHandler (insertFile: Domain.Tea.File -> Task<DbId option>) (blobRepository: string*Stream -> Async<System.Uri*string>) =
    fun (next : HttpFunc) (ctx : HttpContext) ->
      task {
        let formFeature = ctx.Features.Get<Features.IFormFeature>()
        let! form = formFeature.ReadFormAsync System.Threading.CancellationToken.None
        let! result = form.Files |> (uploadFiles insertFile blobRepository) 
        match result with
        | Domain.SharedTypes.Result.Success x -> return! (Successful.OK (Domain.SharedTypes.ImageId x)) next ctx
        | Domain.SharedTypes.Result.Failure y -> return! (RequestErrors.BAD_REQUEST y) next ctx
      }

  // https://codereview.stackexchange.com/questions/90569/saving-an-uploaded-file-and-returning-form-data
  //let singleFile 
  //  (req : HttpRequestMessage) 
  //  dirName 
  //  typeDir 
  //  (fileType : string) 
  //  userName 
  //  clearDir 
  //  deleteExistingFile =

  //  let isValidFile (fileData: seq<MultipartFileData>) =
  //      let nameLength =
  //          [
  //              for x in fileData ->
  //                  x.Headers.ContentDisposition.FileName.Length
  //          ]

  //      let fileTypeList =
  //          [
  //              for x in fileData ->
  //                  x.Headers.ContentType.MediaType.[..fileType.Length-1]
  //          ]

  //      let nameLengthNotValid = nameLength |> List.exists ((>=) 2)
  //      let fileTypeNotValid = fileTypeList |> List.exists ((<>) fileType)

  //      not (nameLengthNotValid || fileTypeNotValid)

  //  async {
  //      if not (req.Content.IsMimeMultipartContent()) then
  //          return req.CreateResponse HttpStatusCode.UnsupportedMediaType
  //      else 

  //          let tempFileFolder = 
  //              HttpContext.Current.Server.MapPath @"~/TempFileUploads"

  //          Directory.CreateDirectory tempFileFolder |> ignore

  //          let provider = new MultipartFormDataStreamProvider(tempFileFolder)

  //          try
  //              let dirPath = 
  //                  let path = 
  //                      @"~/Uploads/" + typeDir + @"/" + dirName 
  //                      + if String.IsNullOrWhiteSpace userName then "" else @"/" + userName
  //                  HttpContext.Current.Server.MapPath path

  //              if clearDir && Directory.Exists dirPath then 
  //                  Directory.Delete(dirPath, true)

  //              // create the final directory path
  //              Directory.CreateDirectory dirPath |> ignore

  //              let! readToProvider = 
  //                  req.Content.ReadAsMultipartAsync provider |> Async.AwaitIAsyncResult

  //              let fileNameAndTypeIsValid = isValidFile provider.FileData && provider.FormData.Count > 0

  //              if fileNameAndTypeIsValid then
  //                  let fileInfo =
  //                      new FileInfo(
  //                          provider.FileData.ElementAt(0).LocalFileName
  //                      )

  //                  let ext = 
  //                      Path.GetExtension(
  //                          provider.FileData.ElementAt(0)
  //                              .Headers.ContentDisposition.FileName
  //                              .Replace(@"""", String.Empty)
  //                      )

  //                  File.Move(fileInfo.FullName, Path.Combine(dirPath, fileInfo.Name + ext))

  //                  let fileUrl = 
  //                      @"/Uploads/" + typeDir + @"/" + dirName + @"/"  
  //                      + if String.IsNullOrWhiteSpace userName then "" else userName + @"/" 
  //                      + fileInfo.Name + ext

  //                  provider.FormData.Add("FileUrl", fileUrl)

  //                  if deleteExistingFile then
  //                      for x in provider.FormData.GetValues("ExistingPath") do
  //                          File.Delete (System.Web.Hosting.HostingEnvironment.MapPath(x))

  //                  return req.CreateResponse(HttpStatusCode.OK, provider.FormData)
  //              else
  //                  return req.CreateResponse HttpStatusCode.InternalServerError
  //          with
  //              | _ -> return req.CreateResponse HttpStatusCode.InternalServerError
  //  }

namespace Domain

module SharedTypes =
  type DbId = DbId of int
    with
    member this.Int = let (DbId i) = this in i
    static member Empty = DbId 0

  type ImageId = ImageId of DbId option
    with
    member this.Option = let (ImageId o) = this in o
    static member Empty = ImageId None

  type EmailAddress = EmailAddress of string
    with
    member this.String = let (EmailAddress s) = this in s
    static member Empty = EmailAddress ""

  type Password = Password of string
    with
    member this.String = let (Password s) = this in s
    static member Empty = Password ""
  
  type RefreshToken = RefreshToken of string
    with
    member this.String = let (RefreshToken s) = this in s

  type Flavour = Flavour of string
    with
    member this.String = let (Flavour s) = this in s

  type Hallmark = Hallmark of string
    with
    member this.String = let (Hallmark s) = this in s
    static member From (value: string option) = match value with | None -> None | Some x -> x |> Hallmark |> Some

  type Serie = Serie of string
    with
    member this.String = let (Serie s) = this in s
    static member From (value: string option) = match value with | None -> None | Some x -> x |> Serie |> Some

  type SerialNumber = SerialNumber of string
    with
    member this.String = let (SerialNumber s) = this in s
    static member From (value: string option) = match value with | None -> None | Some x -> x |> SerialNumber |> Some

  type CountBy<'a> = {
    count: int
    description: 'a
  }

  type RefValueTypes =
    | Brand
    | Bagtype
    | Country

  // https://fsharpforfunandprofit.com/posts/recipe-part2/
  type Result<'TSuccess,'TFailure> =
    | Success of 'TSuccess
    | Failure of 'TFailure

  let bind switchFunction twoTrackInput =
    match twoTrackInput with
    | Success s -> switchFunction s
    | Failure f -> Failure f

  let (>>=) twoTrackInput switchFunction =
    bind switchFunction twoTrackInput

  let (>=>) switch1 switch2 =
    switch1 >> (bind switch2)



namespace Domain

module SharedTypes =
  type DbId = DbId of int
  type ImageId = ImageId of DbId option

  type EmailAddress = EmailAddress of string
    with
    member this.String = let (EmailAddress s) = this in s
    static member Empty = EmailAddress ""

  type Password = Password of string
    with
    member this.String = let (Password s) = this in s
    static member Empty = Password ""

  type CountBy<'a> = {
    count: int
    description: 'a
  }

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



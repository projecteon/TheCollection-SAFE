namespace Domain

module SharedTypes =
  type DbId = DbId of int
  type ImageId = ImageId of DbId option

  type CountBy<'a> = {
    count: int
    description: 'a
  }

  type Result<'TSuccess,'TFailure> =
    | Success of 'TSuccess
    | Failure of 'TFailure

  let bind switchFunction twoTrackInput =
    match twoTrackInput with
    | Success s -> switchFunction s
    | Failure f -> Failure f

  let (>>=) twoTrackInput switchFunction = 
    bind switchFunction twoTrackInput 
    


namespace Domain.Types

type DbId = int

type ImageId = int option
type Flavour = string option
type Hallmark = string option
type Serie = string option
type SerialNumber = string option

type Result<'TSuccess,'TFailure> =
    | Success of 'TSuccess
    | Failure of 'TFailure

// module ROP =
//   let bind switchFunction twoTrackInput =
//     match twoTrackInput with
//     | Success s -> switchFunction s
//     | Failure f -> Failure f
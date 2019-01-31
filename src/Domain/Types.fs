namespace Domain.Types

open NodaTime

type DbId = int

type ImageId =  DbId option
type Flavour = string option
type Hallmark = string option
type Serie = string option
type SerialNumber = string option
type BrandName = string
type BagtypeName = string
type CountryName = string
type CreatedDate = Instant

type CountBy<'a> = {
  count: int
  description: 'a
}

type Result<'TSuccess,'TFailure> =
    | Success of 'TSuccess
    | Failure of 'TFailure

 module ROP =
   let bind switchFunction twoTrackInput =
     match twoTrackInput with
     | Success s -> switchFunction s
     | Failure f -> Failure f
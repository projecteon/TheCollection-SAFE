namespace Domain.Tea

open NodaTime

open Domain.Searchable
open Domain.Types

type Brand = {
    id: DbId
    name: string
}

type RefValue = {
  id: DbId
  [<Searchable()>] description: string
}

type Teabag = {
  id: DbId;
  [<Searchable()>] brand: RefValue option;
  [<Searchable()>] serie: Serie;
  [<Searchable()>] flavour: Flavour;
  [<Searchable()>] hallmark: Hallmark;
  [<Searchable()>] bagtype: RefValue option;
  [<Searchable()>] country: RefValue option;
  [<Searchable()>] serialnumber: SerialNumber;
  imageid: ImageId;
  created: Instant;
}

type Result<'TSuccess,'TFailure> =
    | Success of 'TSuccess
    | Failure of 'TFailure

module ROP =
  let bind switchFunction twoTrackInput =
    match twoTrackInput with
    | Success s -> switchFunction s
    | Failure f -> Failure f
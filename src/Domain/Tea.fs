namespace Domain.Tea

open Domain.Searchable
open Domain.SharedTypes
open Domain.Types

type Brand = {
    id: DbId
    name: BrandName
}

type Bagtype = {
    id: DbId
    name: BagtypeName
}

type Country = {
    id: DbId
    name: CountryName
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
  created: CreatedDate;
}

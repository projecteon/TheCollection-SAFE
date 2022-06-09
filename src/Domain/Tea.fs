namespace Domain.Tea

open Domain.Searchable
open Domain.SharedTypes
open Domain.Types

type File = {
    id: DbId
    importId: int
    uri: string
    filename: string
    created: CreatedDate
    modified: ModifiedDate
}

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
    tlp_code: CountryCodeTLP
    name_nl: CountryName
}

type RefValue = {
  id: DbId
  [<Searchable()>] description: string
}

type Teabag = {
  id: DbId;
  [<Searchable()>] brand: RefValue;
  [<Searchable()>] serie: Serie option;
  [<Searchable()>] flavour: Flavour;
  [<Searchable()>] hallmark: Hallmark option;
  [<Searchable()>] bagtype: RefValue;
  [<Searchable()>] country: RefValue option;
  [<Searchable()>] serialnumber: SerialNumber option;
  imageid: ImageId;
  archiveNumber: int option;
  created: CreatedDate;
}

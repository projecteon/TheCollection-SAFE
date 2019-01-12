module Services.Dtos

open Domain.Types

type JWT = string
type UserData =
  { UserName : string
    Token    : JWT }

type SearchResult<'a> = {
  data: 'a
  count: int
}

type RefValue = {
  id: int;
  description: string;
};

type Teabag = {
  id: int;
  brand: RefValue option;
  serie: string;
  flavour: string;
  hallmark: string;
  bagtype: RefValue option;
  country: RefValue option;
  serialnumber: string;
  imageid: ImageId;
}

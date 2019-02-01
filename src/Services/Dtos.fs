module Services.Dtos

type Email = Email of string
type JWT = JWT of string

type UserData = {
  UserName : Email
  Token    : JWT
}

type SearchResult<'a> = {
  data: 'a
  count: int
}

type RefValue = {
  id: int;
  description: string;
};


type DbId = int
type ImageId =  DbId option

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

type CountBy<'a> = {
  count: int
  description: 'a
}

let getToken jwtToken =
  match jwtToken with
  | JWT token -> token

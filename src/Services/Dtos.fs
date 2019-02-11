namespace Services

open Domain.SharedTypes

module Dtos =
  type JWT = JWT of string
    with
    member this.String = let (JWT s) = this in s

  type Name = private | Name of string
    with
    member this.String = let (Name s) = this in s
    static member Create(s : string)=
        if (s <> "" || s <> null ) then Ok s
        else Error "Invalid string"

  [<CLIMutable>]
  type LoginViewModel = {
      Email : EmailAddress
      Password : Password
  }

  type UserData = {
    UserName : EmailAddress
    Token    : JWT
  }

  type SearchResult<'a> = {
    data: 'a
    count: int
  }

  type UtcDateTimeString = UtcDateTimeString of string with
    member this.String = let (UtcDateTimeString s) = this in s

  type RefValue = {
    id: DbId;
    description: string;
  };

  type Teabag = {
    id: DbId;
    brand: RefValue option;
    serie: string;
    flavour: string;
    hallmark: string;
    bagtype: RefValue option;
    country: RefValue option;
    serialnumber: string;
    imageid: ImageId;
  }


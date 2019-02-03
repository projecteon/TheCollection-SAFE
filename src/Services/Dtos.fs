namespace Services

open Domain.SharedTypes
open System.Text.RegularExpressions

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

  type CountBy<'a> = {
    count: int
    description: 'a
  }

  let getToken jwtToken =
    match jwtToken with
    | JWT token -> token

  let getDbId dbId =
    match dbId with
    | DbId dbId -> dbId

  let getImageId imageId =
    match imageId with
    | ImageId imageId -> imageId

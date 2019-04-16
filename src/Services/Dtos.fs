namespace Services

open Domain.SharedTypes

module Dtos =
  type JWT = JWT of string
    with
    member this.String = let (JWT s) = this in s

  type RefreshToken = RefreshToken of string
    with
    member this.String = let (RefreshToken s) = this in s

  type Name = Name of string
    with
    member this.String = let (Name s) = this in s

  //type Name = private | Name of string
  //  with
  //  member this.String = let (Name s) = this in s
  //  static member Create(s : string)=
  //      if (s <> "" || (not (isNull s)) ) then Ok s
  //      else Error "Invalid string"

  [<CLIMutable>]
  type LoginViewModel = {
      Email     : EmailAddress
      Password  : Password
  }

  type UserData = {
    UserName    : EmailAddress
    Token       : JWT
    RefreshToken: RefreshToken
  }

  type SearchResult<'a> = {
    data: 'a
    count: int
  }

  type UtcDateTimeString = UtcDateTimeString of string with
    member this.String = let (UtcDateTimeString s) = this in s

  [<ReferenceEquality>]
  type RefValue = {
    id: DbId;
    description: string;
  };

  [<ReferenceEquality>]
  type Bagtype = {
    id: DbId;
    name: Name;
  };

  [<ReferenceEquality>]
  type Brand = {
    id: DbId;
    name: Name;
  };

  [<ReferenceEquality>]
  type Country = {
    id: DbId;
    name: Name;
  };

  [<ReferenceEquality>]
  type Teabag = {
    id: DbId;
    brand: RefValue;
    serie: string;
    flavour: string;
    hallmark: string;
    bagtype: RefValue;
    country: RefValue option;
    serialnumber: string;
    imageid: ImageId;
  }

  type UpsertTeabag = {
    brand: RefValue;
    serie: Serie option;
    flavour: Flavour;
    hallmark: Hallmark option;
    bagtype: RefValue;
    country: RefValue option;
    serialnumber: SerialNumber option;
    imageid: ImageId;
  }

module RefValueValidation =
  open Dtos

  let private validateHasValue (refValue: RefValue) =
    if (refValue.id.Int > 0) then
      Success refValue
    else
      Failure "Value is required"

  let validate (refValue: RefValue) =
    refValue
    |> validateHasValue



module NameValidation =
  open Dtos
  open Domain.Validation

  let private validateLength (name: Name) =
    let isValid =
      name.String
      |> Domain.Validation.StringValidations.minimumLength 1
      >>= Domain.Validation.StringValidations.maximumLength 255

    Helpers.reMapResult name isValid

  let validate (name: Name) =
    name
    |> validateLength


module FlavourValidation =
  open Domain.Validation

  let private validateLength (flavour: Flavour) =
    let isValid =
      flavour.String
      |> Domain.Validation.StringValidations.minimumLength 1
      >>= Domain.Validation.StringValidations.maximumLength 255

    Helpers.reMapResult flavour isValid

  let validate (flavour: Flavour) =
    flavour
    |> validateLength


module TeabagValidation =
  open Dtos
  open Domain.Validation

  let private validateFlavour (teabag: Teabag) =
    let isValid =
      teabag.flavour
      |> StringValidations.maximumLength 1
      >>= StringValidations.maximumLength 255
    Helpers.reMapResult teabag isValid

  let private validateBrand (teabag: Teabag) =
    let isValid = teabag.brand |> RefValueValidation.validate
    Helpers.reMapResult teabag isValid

  let validate (teabag: Teabag) =
    teabag
    |> validateBrand
    >>= validateFlavour

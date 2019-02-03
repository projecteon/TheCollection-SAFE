namespace Services

open Domain.SharedTypes
open System.Text.RegularExpressions

module Dtos =
  type EmailAddress = EmailAddress of string
  type JWT = JWT of string
  type Password = Password of string

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

  let getEmail emailAddress =
    match emailAddress with
    | EmailAddress emailAddress -> emailAddress

  let getPassword password =
    match password with
    | Password password -> password

module PasswordValidation =
  let private validatePasswordEmptiness (password: Dtos.Password) =
    let passwordString = Dtos.getPassword password
    if (passwordString <> null &&
          passwordString <> "") then
      Success password
    else
      Failure "Password cannot be empty"

  let private validatePasswordLength (password: Dtos.Password) =
    let passwordString = Dtos.getPassword password
    if (passwordString.Length <= 6) then
      Failure "Password should contain more than 6 characters"
    else if (passwordString.Length <= 10) then
      Success password
    else
      Failure "Password should not contain more than 10 characters"

  let validate (password: Dtos.Password) =
    password
    |> validatePasswordEmptiness 
    >>= validatePasswordLength 

module EmailValidation =

  let private isValidEmailAddress (emailAddress: Dtos.EmailAddress) =
    // https://emailregex.com/
    let emailAddrRegex =
      @"^(([^<>()\[\]\\.,;:\s@]+(\.[^<>()\[\]\\.,;:\s@]+)*)|(.+))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$"
    let regex = Regex.IsMatch((Dtos.getEmail emailAddress), emailAddrRegex, RegexOptions.IgnoreCase)
    match regex with
    | true -> Success emailAddress
    | false -> Failure "Email address is not in valid format"

  let private validateEmailEmptiness (emailAddress: Dtos.EmailAddress) =
    let emailAddressString = Dtos.getEmail emailAddress
    if (emailAddressString <> null &&
          emailAddressString <> "") then
      Success emailAddress
    else
      Failure "Email address cannot be empty"

  let validate (emailAddress: Dtos.EmailAddress) =
    emailAddress
    |> isValidEmailAddress 
    >>= validateEmailEmptiness 
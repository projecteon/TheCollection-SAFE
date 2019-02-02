namespace Services

open Domain.SharedTypes

module Dtos =
  type EmailAddress = EmailAddress of string
  type JWT = JWT of string
  type Password = Password of string

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
  let validatePasswordEmptiness (password: Dtos.Password) =
    let passwordString = Dtos.getPassword password
    if (passwordString <> null &&
          passwordString <> "") then
      Success password
    else
      Failure "Password cannot be empty"

  let validatePasswordLength (password: Dtos.Password) =
    let passwordString = Dtos.getPassword password
    if (passwordString.Length <= 10) then
      Success passwordString
    else
      Failure "Password should not contain more than 10 characters"

  //let validatePasswordStrength  (password: Dtos.Password)  =
  //  let specialCharacters = [ "*"; "&"; "%"; "$" ]
  //  let hasSpecialCharacters (str : String) =
  //    specialCharacters |> List.exists (fun c -> str.Contains(c))
  //  let errorMessage =
  //    "Password should contain atleast one of the special characters "
  //      + String.Join(",", specialCharacters)
  //  validate
  //    (fun createUserRequest ->
  //      hasSpecialCharacters createUserRequest.Password)
  //    "Password"
  //    errorMessage

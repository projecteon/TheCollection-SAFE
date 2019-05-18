module Client.Teabag.Country.Types

open Domain.SharedTypes
open Server.Api.Dtos

type ValidationErrors =
  | NameError of string
  | UnknownError of string

type Model = {
  originaldata: Option<Country>
  data: Option<Country>
  userData: UserData option
  fetchError: exn option
  doValidation: bool
  validationErrors: ValidationErrors seq
  isWorking: bool
}

let NewCountry = {
  id = DbId 0
  name = Name ""
}

type ExternalMsg =
  | UnChanged
  | OnChange of RefValue

type Msg =
  | New
  | GetError of exn
  | GetSuccess of Country
  | NameChanged of string
  | Save of Country
  | SaveSuccess of int
  | SaveFailure of exn
  | Validate
  | ValidateAndSave
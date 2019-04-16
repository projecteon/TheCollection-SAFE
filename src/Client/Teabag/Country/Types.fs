module Client.Teabag.Country.Types

open Domain.SharedTypes
open Services.Dtos

type ValidationErrors =
  | NameError of string

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
  | GetError of exn
  | GetSuccess of Country
  | NameChanged of string
  | Save of Country
  | SaveSuccess of int
  | SaveFailure of exn
  | Validate
  | ValidateAndSave
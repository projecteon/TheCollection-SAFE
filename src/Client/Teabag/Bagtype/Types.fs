module Client.Teabag.Bagtype.Types

open Domain.SharedTypes
open Services.Dtos

type ValidationErrors =
  | NameError of string

type Model = {
  originaldata: Option<Bagtype>
  data: Option<Bagtype>
  userData: UserData option
  fetchError: exn option
  doValidation: bool
  validationErrors: ValidationErrors seq
  isWorking: bool
}

let NewBagtype: Bagtype = {
  id = DbId 0
  name = Name ""
}

type ExternalMsg =
  | UnChanged
  | OnChange of RefValue

type Msg =
  | GetError of exn
  | GetSuccess of Bagtype
  | NameChanged of string
  | Save of Bagtype
  | SaveSuccess of int
  | SaveFailure of exn
  | Validate
  | ValidateAndSave
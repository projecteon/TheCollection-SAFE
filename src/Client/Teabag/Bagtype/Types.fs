module Client.Teabag.Bagtype.Types

open Domain.SharedTypes
open Server.Api.Dtos

type ValidationErrors =
  | NameError of string
  | UnknownError of string

type Model = {
  originaldata: Option<Bagtype>
  data: Option<Bagtype>
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
  | New
  | GetError of exn
  | GetSuccess of Bagtype
  | NameChanged of string
  | Save of Bagtype
  | SaveSuccess of int
  | SaveFailure of exn
  | Validate
  | ValidateAndSave
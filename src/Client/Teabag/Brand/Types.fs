module Client.Teabag.Brand.Types

open Domain.SharedTypes
open Services.Dtos

type ValidationErrors =
  | NameError of string

type Model = {
  originaldata: Option<Brand>
  data: Option<Brand>
  userData: UserData option
  fetchError: exn option
  doValidation: bool
  validationErrors: ValidationErrors seq
  isWorking: bool
}

let NewBrand: Brand = {
  id = DbId 0
  name = Name ""
}

type ExternalMsg =
  | UnChanged
  | OnChange of RefValue

type Msg =
  | GetError of exn
  | GetSuccess of Brand
  | NameChanged of string
  | Save of Brand
  | SaveSuccess of int
  | SaveFailure of exn
  | Validate
  | ValidateAndSave
module Client.Teabag.Types

open Client.Components
open Domain.SharedTypes
open Server.Api.Dtos

type ValidationErrors =
  | BrandError of string
  | BagtypeError of string
  | FlavourError of string

type Model = {
  originaldata: Option<Teabag>
  data: Option<Teabag>
  brandCmp: ComboBox.Types.Model
  bagtypeCmp: ComboBox.Types.Model
  countryCmp: ComboBox.Types.Model
  userData: UserData option
  fetchError: exn option
  doValidation: bool
  validationErrors: ValidationErrors seq
  isWorking: bool
  editBagtypeCmp: Client.Teabag.Bagtype.Types.Model option
  editBrandCmp: Client.Teabag.Brand.Types.Model option
  editCountryCmp: Client.Teabag.Country.Types.Model option
}

let EmptyRefValue: RefValue = {
  id = DbId 0
  description = ""
}

let NewTeabag = {
  id = DbId 0
  brand = EmptyRefValue
  serie = ""
  flavour = ""
  hallmark = ""
  bagtype = EmptyRefValue
  country = None
  serialnumber = ""
  imageid = ImageId None;
}

type Msg =
  | GetError of exn
  | GetSuccess of Teabag
  | FlavourChanged of string
  | HallmarkChanged of string
  | SerialnumberChanged of string
  | SerieChanged of string
  | BrandCmp of ComboBox.Types.Msg
  | BagtypeCmp of ComboBox.Types.Msg
  | CountryCmp of ComboBox.Types.Msg
  | ImageChanged of ImageId
  | Upload of Fable.Import.Browser.File
  | UploadError of exn
  | Save of Teabag
  | SaveSuccess of int
  | SaveFailure of exn
  | Validate
  | ValidateAndSave
  | Reload
  | ToggleAddBagtypeModal of bool
  | ToggleAddBrandModal of bool
  | ToggleAddCountryModal of bool
  | EditBagtypeCmp of Bagtype.Types.Msg
  | EditBrandCmp of Brand.Types.Msg
  | EditCountryCmp of Country.Types.Msg
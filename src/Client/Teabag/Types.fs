module Client.Teabag.Types

open Client.Components
open Domain.SharedTypes
open Services.Dtos

type Model = {
  data: Option<Teabag>
  brandCmp: ComboBox.Types.Model
  bagtypeCmp: ComboBox.Types.Model
  countryCmp: ComboBox.Types.Model
  userData: UserData option
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
    country = Some EmptyRefValue
    serialnumber = ""
    imageid = ImageId None;
}

type Msg =
| Get
| GetError of exn
| GetSuccess of Teabag
| FlavourChanged of string
| HallmarkChanged of string
| SerialnumberChanged of string
| SerieChanged of string
| BrandCmp of ComboBox.Types.Msg
| BagtypeCmp of ComboBox.Types.Msg
| CountryCmp of ComboBox.Types.Msg
module Client.Teabag.Types

open Client.Components
open Services.Dtos

type Model = {
  data: Option<Teabag>
  brandCmp: ComboBox.Types.Model
  bagtypeCmp: ComboBox.Types.Model
  countryCmp: ComboBox.Types.Model
}

let EmptyRefValue = {
  id = 0
  description = ""
}

let NewTeabag = {
    id = 0
    brand = Some EmptyRefValue
    serie = ""
    flavour = ""
    hallmark = ""
    bagtype = Some EmptyRefValue
    country = Some EmptyRefValue
    serialnumber = ""
    imageid = None;
}

type Msg =
| Get
| GetError of exn
| GetSuccess of Teabag
| HallmarkChanged of string
| BrandCmp of ComboBox.Types.Msg
| BagtypeCmp of ComboBox.Types.Msg
| CountryCmp of ComboBox.Types.Msg
module Client.Dashboard.Types

open Fable.C3
open Services.Dtos

type DataCount = Ten=10 | Twenty=20 

type ChartConfig = {
  data: Data  
  axis: Axis option
}

type ChartTransformation =
| PieToBar
| BarToPie

type Model = {
  countByBrands: CountBy<string> list option
  countByBagtypes: CountBy<string> list option
  countBrands: ChartConfig option 
  countBagtypes: ChartConfig option
  userData: UserData option
}

type Msg =
| GetCountByBrandsError of exn
| GetCountByBrandsSuccess of CountBy<string> list
| GetCountByBagtypesError of exn
| GetCountByBagtypesSuccess of CountBy<string> list
| TransformCountByBrand of ChartTransformation
| TransformCountByBagtype of ChartTransformation
| ReloadBrands

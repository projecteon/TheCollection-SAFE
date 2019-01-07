module Client.Dashboard.Types

open Domain.Types

type Model = {
  countByBrands: CountBy<string> list option
  countByBagtypes: CountBy<string> list option
}

type Msg =
| GetCountByBrandsError of exn
| GetCountByBrandsSuccess of CountBy<string> list
| GetCountByBagtypesError of exn
| GetCountByBagtypesSuccess of CountBy<string> list
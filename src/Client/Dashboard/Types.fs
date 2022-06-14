module Client.Dashboard.Types


open System
open Fable.Core
open Browser.Types

open Server.Api.Dtos
open Domain.SharedTypes
open Fable.Import.Moment.Moment

open Client

type PieChartData<'a> = {
  data: 'a list option
  hoveredIndex: int option
  count: ReChartHelpers.DataCount
}

type BarChartData<'a> = {
  data: 'a list option
  count: ReChartHelpers.DataCount
}

type LineChartData<'a> = {
  data: 'a list option
  hoveredLegendKey: string option
}

type HighchartData = {value: int; key: string}
type Model = {
  statistics: Statistics option
  countByBrands: CountBy<string> list option
  countByBagtypes: CountBy<string> list option
  countByCountryTLD: CountBy<string> list option
  countByInserted: CountBy<Moment> list option
  countByInsertedHoveredKey: string option
  countCountryTLD: HighchartData array option
  displayedByBrands: ReChartHelpers.DataCount
  displayedBrands: ReChartHelpers.DataCount
}

type Msg =
| GetStatisticsError of exn
| GetStatisticsSuccess of Statistics
| GetCountByBrandsError of exn
| GetCountByBrandsSuccess of CountBy<string> list
| GetCountByBagtypesError of exn
| GetCountByBagtypesSuccess of CountBy<string> list
| GetCountByCountryTLDError of exn
| GetCountByCountryTLDSuccess of CountBy<string> list
| GetCountByInsertedError of exn
| GetCountByInsertedSuccess of CountBy<UtcDateTimeString> list
| ExpandByBrands
| CollapseByBrands
| ExpandBrands
| CollapseBrands
| ToggleCountByInsertedHoveredKey of string option

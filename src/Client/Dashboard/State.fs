module Client.Dashboard.State

open Elmish
open Fable.Core
open Fable.Core.JsInterop
open Fetch

open Fable.Import.Moment.Moment
open Thoth.Json

open Client
open Client.Extensions
open Client.ElmishHelpers
open Client.Dashboard.Types
open Domain.SharedTypes
open Server.Api.Dtos
open Client

// https://github.com/fable-compiler/fable-powerpack/blob/master/tests/FetchTests.fs
let getCountByBrandsCmd (token: JWT) =
  Cmd.OfPromise.either
    (Client.Http.fetchAs<CountBy<string> list> "/api/teabags/countby/brands" (Decode.Auto.generateDecoder<CountBy<string> list>()) )
    [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
        HttpRequestHeaders.Accept "application/json"
    ]]
    GetCountByBrandsSuccess
    GetCountByBrandsError

let getCountByBagtypesCmd (token: JWT) =
  Cmd.OfPromise.either
    (Client.Http.fetchAs<CountBy<string> list> "/api/teabags/countby/bagtypes" (Decode.Auto.generateDecoder<CountBy<string> list>()) )
    [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
        HttpRequestHeaders.Accept "application/json"
    ]]
    GetCountByBagtypesSuccess
    GetCountByBagtypesError

let getCountByCountryTLDCmd (token: JWT) =
  Cmd.OfPromise.either
    (Client.Http.fetchAs<CountBy<string> list> "/api/teabags/countby/countrytlp" (Decode.Auto.generateDecoder<CountBy<string> list>()) )
    [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
        HttpRequestHeaders.Accept "application/json"
    ]]
    GetCountByCountryTLDSuccess
    GetCountByCountryTLDError

let getCountByInsertedCmd (token: JWT) =
  Cmd.OfPromise.either
    (Client.Http.fetchAs<CountBy<UtcDateTimeString> list> "/api/teabags/countby/inserteddate" (Decode.Auto.generateDecoder<CountBy<UtcDateTimeString> list>()) )
    [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
        HttpRequestHeaders.Accept "application/json"
    ]]
    GetCountByInsertedSuccess
    GetCountByInsertedError

let getStatisticsCmd (token: JWT) =
  Cmd.OfPromise.either
    (Client.Http.fetchAs<Statistics> "/api/teabags/statistics" (Decode.Auto.generateDecoder<Statistics>()) )
    [Fetch.requestHeaders [
        HttpRequestHeaders.Authorization ("Bearer " + token.String)
        HttpRequestHeaders.ContentType "application/json; charset=utf-8"
        HttpRequestHeaders.Accept "application/json"
    ]]
    GetStatisticsSuccess
    GetStatisticsError

let mapToData (countBy: CountBy<string> list) (count: ReChartHelpers.DataCount) =
  countBy
  |> List.truncate (int count)
  |> List.map (fun x -> [| Some (U3.Case1 x.description); Some (U3.Case3 (float x.count)) |] )
  |> Array.ofList
  |> ResizeArray
  |> Some

let dataCountToggle previousCount =
  match previousCount with
  | ReChartHelpers.DataCount.Ten -> ReChartHelpers.DataCount.Twenty
  | ReChartHelpers.DataCount.Twenty -> ReChartHelpers.DataCount.Ten
  | _ -> ReChartHelpers.DataCount.Ten

let mapToHighchatData (data: CountBy<string> list): HighchartData array =
  data
  |> Seq.map (fun x -> {value = x.count; key = x.description})
  |> Seq.toArray

let init =
    let initialModel = {
      statistics = None
      countByBrands = None
      countByBagtypes = None
      countByCountryTLD = None
      countByInserted = None
      countByInsertedHoveredKey = None
      countCountryTLD = None
      displayedByBrands = ReChartHelpers.DataCount.Ten
      displayedBrands = ReChartHelpers.DataCount.Ten
    }
    initialModel, getCountByBrandsCmd, getCountByBagtypesCmd, getCountByCountryTLDCmd, getCountByInsertedCmd, getStatisticsCmd

let moment: Fable.Import.Moment.IExports = importAll "moment"
let update (msg:Msg) model : Model*Cmd<Msg> =
  match msg with
  | GetStatisticsSuccess data ->
    { model with statistics = Some data }, Cmd.none
  | GetStatisticsError exn -> model, Cmd.none
  | GetCountByBrandsSuccess data ->
    { model with countByBrands = Some data; }, Cmd.none
  | GetCountByBrandsError exn -> model, Cmd.none
  | GetCountByBagtypesSuccess data ->
    { model with countByBagtypes = Some data; }, Cmd.none
  | GetCountByBagtypesError exn -> model, Cmd.none
  | GetCountByCountryTLDSuccess data ->
    { model with countByCountryTLD = Some data; countCountryTLD = Some (mapToHighchatData data)}, Cmd.none
  | GetCountByCountryTLDError exn -> model, Cmd.none
  | GetCountByInsertedSuccess data ->
    let dashboardData = data |> List.map(fun x -> {count = x.count; description = moment(U7.Case3 x.description.String)})
    { model with countByInserted = Some dashboardData; }, Cmd.none
  | GetCountByInsertedError exn -> model, Cmd.none
  | ExpandByBrands -> {model with displayedByBrands = dataCountToggle model.displayedByBrands}, Cmd.none
  | CollapseByBrands -> {model with displayedByBrands = dataCountToggle model.displayedByBrands}, Cmd.none
  | ExpandBrands -> {model with displayedBrands = dataCountToggle model.displayedBrands}, Cmd.ofMsg (GetCountByBrandsSuccess model.countByBrands.Value)
  | CollapseBrands -> {model with displayedBrands = dataCountToggle model.displayedBrands}, Cmd.ofMsg (GetCountByBrandsSuccess model.countByBrands.Value)
  | ToggleCountByInsertedHoveredKey key -> {model with countByInsertedHoveredKey = key}, Cmd.none



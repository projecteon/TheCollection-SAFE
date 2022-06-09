module Client.Components.Pagination

open Fable.React
open Fable.React.Props
open Fulma
open System

[<Literal>]
let private MaxButtons = 7


type ExternalMsg =
  | SearchPageNext
  | SearchPagePrevious
  | SearchPage of int

type Model = {
  currentPage: int
  pageSize: int
  itemCount: int
}

let totalPages (model: Model) =
  let pages = (float model.itemCount) / (float model.pageSize);
  //printf "%f %i %i %i" pages (Math.Ceiling pages |> int) model.itemCount model.pageSize
  Math.Ceiling pages |> int

// https://stackoverflow.com/questions/39670902/pagination-calculation-algorithm
// https://stackoverflow.com/questions/46382109/limit-the-number-of-visible-pages-in-pagination


let getPageList (model: Model) =
  let totalPages = totalPages model
  let sideWidth:int = if MaxButtons < 9 then 1 else 2;
  let leftWidth = (MaxButtons - sideWidth * 2 - 3) >>> 1;
  let rightWidth = (MaxButtons - sideWidth * 2 - 2) >>> 1;
  //printf "%i %i %i %i" totalPages sideWidth leftWidth rightWidth
  if totalPages <= MaxButtons then
    seq { 1 .. totalPages }
  else if (model.currentPage <= MaxButtons - sideWidth - 1 - rightWidth) then
    printf "case 1 %i %i" (totalPages - sideWidth + 1) (MaxButtons - sideWidth - totalPages)
    seq { yield! seq {1 .. MaxButtons - sideWidth - 1}; yield! [0]; yield! seq {totalPages - sideWidth + 1 .. totalPages} }
  else if (model.currentPage >= totalPages - sideWidth - 1 - rightWidth) then
    printf "case 2"
    seq { yield! seq {1 .. sideWidth}; yield! [0]; yield! seq {totalPages - sideWidth - 1 - rightWidth - leftWidth .. totalPages} }
  else 
    printf "case 3"
    seq { yield! seq {1 .. sideWidth}; yield! [0]; yield! seq {model.currentPage - leftWidth .. model.currentPage + rightWidth}; yield! [0]; yield! seq {totalPages - sideWidth + 1 .. totalPages} }

let createPageButtons (currentPage: int) (dispatch: ExternalMsg -> unit) pageList=
  pageList
  |> Seq.map (fun page ->
    if page = 0 then
      Pagination.ellipsis [ GenericOption.Props [Key (page.ToString())] ]
    else
      Pagination.link [ Pagination.Link.Props [Key (page.ToString()); OnClick (fun ev -> dispatch (SearchPage page))]; Pagination.Link.Current (page = currentPage) ] [ page |> sprintf "%i" |> str ])
  |> Seq.toList

let view (model: Model) (dispatch: ExternalMsg -> unit) =
  Pagination.pagination [ Pagination.IsCentered; Pagination.CustomClass "is-sticky-top" ]
    [
      Pagination.previous [ Props [ Disabled (model.currentPage < 2); OnClick (fun ev -> dispatch SearchPagePrevious) ] ]
        [ str "Previous" ]
      Pagination.next [ Props [Disabled (model.currentPage = (totalPages model)); OnClick (fun ev -> dispatch SearchPageNext)] ]
        [ str "Next page" ]
      Pagination.list [ ]
        [
          model|> getPageList |> createPageButtons model.currentPage dispatch |> ofList
        ]
    ]

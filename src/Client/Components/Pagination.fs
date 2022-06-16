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

let getPageList numberOfPages (model: Model) =
  let sideWidth:int = if MaxButtons < 9 then 1 else 2;
  let leftWidth = (MaxButtons - sideWidth * 2 - 3) >>> 1;
  let rightWidth = (MaxButtons - sideWidth * 2 - 2) >>> 1;
  //printf "%i %i %i %i" totalPages sideWidth leftWidth rightWidth
  if numberOfPages <= MaxButtons then
    seq { 1 .. numberOfPages }
  else if (model.currentPage <= MaxButtons - sideWidth - 1 - rightWidth) then
    //printf "case 1 %i %i" (numberOfPages - sideWidth + 1) (MaxButtons - sideWidth - numberOfPages)
    seq { yield! seq {1 .. MaxButtons - sideWidth - 1}; yield! [0]; yield! seq {numberOfPages - sideWidth + 1 .. numberOfPages} }
  else if (model.currentPage >= numberOfPages - sideWidth - 1 - rightWidth) then
    //printf "case 2"
    seq { yield! seq {1 .. sideWidth}; yield! [0]; yield! seq {numberOfPages - sideWidth - 1 - rightWidth - leftWidth .. numberOfPages} }
  else 
    //printf "case 3"
    seq { yield! seq {1 .. sideWidth}; yield! [0]; yield! seq {model.currentPage - leftWidth .. model.currentPage + rightWidth}; yield! [0]; yield! seq {numberOfPages - sideWidth + 1 .. numberOfPages} }

let createPageButtons (currentPage: int) (dispatch: ExternalMsg -> unit) pageList=
  pageList
  |> Seq.mapi (fun i page ->
    if page = 0 then
      Pagination.ellipsis [ GenericOption.Props [Key (i.ToString())] ]
    else
      Pagination.link [ Pagination.Link.Props [Key (i.ToString()); OnClick (fun ev -> dispatch (SearchPage page))]; Pagination.Link.Current (page = currentPage) ] [ page |> sprintf "%i" |> str ])
  |> Seq.toList
  |> ofList

let view (model: Model) (dispatch: ExternalMsg -> unit) =
  let numberOfPages = totalPages model
  match numberOfPages with
  | 0 -> nothing
  | 1 -> nothing
  | _ ->
    Pagination.pagination [ Pagination.IsCentered; Pagination.CustomClass "is-sticky-top" ]
      [
        Pagination.previous [ Props [ Key "previous"; Disabled (model.currentPage < 2); OnClick (fun ev -> dispatch SearchPagePrevious) ] ]
          [ str "Previous" ]
        Pagination.next [ Props [ Key "next"; Disabled (model.currentPage = numberOfPages); OnClick (fun ev -> dispatch SearchPageNext)] ]
          [ str "Next page" ]
        Pagination.list [ Props [Key "list" ] ]
          [
            model|> getPageList numberOfPages |> createPageButtons model.currentPage dispatch
          ]
      ]

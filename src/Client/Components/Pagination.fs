module Client.Components.Pagination

open Fable.React
open Feliz
open Feliz.Bulma
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
      Bulma.paginationEllipsis [ prop.key (i.ToString()) ]
    else
      Bulma.paginationLink.a [ prop.key (i.ToString()); prop.onClick (fun _ -> dispatch (SearchPage page)); prop.text page; if (page = currentPage) then Bulma.paginationLink.isCurrent])
  |> Seq.toList
  |> ofList

let view (model: Model) (dispatch: ExternalMsg -> unit) =
  let numberOfPages = totalPages model
  match numberOfPages with
  | 0 -> Html.none
  | 1 -> Html.none
  | _ ->
    Bulma.pagination [
      Bulma.pagination.isCentered
      prop.className "is-sticky-top"
      prop.children [
        Bulma.paginationPrevious.a [ prop.key "previous"; prop.disabled (model.currentPage < 2); prop.onClick (fun _ -> dispatch SearchPagePrevious); prop.text "Previous" ]
        Bulma.paginationNext.a [ prop.key "next"; prop.disabled (model.currentPage = numberOfPages); prop.onClick (fun _ -> dispatch SearchPageNext); prop.text "Next page" ]
        Bulma.paginationList [
          prop.key "list"
          prop.children [
            model|> getPageList numberOfPages |> createPageButtons model.currentPage dispatch
          ]
        ]
      ]
    ]

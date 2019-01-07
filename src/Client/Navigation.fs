module Client.Navigation

open Elmish.Browser.UrlParser

/// The different pages of the application. If you add a new page, then add an entry here.
[<RequireQualifiedAccess>]
type Page =
  | Dashboard
  | Teabags
  | Teabag of string

let toPath =
  function
  | Page.Dashboard -> "/"
  | Page.Teabags -> "/teabags"
  | Page.Teabag id -> "/teabags/" + id

/// https://elmish.github.io/browser/routing.html
/// https://github.com/elmish/sample-react-navigation/blob/master/src/app.fs

/// The URL is turned into a Result.
let pageParser : Parser<Page -> Page,_> =
  oneOf
    [
      map Page.Dashboard (s "")
      map Page.Teabags (s "teabags")
      map Page.Teabag (s "teabags" </> str)
    ]

let urlParser location = parsePath pageParser location

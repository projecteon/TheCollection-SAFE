module Client.Navigation

open Elmish.Browser.UrlParser

/// The different pages of the application. If you add a new page, then add an entry here.
[<RequireQualifiedAccess>]
type Page =
  | Dashboard
  | Login
  | Logout
  | Teabags
  | Teabag of int
  | TeabagNew of string

let toPath page =
  match page with
  | Page.Dashboard -> "/"
  | Page.Login -> "/login"
  | Page.Logout -> "/logout"
  | Page.Teabags -> "/teabags"
  | Page.Teabag id -> sprintf "/teabags/%i" id
  | Page.TeabagNew str -> "/teabags/new"

/// https://elmish.github.io/browser/routing.html
/// https://github.com/elmish/sample-react-navigation/blob/master/src/app.fs

/// The URL is turned into a Result.
let pageParser : Parser<Page -> Page,_> =
  oneOf
    [
      map Page.Dashboard (s "")
      map Page.Login (s "login")
      map Page.Logout (s "logout")
      map Page.Teabags (s "teabags")
      map Page.Teabag (s "teabags" </> i32)
      map Page.TeabagNew (s "teabags" </> str)
    ]

let urlParser location =
  parsePath pageParser location

module ClientTypes

open Services.Dtos

type PageModel =
    | DashboardPageModel of Client.Dashboard.Types.Model
    | TeabagsPageModel of Client.Teabags.Types.Model
    | TeabagPageModel of Client.Teabag.Types.Model
    | LoginPageModel

type Model =
    { User : UserData option
      PageModel : PageModel }

type Msg =
    | DashboardMsg of Client.Dashboard.Types.Msg
    | TeabagsMsg of Client.Teabags.Types.Msg
    | TeabagMsg of Client.Teabag.Types.Msg

module ClientTypes

open Services.Dtos

type PageModel =
    | HomePageModel
    | TeabagsPageModel of Client.Teabags.Types.Model
    | TeabagPageModel of Client.Teabag.Types.Model

type Model =
    { User : UserData option
      PageModel : PageModel }

type Msg =
    | TeabagsMsg of Client.Teabags.Types.Msg
    | TeabagMsg of Client.Teabag.Types.Msg

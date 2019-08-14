module Client.Components.Navbar.Types

open Elmish

open Server.Api.Dtos
open Domain.SharedTypes

type Model =
  { isBurgerOpen: bool }

type Msg =
  | ToggleBurger

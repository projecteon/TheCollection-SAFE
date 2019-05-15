namespace Security

module Authorization =
  open System.Security.Claims
  open Microsoft.AspNetCore.Authentication.JwtBearer
  open Microsoft.AspNetCore.Http
  open Microsoft.IdentityModel.Tokens
  open FSharp.Control.Tasks.V2
  open Giraffe

  open Domain.SharedTypes
  open Domain.Types
  open Server.Api.Dtos
  open Security.JsonWebToken

  let authorize: HttpHandler =
    requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)

  let refreshToken (token: JWT, refreshToken: RefreshToken) =
    let principal = getPrincipalFromExpiredToken token
    let username = principal.Identity.Name
    let savedRefreshToken = RefreshToken "" // GetRefreshToken(username) //retrieve the refresh token from a data store
    if (savedRefreshToken <> refreshToken) then
        raise (new SecurityTokenException("Invalid refresh token"))
    else
      let newJwtToken = generateTokenFromClaims principal.Claims
      let newRefreshToken = generateRefreshToken
      //DeleteRefreshToken(username, refreshToken)
      //SaveRefreshToken(username, newRefreshToken)
      (newJwtToken, newRefreshToken)


  let handleGetSecured =
      fun (next : HttpFunc) (ctx : HttpContext) ->
          let email = ctx.User.FindFirst ClaimTypes.NameIdentifier
          text ("User " + email.Value + " is authorized to access this resource.") next ctx

  let loginUserCmd (userRepository: (EmailAddress) -> System.Threading.Tasks.Task<User option>) (loginModel: LoginViewModel) =
    task {
      let! dbUser = userRepository loginModel.Email
      match dbUser with
      | None -> return Failure "Invalid username or password!"
      | Some user ->
        if user.Password = loginModel.Password then return ((generateToken user.Email) |> Success)
        else return Failure "Invalid username or password!"
    }

  let handlePostToken userRepository =
    fun (next : HttpFunc) (ctx : HttpContext) ->
      task {
        printf "handlePostToken"
        let! model = ctx.BindJsonAsync<LoginViewModel>()
        let! user = loginUserCmd userRepository model
        match user with
        | Success loginResult ->
          return! Successful.OK loginResult next ctx
        | Failure msg -> return! (RequestErrors.UNAUTHORIZED "Basic" "The Collection" msg) next ctx
      }


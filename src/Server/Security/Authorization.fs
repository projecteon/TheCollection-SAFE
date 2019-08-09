namespace Security

module Authorization =
  open System.Security.Claims
  open Microsoft.AspNetCore.Authentication.JwtBearer
  open Microsoft.AspNetCore.Http
  open FSharp.Control.Tasks.V2
  open Giraffe

  open Domain.SharedTypes
  open Domain.Types
  open Server.Api.Dtos
  open Security.JsonWebToken
  open System

  let authorize: HttpHandler =
    requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)

  let refreshToken (userRepository: (EmailAddress) -> System.Threading.Tasks.Task<User option>) (updateUserRepository: User -> System.Threading.Tasks.Task<int>) (token: JWT, refreshToken: RefreshToken) =
    task {
      let securityToken = getJwtSecturityTokenFromExpiredToken token
      let emailClaim = securityToken.Claims |> Seq.tryFind (fun x -> x.Type = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
      let userEmail = emailClaim.Value.Value |> EmailAddress
      let! userOption = userRepository userEmail;
      match userOption with
      | Some user when user.RefreshToken.IsSome ->
        if (user.RefreshToken.Value <> refreshToken) then
          return Failure "Invalid refresh token!"
        else if user.RefreshTokenExpire.Value.IsBefore System.DateTime.Now then
          return Failure "Refresh token expired!"
        else
          let newToken = {
                            UserName = userEmail
                            Token = generateTokenFromClaims securityToken.Claims
                            RefreshToken = generateRefreshToken
                          }
          let updatedUser = {user with RefreshToken = Some newToken.RefreshToken; RefreshTokenExpire = Some (DateTime.Now.AddHours(8.0) |> RefreshTokenExpire)}
          let! updateUserResult = updateUserRepository updatedUser
          return Success newToken
      | _ -> return Failure "Invalid user" //raise (new SecurityTokenException("Invalid user"))
    }

  let handleGetSecured =
      fun (next : HttpFunc) (ctx : HttpContext) ->
          let email = ctx.User.FindFirst ClaimTypes.NameIdentifier
          text ("User " + email.Value + " is authorized to access this resource.") next ctx

  let loginUserCmd (userRepository: (EmailAddress) -> System.Threading.Tasks.Task<User option>) (updateUserRepository: User -> System.Threading.Tasks.Task<int>) (loginModel: LoginViewModel) =
    task {
      let! dbUser = userRepository loginModel.Email
      match dbUser with
      | None -> return Failure "Invalid username or password!"
      | Some user ->
        if user.Password = loginModel.Password then
          let token = generateToken user.Email
          let updatedUser = {user with RefreshToken = Some token.RefreshToken; RefreshTokenExpire = Some (DateTime.Now.AddHours(8.0) |> RefreshTokenExpire)}
          let! updateUserResult = updateUserRepository updatedUser
          return (token |> Success)
        else return Failure "Invalid username or password!"
    }

  let handlePostToken userRepository updateUserRepository =
    fun (next : HttpFunc) (ctx : HttpContext) ->
      task {
        let! model = ctx.BindJsonAsync<LoginViewModel>()
        let! loginResult = loginUserCmd userRepository updateUserRepository model
        match loginResult with
        | Success userData ->
          return! json userData next ctx
        | Failure msg -> return! (RequestErrors.UNAUTHORIZED "Basic" "The Collection" msg) next ctx
    }

  let handlePostRefreshToken userRepository updateUserRepository (next : HttpFunc) (ctx : HttpContext) =
    task {
      let! model = ctx.BindJsonAsync<RefreshTokenViewModel>()
      let! loginResult = refreshToken userRepository updateUserRepository (model.Token, model.RefreshToken)
      match loginResult with
      | Success userData ->
        return! json userData next ctx
      | Failure msg -> return! (RequestErrors.UNAUTHORIZED "Basic" "The Collection" msg) next ctx
    }

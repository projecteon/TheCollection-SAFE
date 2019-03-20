module Security

open System
open System.Text
open System.Security.Claims
open System.IdentityModel.Tokens.Jwt
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Http
open Microsoft.IdentityModel.Tokens
open FSharp.Control.Tasks.V2
open Giraffe

open Domain.SharedTypes
open Services.Dtos


// https://medium.com/@dsincl12/json-web-token-with-giraffe-and-f-4cebe1c3ef3b
// https://github.com/giraffe-fsharp/Giraffe/blob/master/samples/JwtApp/JwtApp/Program.fs

let secret = "spadR2dre#u-ruBrE@TepA&*Uf@U"

let authorize: HttpHandler =
    requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)

let generateToken (email: EmailAddress) =
    let claims = [|
        Claim(JwtRegisteredClaimNames.Sub, email.String);
        Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) |]

    let expires = Nullable(DateTime.UtcNow.AddHours(1.0))
    let notBefore = Nullable(DateTime.UtcNow)
    let securityKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    let signingCredentials = SigningCredentials(key = securityKey, algorithm = SecurityAlgorithms.HmacSha256)

    let token =
        JwtSecurityToken(
            issuer = "jwtwebapp.net",
            audience = "jwtwebapp.net",
            claims = claims,
            expires = expires,
            notBefore = notBefore,
            signingCredentials = signingCredentials)

    let tokenResult = {
      UserName = email
      Token = JWT <| JwtSecurityTokenHandler().WriteToken(token)
    }

    tokenResult

let handleGetSecured =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let email = ctx.User.FindFirst ClaimTypes.NameIdentifier
        text ("User " + email.Value + " is authorized to access this resource.") next ctx

let loginUser loginModel =
  if loginModel.Email.String = "spro@outlook.com" && loginModel.Password.String = "appmaster" then (generateToken loginModel.Email) |> Some
  else if loginModel.Email.String = "l.wolterink@hotmail.com" && loginModel.Password.String = "teamaster" then (generateToken loginModel.Email) |> Some
  else None

let handlePostToken =
  fun (next : HttpFunc) (ctx : HttpContext) ->
    task {
      let! model = ctx.BindJsonAsync<LoginViewModel>()
      match (loginUser model) with
      | Some tokenResult -> 
        return! Successful.OK tokenResult next ctx
      | None -> return! (RequestErrors.UNAUTHORIZED "Basic" "The Collection" "Invalid username or password!") next ctx 
    }


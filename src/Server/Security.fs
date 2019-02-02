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

let secret = "spadR2dre#u-ruBrE@TepA&*Uf@U"

let authorize: HttpHandler =
    requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)

let generateToken email =
    let claims = [|
        Claim(JwtRegisteredClaimNames.Sub, email);
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
      UserName = EmailAddress email
      Token = JWT <| JwtSecurityTokenHandler().WriteToken(token)
    }

    tokenResult

let handleGetSecured =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let email = ctx.User.FindFirst ClaimTypes.NameIdentifier
        text ("User " + email.Value + " is authorized to access this resource.") next ctx

let private unboxEmailAdress emailAddress =
  match emailAddress with
  | EmailAddress emailAddress -> emailAddress

let private unboxPassword password =
  match password with
  | Password password -> password

let validateUser loginModel =
  if (unboxEmailAdress loginModel.Email) = "spro@outlook.com" && (unboxPassword loginModel.Password) = "appmaster" then true
  else if (unboxEmailAdress loginModel.Email) = "l.wolterink@hotmail.com" && (unboxPassword loginModel.Password) = "teamaster" then true
  else false

let handlePostToken =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! model = ctx.BindJsonAsync<LoginViewModel>()
            match (validateUser model) with
            | true -> 
              let tokenResult = generateToken (getEmail model.Email)
              return! Successful.OK tokenResult next ctx
            | _ -> return! (RequestErrors.UNAUTHORIZED "Basic" "The Collection" "Invalid username or password!") next ctx 
        }
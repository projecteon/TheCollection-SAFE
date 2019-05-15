module Security

open System
open System.Text
open System.Security.Claims
open System.Security.Cryptography
open System.IdentityModel.Tokens.Jwt
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Http
open Microsoft.IdentityModel.Tokens
open FSharp.Control.Tasks.V2
open Giraffe

open Domain.SharedTypes
open Services.Dtos
open Domain.Types


// https://medium.com/@dsincl12/json-web-token-with-giraffe-and-f-4cebe1c3ef3b
// https://github.com/giraffe-fsharp/Giraffe/blob/master/samples/JwtApp/JwtApp/Program.fs

let secret = "spadR2dre#u-ruBrE@TepA&*Uf@U"

let authorize: HttpHandler =
    requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)

// https://www.blinkingcaret.com/2018/05/30/refresh-tokens-in-asp-net-core-web-api/
// https://www.c-sharpcorner.com/article/handle-refresh-token-using-asp-net-core-2-0-and-json-web-token/
let generateRefreshToken =
  let randomNumber = Array.zeroCreate<byte> 32
  use rng = RandomNumberGenerator.Create()
  rng.GetBytes(randomNumber)
  Convert.ToBase64String(randomNumber)
  |> RefreshToken
    
let generateTokenFromClaims claims =
    let expires = Nullable(DateTime.UtcNow.AddHours(1.0))
    let notBefore = Nullable(DateTime.UtcNow)
    let securityKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    let signingCredentials = SigningCredentials(key = securityKey, algorithm = SecurityAlgorithms.HmacSha256)

    let token =
        JwtSecurityToken(
            issuer = "thecollection.net",
            audience = "thecollection.net",
            claims = claims,
            expires = expires,
            notBefore = notBefore,
            signingCredentials = signingCredentials)

    JWT <| JwtSecurityTokenHandler().WriteToken(token)

let generateToken (email: EmailAddress) =
    let claims = [|
        Claim(JwtRegisteredClaimNames.Sub, email.String);
        Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) |]

    let tokenResult = {
      UserName = email
      Token = generateTokenFromClaims claims
      RefreshToken = generateRefreshToken
    }

    tokenResult

let getPrincipalFromExpiredToken (token: JWT) =
  let tokenValidationParameters = new TokenValidationParameters(
                                                                  ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                                                                  ValidateIssuer = false,
                                                                  ValidateIssuerSigningKey = true,
                                                                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("the server key used to sign the JWT token is here, use more than 16 chars")),
                                                                  ValidateLifetime = true //here we are saying that we don't care about the token's expiration date
                                                                )
  let tokenHandler = new JwtSecurityTokenHandler()
  let (principal, securityToken) = tokenHandler.ValidateToken(token.String, tokenValidationParameters)
  let jwtSecurityToken = securityToken :?> JwtSecurityToken
  if (jwtSecurityToken = null || (String.Compare(jwtSecurityToken.Header.Alg, SecurityAlgorithms.HmacSha256, StringComparison.CurrentCultureIgnoreCase) <> 0)) then
      raise (new SecurityTokenException("Invalid token"))
  else
    principal

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
      let! model = ctx.BindJsonAsync<LoginViewModel>()
      let! user = loginUserCmd userRepository model
      match user with
      | Success loginResult -> 
        return! Successful.OK loginResult next ctx
      | Failure msg -> return! (RequestErrors.UNAUTHORIZED "Basic" "The Collection" msg) next ctx 
    }


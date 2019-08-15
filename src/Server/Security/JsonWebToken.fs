namespace Security

module JsonWebToken =
  open System
  open System.Text
  open System.Security.Claims
  open System.Security.Cryptography
  open System.IdentityModel.Tokens.Jwt

  open Domain.SharedTypes
  open Server.Api.Dtos

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
      let securityKey = Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(Server.Config.JwtSecret))
      let signingCredentials = Microsoft.IdentityModel.Tokens.SigningCredentials(key = securityKey, algorithm = Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256)

      let token =
          JwtSecurityToken(
              issuer = Server.Config.JwtIssuer,
              audience = Server.Config.JwtIssuer,
              claims = claims,
              expires = expires,
              notBefore = notBefore,
              signingCredentials = signingCredentials)

      JWT <| JwtSecurityTokenHandler().WriteToken(token)

  let generateToken (email: EmailAddress) =
      let claims = [|
          Claim(JwtRegisteredClaimNames.Sub, email.String);
          Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) |]

      let tokenResult: UserData = {
        UserName = email
        Token = generateTokenFromClaims claims
        RefreshToken = generateRefreshToken
      }

      tokenResult

  let getJwtSecturityTokenFromExpiredToken (token: JWT) =
    let tokenValidationParameters = Microsoft.IdentityModel.Tokens.TokenValidationParameters(
                                                                    ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                                                                    ValidateIssuer = false,
                                                                    ValidateIssuerSigningKey = true,
                                                                    IssuerSigningKey = Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(Server.Config.JwtSecret)),
                                                                    ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
                                                                  )
    let tokenHandler = JwtSecurityTokenHandler()
    let (principal, securityToken) = tokenHandler.ValidateToken(token.String, tokenValidationParameters)
    let jwtSecurityToken = securityToken :?> JwtSecurityToken
    if ((isNull jwtSecurityToken) || (String.Compare(jwtSecurityToken.Header.Alg, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256, StringComparison.CurrentCultureIgnoreCase) <> 0)) then
        raise (Microsoft.IdentityModel.Tokens.SecurityTokenException("Invalid token"))
    else
      jwtSecurityToken

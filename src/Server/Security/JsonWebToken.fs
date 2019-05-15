namespace Security

module JsonWebToken =
  open System
  open System.Text
  open System.Security.Claims
  open System.Security.Cryptography
  open System.IdentityModel.Tokens.Jwt
  open Microsoft.IdentityModel.Tokens

  open Domain.SharedTypes
  open Services.Dtos

  // https://medium.com/@dsincl12/json-web-token-with-giraffe-and-f-4cebe1c3ef3b
  // https://github.com/giraffe-fsharp/Giraffe/blob/master/samples/JwtApp/JwtApp/Program.fs
  let secret = "spadR2dre#u-ruBrE@TepA&*Uf@U"

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

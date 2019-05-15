namespace Domain.Types

open System
open NodaTime

open Domain.SharedTypes

type BrandName  = BrandName of string
    with
    member this.String = let (BrandName s) = this in s
type BagtypeName  = BagtypeName of string
    with
    member this.String = let (BagtypeName s) = this in s
type CountryName  = CountryName of string
    with
    member this.String = let (CountryName s) = this in s

type CreatedDate = Instant

type RefreshTokenExpire = RefreshTokenExpire of DateTime
  with
  member this.DateTime = let (RefreshTokenExpire dt) = this in dt

type User = {
  Id: DbId
  Email : EmailAddress
  Password : Password
  RefreshToken: RefreshToken option
  RefreshTokenExpire: RefreshTokenExpire option
}
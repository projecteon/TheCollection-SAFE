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

type CreatedDate = CreatedDate of Instant
    with
    member this.Instant = let (CreatedDate instant) = this in instant
    static member Now = DateTime.Now.ToUniversalTime() |> Instant.FromDateTimeUtc |> CreatedDate

type ModifiedDate = ModifiedDate of Instant
    with
    member this.Instant = let (ModifiedDate instant) = this in instant
    static member Now = DateTime.Now.ToUniversalTime() |> Instant.FromDateTimeUtc |> ModifiedDate

type RefreshTokenExpire = RefreshTokenExpire of DateTime
  with
  member this.DateTime = let (RefreshTokenExpire dt) = this in dt
  member this.IsBefore (dateTime: System.DateTime) = this.DateTime < dateTime

type User = {
  Id: DbId
  Email : EmailAddress
  Password : Password
  RefreshToken: RefreshToken option
  RefreshTokenExpire: RefreshTokenExpire option
}
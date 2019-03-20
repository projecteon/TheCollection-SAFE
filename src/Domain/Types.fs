namespace Domain.Types

open NodaTime

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

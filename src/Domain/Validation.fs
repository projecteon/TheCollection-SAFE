namespace Domain.Validation

open Domain.SharedTypes
open System.Text.RegularExpressions

module Helpers =
  let reMapResult (model: 'a) (validationResult: Result<'b, 'c>) =
    match validationResult with
    | Success x -> Success model
    | Failure y -> Failure y

module StringValidations =
  let notNull (value: string) =
    if (value = null && value.Length > 0) then
      Failure (sprintf "Is required")
    else
      Success value

  let minimumLength (length: int) (value: string) =
    if (value.Length < length) then
      Failure (sprintf "Must contain more than %i characters" length)
    else
      Success value

  let maximumLength (length: int) (value: string) =
    if (value.Length > length) then
      Failure (sprintf "Cannot contain more than %i characters" length)
    else
      Success value

module PasswordValidation =
  let private validatePasswordEmptiness (password: Password) =
    let passwordString = password.String
    if (passwordString <> null && passwordString <> "") then
      Success password
    else
      Failure "Password cannot be empty"

  let private validatePasswordLength (password: Password) =
    let passwordString = password.String
    if (passwordString.Length <= 6) then
      Failure "Password should contain more than 6 characters"
    else if (passwordString.Length <= 20) then
      Success password
    else
      Failure "Password should not contain more than 20 characters"

  let validate (password: Password) =
    password
    |> validatePasswordEmptiness
    >>= validatePasswordLength

module EmailValidation =
  let private isValidEmailAddress (emailAddress: EmailAddress) =
    // https://emailregex.com/
    let emailAddrRegex =
      @"^(([^<>()\[\]\\.,;:\s@]+(\.[^<>()\[\]\\.,;:\s@]+)*)|(.+))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$"
    let regex = Regex.IsMatch((emailAddress.String), emailAddrRegex, RegexOptions.IgnoreCase)
    match regex with
    | true -> Success emailAddress
    | false -> Failure "Email address is not in a valid format"

  let private validateEmailEmptiness (emailAddress: EmailAddress) =
    let emailAddressString = emailAddress.String
    if (emailAddressString <> null &&
          emailAddressString <> "") then
      Success emailAddress
    else
      Failure "Email address cannot be empty"

  let validate (emailAddress: EmailAddress) =
    emailAddress
    |> isValidEmailAddress
    >>= validateEmailEmptiness


//module RefValueValidation =
//  open Domain.Tea

//  let private validateHasValue (refValue: RefValue option) =
//    if (refValue.IsSome && refValue.Value.id > (0 |> DbId)) then
//      Success refValue
//    else
//      Failure "Value is required"

//  let validate (refValue: RefValue option) =
//    refValue
//    |> validateHasValue

//module FlavourValidation =
//  open Domain.Types

//  let private validateEmailEmptiness (flavour: Flavour) =
//    let flavourString = flavour.String
//    if (flavourString.IsSome &&
//        flavourString.Value <> null &&
//        flavourString.Value <> "") then
//      Success flavour
//    else
//      Failure "Flavour is required"

//  let validate (flavour: Flavour) =
//    flavour
//    |> validateEmailEmptiness

//module TeabagValidation =
//  open Domain.Tea

//  let private validateFlavour (teabag: Teabag) =
//    match (FlavourValidation.validate teabag.flavour) with
//    | Failure x -> Failure x
//    | _ -> Success teabag

//  let private validateBrand (teabag: Teabag) =
//    match (RefValueValidation.validate teabag.brand) with
//    | Failure x -> Failure x
//    | _ -> Success teabag

//  let validate (teabag: Teabag) =
//    teabag
//    |> validateFlavour
//    >>= validateBrand
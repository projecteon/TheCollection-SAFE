namespace Domain.Validation

open Domain.SharedTypes
open System.Text.RegularExpressions

module PasswordValidation =
  let private validatePasswordEmptiness (password: Password) =
    let passwordString = password.String
    if (passwordString <> null &&
          passwordString <> "") then
      Success password
    else
      Failure "Password cannot be empty"

  let private validatePasswordLength (password: Password) =
    let passwordString = password.String
    if (passwordString.Length <= 6) then
      Failure "Password should contain more than 6 characters"
    else if (passwordString.Length <= 10) then
      Success password
    else
      Failure "Password should not contain more than 10 characters"

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
    | false -> Failure "Email address is not in valid format"

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
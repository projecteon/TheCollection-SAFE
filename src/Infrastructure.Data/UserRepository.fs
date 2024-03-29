namespace Infrastructure.Data

open System.Threading.Tasks
open FSharp.Control.Tasks.V2

open FSharp.Data
open DbContext
open Domain.SharedTypes
open Domain.Types

module UserRepository =
    [<Literal>]
    let  ByIdSQL = "
        SELECT a.id, a.s_email, a.s_password, a.s_refreshtoken, a.dt_refreshtoken_expire
        FROM tcuac_users a
        WHERE id = @id
    "

    let makeTask() =
      task { return 1 }

    type UserById = SqlCommandProvider<ByIdSQL, DevConnectionString, SingleRow = true>
    let getById (config: DbConfig) (id: int) : Task<User option> =
      task {
        let cmd = new UserById(config.ReadOnly.String)
        return cmd.Execute(id)
        |> function
          | Some x -> Some {
              Id = DbId x.id
              Email = EmailAddress x.s_email
              Password = Password x.s_password
              RefreshToken = match x.s_refreshtoken with | Some token -> token |> RefreshToken |> Some | None -> None
              RefreshTokenExpire = match x.dt_refreshtoken_expire with | Some token -> token |> RefreshTokenExpire |> Some | None -> None
            }
          | _ -> None
      }

    [<Literal>]
    let  ByEmailSQL = "
        SELECT a.id, a.s_email, a.s_password, a.s_refreshtoken, a.dt_refreshtoken_expire
        FROM tcuac_users a
        WHERE a.s_email = @email
    "

    type UserByEmail = SqlCommandProvider<ByEmailSQL, DevConnectionString, SingleRow = true>
    let getByEmail (config: DbConfig) (email: EmailAddress) : Task<User option> =
      task {
        let cmd = new UserByEmail(config.ReadOnly.String)
        return cmd.Execute(email.String)
        |> function
          | Some x -> Some {
              Id = DbId x.id
              Email = EmailAddress x.s_email
              Password = Password x.s_password
              RefreshToken = match x.s_refreshtoken with | Some token -> token |> RefreshToken |> Some | None -> None
              RefreshTokenExpire = match x.dt_refreshtoken_expire with | Some token -> token |> RefreshTokenExpire |> Some | None -> None
            }
          | _ -> None
      }

    [<Literal>]
    let  UpdateUserSQL = "
        UPDATE tcuac_users
          SET s_refreshtoken = @refreshToken
              , dt_refreshtoken_expire = @refreshTokenExpire
              , dt_modified = GETDATE()
        WHERE id = @id
    "

    type UpdateUser = SqlCommandProvider<UpdateUserSQL, DevConnectionString, SingleRow = true>
    let update (config: DbConfig) (user: User) =
      task {
        let cmd = new UpdateUser(config.Default.String)
        return cmd.Execute(
          user.RefreshToken |> Mapping.toDbStringValue
          , user.RefreshTokenExpire |> Mapping.toDbDateTimeValue
          , user.Id.Int)
      }

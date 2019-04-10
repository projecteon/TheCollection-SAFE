namespace TeaCollection.Infrastructure.MsSql

open System.Threading.Tasks
open FSharp.Control.Tasks.V2

open FSharp.Data
open DbContext
open Domain.SharedTypes
open Domain.Types

module UserRepository =
    [<Literal>]
    let  ByIdSQL = "
        SELECT a.id, a.s_email, a.s_password
        FROM tcuac_users a
        WHERE id = @id
    "

    type UserById = SqlCommandProvider<ByIdSQL, ConnectionString, SingleRow = true>
    let getById (connectiongString: string) (id: int) : Task<User option> =
      task {
        let cmd = new UserById(connectiongString)
        return cmd.Execute(id)
        |> function
          | Some x -> Some {
              Id = DbId x.id
              Email = EmailAddress x.s_email
              Password = Password x.s_password
            }
          | _ -> None
      }

    [<Literal>]
    let  ByEmailSQL = "
        SELECT a.id, a.s_email, a.s_password
        FROM tcuac_users a
        WHERE a.s_email = @email
    "

    type UserByEmail = SqlCommandProvider<ByEmailSQL, ConnectionString, SingleRow = true>
    let getByEmail (connectiongString: string) (email: EmailAddress) : Task<User option> =
      task {
        let cmd = new UserByEmail(connectiongString)
        return cmd.Execute(email.String)
        |> function
          | Some x -> Some {
              Id = DbId x.id
              Email = EmailAddress x.s_email
              Password = Password x.s_password
            }
          | _ -> None
      }

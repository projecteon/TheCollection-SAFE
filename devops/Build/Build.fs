open Fake.Core
open Fake.IO
open Farmer
open Farmer.Builders

open Helpers

initializeContext()

let migrationsPath = Path.getFullName "devops/Migrations"
let sharedPath = Path.getFullName "src/Shared"
let serverPath = Path.getFullName "src/Server"
let clientPath = Path.getFullName "src/Client"
let deployPath = Path.getFullName "deploy"
let sharedTestsPath = Path.getFullName "tests/Shared"
let serverTestsPath = Path.getFullName "tests/Server"
let clientTestsPath = Path.getFullName "tests/Client"

Target.create "Clean" (fun _ ->
    Shell.cleanDir deployPath
    run dotnet "fable clean --yes" clientPath // Delete *.fs.js files created by Fable
)

Target.create "InstallClient" (fun _ -> run npm "install" ".")

Target.create "Bundle" (fun _ ->
    [ "server", dotnet $"publish -c Release -o \"{deployPath}\"" serverPath
      "client", dotnet "fable -o output -s --run webpack -p" clientPath ]
    |> runParallel
)

Target.create "Azure" (fun _ ->
    Azure.executeDeployment
    |> ignore
)

Target.create "Build" (fun _ ->
    [ "server", dotnet $"build -c Release -o \"{deployPath}\"" serverPath
      "client", dotnet "fable -o output -s webpack -p" clientPath ]
    |> runParallel
)

Target.create "Run" (fun _ ->
    run dotnet "build" sharedPath
    [ "server", dotnet "watch run" serverPath
      "client", dotnet "fable watch -o output -s --run webpack-dev-server" clientPath ]
    |> runParallel
)

Target.create "RunTests" (fun _ ->
    run dotnet "build" sharedTestsPath
    [ "server", dotnet "watch run" serverTestsPath
      "client", dotnet "fable watch -o output -s --run webpack-dev-server --config ../../webpack.tests.config.js" clientTestsPath ]
    |> runParallel
)

Target.create "RunMigrations" (fun _ ->
    run dotnet "run" migrationsPath
)

Target.create "Format" (fun _ ->
    run dotnet "fantomas . -r" "src"
)

open Fake.Core.TargetOperators

let dependencies = [
    "Clean"
        ==> "InstallClient"
        ==> "Bundle"
        ==> "Azure"

    "Clean"
        ==> "InstallClient"
        ==> "Run"

    "Clean"
        ==> "InstallClient"
        ==> "Build"

    "InstallClient"
        ==> "RunTests"
]

[<EntryPoint>]
let main args =
    runOrDefault args
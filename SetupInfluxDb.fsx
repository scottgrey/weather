#load @"./paket-files/include-scripts/net45/include.main.group.fsx"
#r "./manualpackages/influxdb-fsharp/lib/InfluxDB.FSharp.dll"
#load @"./src/Influx.fs"

open System
open InfluxDB.FSharp
open Weather.Influx

let client = Client("localhost")


let databases = client.ShowDatabases() |> Async.RunSynchronously |> notFail

databases

//let pong = client.PingAsync()
//let pongResult = pong.Result
//pongResult

//let dropDb = "test"
//let deleteDb = client.DropDatabaseAsync(dropDb)
//let deleteDbResult = deleteDb.Result
//deleteDbResult

let newDb = "weather"
let createDb = client.CreateDatabaseAsync(newDb)
let createDbResult = createDb.Result
createDbResult

let databases = client.ShowDatabasesAsync()
let dbResult = databases.Result
dbResult

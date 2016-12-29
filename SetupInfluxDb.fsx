#load @"./paket-files/include-scripts/net46/include.influxdb.net-main.fsx"

open System
open InfluxDB.Net

let client = new InfluxDb("http://localhost:8086", "root", "root")

let databases = client.ShowDatabasesAsync()
let dbResult = databases.Result
dbResult

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

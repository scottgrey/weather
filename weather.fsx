#r "./packages/FSharp.Configuration/lib/net40/FSharp.Configuration.dll"
#r "./packages/FSharp.Configuration/lib/net40/SharpYaml.dll"
#r "./packages/FSharp.Data/lib/net40/FSharp.Data.dll"

open System
open FSharp.Configuration
open FSharp.Data

type Config = YamlConfig<"./sample-config.yaml", ReadOnly=true>

type Auth = JsonProvider<"./data/auth.json">

type NetatmoWeather = JsonProvider<"./data/weather.json">

let config = Config()
let configFilePath = "./config.yaml"

config.Load(configFilePath)

let clientId = config.Netatmo.Auth.ClientId
let clientSecret = config.Netatmo.Auth.ClientSecret
let username = config.Netatmo.Auth.Username
let password = config.Netatmo.Auth.Password

let authResponse clientId clientSecret username password =  
        Http.RequestString 
            (config.Netatmo.Contact.BaseUri.ToString() + config.Netatmo.Contact.AuthEndPoint, 
                body = FormValues [
                    ("grant_type", "password");
                    ("client_id", clientId);
                    ("client_secret", clientSecret);
                    ("username", username);
                    ("password", password);
                    ("scope", "read_station")]
            )

let weatherResponse accessToken = 
        Http.RequestString 
            (config.Netatmo.Contact.BaseUri.ToString() + config.Netatmo.Contact.DataEndPoint, 
                body = FormValues [
                    ("access_token", accessToken);
                    ]
            )
let toDateTime (timestamp: int) =
    let start = DateTime(1970,1,1,0,0,0,DateTimeKind.Utc)
    start.AddSeconds(float timestamp)

let toDateTimeLocal (timestamp: int) =
    let start = DateTime(1970,1,1,0,0,0,DateTimeKind.Utc)
    start.AddSeconds(float timestamp).ToLocalTime()
    
let auth = authResponse clientId clientSecret username password
let ar = Auth.Parse(auth)
let accessToken = ar.AccessToken

let weather = weatherResponse accessToken

let w = NetatmoWeather.Parse(weather)     

let b = w.Body
let u = b.User

let d = b.Devices

for dc in d do
    printfn "Indoor Temp: %f" dc.DashboardData.Temperature


for dc in d do
    let m = dc.Modules
    for md in m do
        printfn "%s" md.ModuleName

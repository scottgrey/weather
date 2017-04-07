namespace Weather

open System
open System.Net
open FSharp.Configuration
open FSharp.Data
open InfluxDB.FSharp


module Main =

    [<EntryPoint>]
    let main argv =

        let config = Config()
        let configFilePath = "./config.yaml"

        config.Load(configFilePath)
    
        let influxClient = Client("localhost")

        let rec loop () = async {
            let auth = authResponse config
            let ar = Auth.Parse(auth)
            let accessToken = ar.AccessToken

            let weather = weatherResponse accessToken config

            //printfn "auth: %s\n" auth

            //printfn "weather: %s\n" weather

            let w = NetatmoWeather.Parse(weather)

            printfn "Weather Status: %s" w.Status
            printfn "Time Executing: %f" w.TimeExec
            printfn "Time on Server(UTC): %s" (toDateTime(w.TimeServer).ToString())
            printfn "Time on Server(Local): %s" (toDateTimeLocal(w.TimeServer).ToString())

            let b = w.Body
            let u = b.User
            printfn "User: %s\n" u.Mail

            let d = b.Devices

            for dc in d do
                let indoor = getDevice dc 
                printfn "%A" (indoor.GetHashCode())
                printfn "%A" indoor

                for m in dc.Modules do
                    match m.ModuleName with
                    | "Outdoor" -> 
                        let outdoor = getOutdoorModule m
                        printfn "%A" (outdoor.GetHashCode())
                        printfn "%A" outdoor
                    | "Rain" ->
                        let rain = getRainModule m
                        printfn "%A" (rain.GetHashCode())
                        printfn "%A" rain
                    | _ -> printfn ""

            let dbs = influxClient.ShowDatabases() |> Async.RunSynchronously
            printfn "%A" dbs
            printfn ""
            printfn "sleeping for 60 seconds...."
            do! Async.Sleep(60000)
            printfn ""
            printfn "done sleeping!"
            printfn ""
            return! loop()
            }

        let cts = new System.Threading.CancellationTokenSource()
        Async.Start(loop (), cts.Token)
        Console.ReadLine() |> ignore
        printfn "Cancelling..."
        cts.Cancel()
        0 // return an integer exit code

open System
open FSharp.Configuration
open FSharp.Data

type Config = YamlConfig<"./sample-config.yaml", ReadOnly=true>

type Auth = JsonProvider<"./data/auth.json">

type NetatmoWeather = JsonProvider<"./data/weather.json">

type IndoorWeather = {
    stationName : string;
    timeUtc : int;
    lastStatusStore : int;
    dateSetup : int;
    lastSetup : int;
    lastUpgrade : int;
    co2Calibrating : bool;
    wifiStatus : int;
    firmware : int;
    temperature : float;
    tempTrend : string;
    humidity : int;
    pressure : decimal;
    pressureTrend : string;
    absolutePressure : decimal;
    noise : int;
    co2 : int;
    maxTemp : float;
    dateMaxTemp : int;
    minTemp : float;
    dateMinTemp : int;
}

type OutdoorWeather = {
    moduleName : string;
    batteryPercent : int;
    batteryVp : int;
    measurementTime : int;
    lastMessage : int;
    lastSeen : int;
    lastSetup : int;
    rfStatus : int;
    firmware : int;
    temperature : float;
    tempTrend : string;
    humidity : int;
    maxTemp : float;
    dateMaxTemp : int;
    minTemp : float;
    dateMinTemp : int;
}

type RainWeather = {
    moduleName : string;
    batteryPercent : int;
    batteryVp : int;
    measurementTime : int;
    lastMessage : int;
    lastSeen : int;
    lastSetup : int;
    rfStatus : int;
    firmware : int;
    rain : float;
    sumRain1 : float;
    sumRain24 : float;
}


module weather =
    let config = Config()
    let configFilePath = "./config.yaml"

    config.Load(configFilePath)

    let authResponse = 
        Http.RequestString 
            (config.Netatmo.Contact.BaseUri.ToString() + config.Netatmo.Contact.AuthEndPoint, 
                body = FormValues [
                    ("grant_type", "password");
                    ("client_id", config.Netatmo.Auth.ClientId);
                    ("client_secret", config.Netatmo.Auth.ClientSecret);
                    ("username", config.Netatmo.Auth.Username);
                    ("password", config.Netatmo.Auth.Password);
                    ("scope", "read_station")]
            )
    let weatherResponse accessToken = 
        Http.RequestString 
            (config.Netatmo.Contact.BaseUri.ToString() + config.Netatmo.Contact.DataEndPoint, 
                body = FormValues [
                    ("access_token", accessToken);
                    ]
            )

    let ctof temp = temp * 1.8 + 32.

    let mmtoin mes = mes / 25.4

    let toDateTime (timestamp: int) =
        let start = DateTime(1970,1,1,0,0,0,DateTimeKind.Utc)
        start.AddSeconds(float timestamp)

    let toDateTimeLocal (timestamp: int) =
        let start = DateTime(1970,1,1,0,0,0,DateTimeKind.Utc)
        start.AddSeconds(float timestamp).ToLocalTime()

    let getRainModule (rmd : NetatmoWeather.Module) = 
        let moduleName = rmd.ModuleName
        let batteryPercent = rmd.BatteryPercent
        let batteryVp = rmd.BatteryVp
        let measurementTime = rmd.DashboardData.TimeUtc
        let lastMessage = rmd.LastMessage
        let lastSeen = rmd.LastSeen
        let lastSetup = rmd.LastSetup
        let rfStatus = rmd.RfStatus
        let firmware = rmd.Firmware
        let rain = 
            let rn = rmd.DashboardData.Rain
            match rn with
            | Some r -> mmtoin (float(r))
            | None -> 0.0
        let sumRain1 = 
            let rn = rmd.DashboardData.SumRain1
            match rn with
            | Some r -> mmtoin (float(r))
            | None -> 0.0
        let sumRain24 = 
            let rn = rmd.DashboardData.SumRain24
            match rn with
            | Some r -> mmtoin (float(r))
            | None -> 0.0
        {
            moduleName = moduleName
            batteryPercent = batteryPercent
            batteryVp = batteryVp
            measurementTime = measurementTime
            lastMessage = lastMessage
            lastSeen = lastSeen
            lastSetup = lastSetup
            rfStatus = rfStatus
            firmware = firmware
            rain = rain
            sumRain1 = sumRain1
            sumRain24 = sumRain24
        }    

    let getOutdoorModule (omd : NetatmoWeather.Module) = 
        let moduleName = omd.ModuleName
        let batteryPercent = omd.BatteryPercent
        let batteryVp = omd.BatteryVp
        let measurementTime = omd.DashboardData.TimeUtc
        let lastMessage = omd.LastMessage
        let lastSeen = omd.LastSeen
        let lastSetup = omd.LastSetup
        let rfStatus = omd.RfStatus
        let firmware = omd.Firmware
        let temperature = 
            let t = omd.DashboardData.Temperature
            match t with
            | Some temp -> ctof (float(temp))
            | None -> 0.0
        let humidity = 
            let h = omd.DashboardData.Humidity
            match h with
            | Some hd -> hd
            | None -> 0
        let tempTrend = 
            let tt = omd.DashboardData.TempTrend
            match tt with
            | Some tt -> tt
            | None -> "Empty"
        let maxTemp =
            let mt = omd.DashboardData.MaxTemp
            match mt with
            | Some mt -> ctof (float(mt))
            | None -> 0.0
        let dateMaxTemp =
            let dmt = omd.DashboardData.DateMaxTemp
            match dmt with
            | Some dmt -> dmt
            | None -> 0
        let minTemp =
            let mt = omd.DashboardData.MinTemp
            match mt with
            | Some mt -> ctof (float(mt))
            | None -> 0.0
        let dateMinTemp =
            let dmt = omd.DashboardData.DateMinTemp
            match dmt with
            | Some dmt -> dmt
            | None -> 0
        
        {
            moduleName = moduleName
            batteryPercent = batteryPercent
            batteryVp = batteryVp
            measurementTime = measurementTime
            lastMessage = lastMessage
            lastSeen = lastSeen
            lastSetup = lastSetup
            rfStatus = rfStatus
            firmware = firmware
            temperature = temperature
            tempTrend = tempTrend
            humidity = humidity
            maxTemp = maxTemp
            dateMaxTemp = dateMaxTemp
            minTemp = minTemp
            dateMinTemp = dateMinTemp
        }    

    let getDevice (dc : NetatmoWeather.Devicis) =
        let stationName = dc.StationName
        let timeUtc = dc.DashboardData.TimeUtc
        let lastStatusStore = dc.LastStatusStore
        let dateSetup = dc.DateSetup
        let lastSetup = dc.LastSetup
        let lastUpgrade = dc.LastUpgrade
        let co2Calibrating = dc.Co2Calibrating
        let wifiStatus = dc.WifiStatus
        let firmware = dc.Firmware
        let temperature = (ctof (float(dc.DashboardData.Temperature)))
        let tempTrend = dc.DashboardData.TempTrend
        let humidity = dc.DashboardData.Humidity
        let pressure = dc.DashboardData.Pressure
        let pressureTrend = dc.DashboardData.PressureTrend
        let absolutePressure = dc.DashboardData.AbsolutePressure
        let noise = dc.DashboardData.Noise
        let co2 = dc.DashboardData.Co2
        let maxTemp = (ctof (float(dc.DashboardData.MaxTemp)))
        let dateMaxTemp = dc.DashboardData.DateMaxTemp
        let minTemp = (ctof (float(dc.DashboardData.MinTemp)))
        let dateMinTemp = dc.DashboardData.DateMinTemp

        (*printfn "Device: %s" stationName
        printfn ""
        printfn "Indoor measurement time(UTC): %s" (toDateTime(timeUtc).ToString())
        printfn "Indoor measurement time(Local): %s" (toDateTimeLocal(timeUtc).ToString())
        printfn "Indoor last status store time(UTC): %s" (toDateTime(lastStatusStore).ToString())
        printfn "Indoor last status store time(Local): %s" (toDateTimeLocal(lastStatusStore).ToString())
        printfn "Indoor setup time(UTC): %s" (toDateTime(dateSetup).ToString())
        printfn "Indoor setup time(Local): %s" (toDateTimeLocal(dateSetup).ToString())
        printfn "Indoor last setup time(UTC): %s" (toDateTime(lastSetup).ToString())
        printfn "Indoor last setup time(Local): %s" (toDateTimeLocal(lastSetup).ToString())
        printfn "Indoor last upgrade time(UTC): %s" (toDateTime(lastUpgrade).ToString())
        printfn "Indoor last upgrade time(Local): %s" (toDateTimeLocal(lastUpgrade).ToString())
        printfn "CO2 Calibrating: %b" dc.Co2Calibrating
        printfn "Indoor wifi status: %i" dc.WifiStatus
        printfn "Indoor firmware: %i" dc.Firmware
        printfn "Indoor Temp: %.2f" (ctof (float(dc.DashboardData.Temperature)))
        printfn "Indoor Temp trend: %s" dc.DashboardData.TempTrend
        printfn "Indoor Humidity: %i" dc.DashboardData.Humidity
        printfn "Pressure: %M" dc.DashboardData.Pressure
        printfn "Pressure trend: %s" dc.DashboardData.PressureTrend
        printfn "Absolute Presssure: %M" dc.DashboardData.AbsolutePressure
        printfn "Noise: %i" dc.DashboardData.Noise
        printfn "CO2: %i" dc.DashboardData.Co2
        printfn "Max Indoor Temp: %.2f" (ctof (float(dc.DashboardData.MaxTemp)))
        printfn "DateTime of Max Indoor Temp (UTC): %s" (toDateTime(dc.DashboardData.DateMaxTemp).ToString())
        printfn "DateTime of Max Indoor Temp (Local): %s" (toDateTimeLocal(dc.DashboardData.DateMaxTemp).ToString())
        printfn "Min Indoor Temp: %.2f" (ctof (float(dc.DashboardData.MinTemp)))
        printfn "DateTime of Min Indoor Temp (UTC): %s" (toDateTime(dc.DashboardData.DateMinTemp).ToString())
        printfn "DateTime of Min Indoor Temp (Local): %s" (toDateTimeLocal(dc.DashboardData.DateMinTemp).ToString())
*)
        //for m in dc.Modules do
        //    let mn = m.ModuleName
           (* printfn ""
            printfn "Module Name: %s" mn
            printfn "%s Battery Status: %i" mn m.BatteryPercent
            printfn "%s Battery volt: %i" mn m.BatteryVp
            printfn "%s measurement time(UTC): %s" mn (toDateTime(m.DashboardData.TimeUtc).ToString())
            printfn "%s measurement time(Local): %s" mn (toDateTimeLocal(m.DashboardData.TimeUtc).ToString())
            printfn "%s last message time(UTC): %s" mn (toDateTime(m.LastMessage).ToString())
            printfn "%s last message time(Local): %s" mn (toDateTimeLocal(m.LastMessage).ToString())
            printfn "%s last seen time(UTC): %s" mn (toDateTime(m.LastSeen).ToString())
            printfn "%s last seen time(Local): %s" mn (toDateTimeLocal(m.LastSeen).ToString())
            printfn "%s last setup time(UTC): %s" mn (toDateTime(m.LastSetup).ToString())
            printfn "%s last setup time(Local): %s" mn (toDateTimeLocal(m.LastSetup).ToString())
            printfn "%s rf status: %i" mn m.RfStatus
            printfn "%s firmware: %i" mn m.Firmware
            match mn with
                | "Outdoor" -> 
                    let ot = m.DashboardData.Temperature
                        
                    match ot with
                    | Some t -> 
                        printfn "%s Temp: %.2f" mn (ctof (float(t)))
                    | None -> ()
                    
                    let tt = m.DashboardData.TempTrend
                        
                    match tt with
                    | Some t -> printfn "%s Temp trend: %s" mn t
                    | None -> ()

                    let hy = m.DashboardData.Humidity

                    match hy with
                    | Some h -> printfn "%s Humidity: %i" mn h
                    | None -> ()

                    let dmaxt = m.DashboardData.DateMaxTemp
                    let maxt = m.DashboardData.MaxTemp

                    match maxt with
                    | Some t -> printfn "Max %s Temp: %.2f" mn (ctof (float(t)))
                    | None -> ()

                    match dmaxt with
                    | Some t -> 
                        printfn "DateTime of Max %s Temp (UTC): %s" mn (toDateTime(t).ToString())
                        printfn "DateTime of Max %s Temp (Local): %s" mn (toDateTimeLocal(t).ToString())
                    | None -> ()

                    let dmint = m.DashboardData.DateMinTemp
                    let mint = m.DashboardData.MinTemp

                    match mint with
                    | Some t -> printfn "Min %s Temp: %.2f" mn (ctof (float(t)))
                    | None -> ()

                    match dmint with
                    | Some t -> 
                        printfn "DateTime of Min %s Temp (UTC): %s" mn (toDateTime(t).ToString())
                        printfn "DateTime of Min %s Temp (Local): %s" mn (toDateTimeLocal(t).ToString())
                    | None -> ()

                | "Rain" -> 
                    let rn = m.DashboardData.Rain

                    match rn with
                    | Some r -> printfn "%s since last measurement: %.4f" mn (mmtoin (float(r)))
                    | None -> ()
                        
                    let rn1 = m.DashboardData.SumRain1

                    match rn1 with
                    | Some r -> printfn "%s in last hour: %.4f" mn (mmtoin (float(r)))
                    | None -> ()

                    let rn24 = m.DashboardData.SumRain24

                    match rn24 with
                    | Some r -> printfn "%s in last 24 hours: %.4f" mn (mmtoin (float(r)))
                    | None -> ()

                | _ -> ()
            printfn ""*)
        {
            stationName = stationName
            timeUtc = timeUtc
            lastStatusStore = lastStatusStore
            dateSetup = dateSetup
            lastSetup = lastSetup
            lastUpgrade = lastUpgrade
            co2Calibrating = co2Calibrating
            wifiStatus = wifiStatus
            firmware = firmware
            temperature = temperature
            tempTrend = tempTrend
            humidity = humidity
            pressure = pressure
            pressureTrend = pressureTrend
            absolutePressure = absolutePressure
            noise = noise
            co2 = co2
            maxTemp = maxTemp
            dateMaxTemp = dateMaxTemp
            minTemp = minTemp
            dateMinTemp = dateMinTemp
        }

 

    [<EntryPoint>]
    let main argv =
        let auth = authResponse
        let ar = Auth.Parse(auth)
        let accessToken = ar.AccessToken

        let weather = weatherResponse accessToken

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


        let console = Console.ReadLine()
        0 // return an integer exit code

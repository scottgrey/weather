namespace Weather

open System
open System.Net
open FSharp.Configuration
open FSharp.Data

[<AutoOpen>]
module Netatmo =

    let weatherResponse accessToken (config: Config) = 
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
        let measurementTime = toDateTime(rmd.DashboardData.TimeUtc)
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
        let measurementTime = toDateTime(omd.DashboardData.TimeUtc)
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
        let measurementTime = toDateTime(dc.DashboardData.TimeUtc)
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
        {
            stationName = stationName
            measurementTime = measurementTime
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

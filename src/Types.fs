namespace Weather

open System
open System.Net
open FSharp.Configuration
open FSharp.Data

type Config = YamlConfig<"../sample-config.yaml", ReadOnly=true>

type Auth = JsonProvider<"../data/auth.json">

type NetatmoWeather = JsonProvider<"../data/weather.json">

type IndoorWeather = {
    stationName : string;
    measurementTime : DateTime;
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
    measurementTime : DateTime;
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
    measurementTime : DateTime;
    lastMessage : int;
    lastSeen : int;
    lastSetup : int;
    rfStatus : int;
    firmware : int;
    rain : float;
    sumRain1 : float;
    sumRain24 : float;
}

namespace Weather

open System
open System.Net
open FSharp.Configuration
open FSharp.Data

[<AutoOpen>]
module WeatherConfig = 

    let authResponse (config: Config)  = 
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
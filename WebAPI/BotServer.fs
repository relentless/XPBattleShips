open Nancy
open Nancy.Extensions
open Nancy.Hosting.Self
open System
open FSharp.Data
open Bot
open System.IO

let bot = Bot()

type Start = JsonProvider<"{ \"maxTurns\" : 10, \"gridSize\" : \"B2\", \"players\"  : [\"P1\",\"p2\"], \"ships\" : [\"S1\",\"S2\"], \"mineCount\" : 5}" >

type Place = JsonProvider<"{\"gridReferences\" : [\"C12\", \"D12\"]}">

let logFile = sprintf """C:/Programming/XPBattleShips/Logs/BattleLog%i-%i-%i.txt""" System.DateTime.Now.Hour System.DateTime.Now.Minute System.DateTime.Now.Millisecond

let logToFile (text:string) =
    System.IO.File.AppendAllText(logFile, text)

let logRequest title requestBody =
    printfn "%s\n%s" title requestBody
    let time = System.DateTime.Now
    logToFile (sprintf "%i:%i:%i : %s%s" time.Minute time.Second time.Millisecond title System.Environment.NewLine)
    logToFile requestBody

type FakeServer() as self = 
    inherit NancyModule()

    do
        
        self.Post.["/START"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                requestBody |> logRequest "START"
                
                let bodyJson = Start.Parse(requestBody)
                bot.SetupGrid(bodyJson.GridSize, bodyJson.MaxTurns, bodyJson.Ships, bodyJson.MineCount)
                200 :> obj

        self.Get.["/PLACE"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                requestBody |> logRequest "PLACE (Get)"

                let response = 
                    match bot.GridLength >= 8 with
                    | true -> 
                        let position = bot.PlaceShip()
                        sprintf "{\"gridReference\": \"%s%i\",\"orientation\": \"%s\"}" (position.GridLetter) (position.GridNumber) (position.Orientation) 
                        |> Nancy.Response.op_Implicit 
                    | _ -> "nowt" |> Nancy.Response.op_Implicit 

                response.StatusCode <- HttpStatusCode.OK
                200 :> obj

        self.Post.["/PLACE"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                requestBody |> logRequest "PLACE (Get)"
                let placeJson = Place.Parse(requestBody)
                bot.MyShips(placeJson.GridReferences)
                200 :> obj

        self.Post.["/SCAN"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                requestBody |> logRequest "SCAN"
                200 :> obj

        self.Get.["/MOVE"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                requestBody |> logRequest "MOVE (Get)"
               // let place = bot.NextMove()
               // let response = "{\"type\": \"ATTACK\", \"gridReference\" : \"" + place + "\"}" |> Nancy.Response.op_Implicit 
                
                let place = bot.NextMoveByType()
                let response = "{\"type\": \"" + place.Type + "\", \"gridReference\" : \"" + place.GridReference + "\"}" |> Nancy.Response.op_Implicit 
                

                response.StatusCode <- HttpStatusCode.OK
                response :> obj

        self.Post.["/HIT"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                requestBody |> logRequest "HIT"
                200 :> obj

        self.Post.["/HIT_MINE"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                requestBody |> logRequest "HIT_MINE"
                200 :> obj

        self.Post.["/MISS"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                requestBody |> logRequest "MISS"
                200 :> obj

        self.Get.["/"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                requestBody |> logRequest "INDEX"
                let response = "Grant and Simon's Bot: XPSMan!" |> Nancy.Response.op_Implicit 
                response.StatusCode <- HttpStatusCode.OK
                response :> obj
[<EntryPoint>]
let main argv = 
    
    let hostConfig = new HostConfiguration()
    hostConfig.UrlReservations<-UrlReservations(CreateAutomatically=true)

    let nancyHost = 
        new NancyHost(
            hostConfig, 
            new Uri("http://localhost:80/Bot/"))

    nancyHost.Start()

    printfn "Nancy host running on http://localhost:80/Bot"

    Console.ReadLine()
    

    0 // return an integer exit code

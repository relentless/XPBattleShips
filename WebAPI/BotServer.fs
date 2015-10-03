﻿open Nancy
open Nancy.Extensions
open Nancy.Hosting.Self
open System
open FSharp.Data
open Bot

let bot = Bot()

type Start = JsonProvider<"{ \"maxTurns\" : 10, \"gridSize\" : \"B2\", \"players\"  : [\"P1\",\"p2\"], \"ships\" : [\"S1\",\"S2\"], \"mineCount\" : 5}" >

type FakeServer() as self = 
    inherit NancyModule()

    do
        self.Get.["/"] <- 
            fun _ -> 
                printfn "Index\n"
                let response = "Grant and Simon's Bot" |> Nancy.Response.op_Implicit 
                response.StatusCode <- HttpStatusCode.OK
                response :> obj

        self.Post.["/START"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                printfn "START\n%s" requestBody

                let body = Start.Parse(requestBody)
                bot.SetupGrid(body.GridSize)
                200 :> obj

        self.Get.["/PLACE"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                printfn "PLACE (Get)\n%s" requestBody
                let response = "Response" |> Nancy.Response.op_Implicit 
                response.StatusCode <- HttpStatusCode.OK
                200 :> obj

        self.Post.["/PLACE"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                printfn "PLACE (Post)\n%s" requestBody
                200 :> obj

        self.Post.["/SCAN"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                printfn "SCAN\n%s" requestBody
                200 :> obj

        self.Get.["/MOVE"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                printfn "MOVE\n%s" requestBody

                
                let place = bot.NextMove()

                let response = "{\"type\": \"ATTACK\", \"gridReference\" : \"" + place + "\"}" |> Nancy.Response.op_Implicit 
                response.StatusCode <- HttpStatusCode.OK
                //response.ContentType <- "application/json"
                response :> obj

        self.Post.["/HIT"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                printfn "HIT\n%s" requestBody
                200 :> obj

        self.Post.["/HIT_MINE"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                printfn "HIT_MINE\n%s" requestBody
                200 :> obj

        self.Post.["/MISS"] <- 
            fun _ -> 
                let requestBody = self.Request.Body.AsString()
                printfn "MISS\n%s" requestBody
                //let response = "Response" |> Nancy.Response.op_Implicit 
                //response.StatusCode <- HttpStatusCode.OK
                200 :> obj

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

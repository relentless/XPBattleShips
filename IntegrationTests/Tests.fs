﻿module Tests

open NUnit.Framework
open FsUnit
open HttpClient
open System

[<TestFixture>]
type ``Integration tests`` ()=

    member x.StartRequest() =
        createRequest Post "http://localhost/Bot/START" 
        |> withBody "{ \"maxTurns\" : 10, \"gridSize\" : \"B2\", \"players\"  : [\"P1\",\"p2\"], \"ships\" : [\"S1\",\"S2\"], \"mineCount\" : 5}"

    member x.NewGame() =
        x.StartRequest() |> getResponseCode |> ignore

    member x.Move() =
        createRequest Get "http://localhost/Bot/MOVE" |> getResponseCode |> ignore

    member x.CheckMoveEquals(position:string) =
        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal ("{\"type\": \"ATTACK\", \"gridReference\" : \"" + position + "\"}")

    member x.CheckMoveEqualsType(moveType:string) =
        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response.Substring(10, 4) |> should equal moveType

    member x.GetRidOfMines() =
        x.Move()
        x.Move()
        x.Move()
        x.Move()
        x.Move()

    [<Test>]
    member x.``Start Get gives a 200`` () =
        x.StartRequest()
        |> getResponseCode |> should equal 200

    [<Test>]
    member x.``Place post doesn't break anything`` () =
        createRequest Post "http://localhost/Bot/PLACE" 
        |> withBody "{\"gridReferences\" : [\"C12\", \"D12\"]}"
        |> getResponseCode |> should equal 200

    [<Test>]
    member x.``Place get with grid size under 8 does nothing useful`` () =
        createRequest Get "http://localhost/Bot/PLACE" 
        |> getResponseBody |> should equal ""

    [<Test>]
    member x.``First five moves are mines`` () =
        x.NewGame()
        x.CheckMoveEqualsType("MINE")
        x.CheckMoveEqualsType("MINE")
        x.CheckMoveEqualsType("MINE")
        x.CheckMoveEqualsType("MINE")
        x.CheckMoveEqualsType("MINE")

    [<Test>]
    member x.``Mines reset for new game`` () =
        x.NewGame()
        x.CheckMoveEqualsType("MINE")
        x.CheckMoveEqualsType("MINE")
        x.CheckMoveEqualsType("MINE")
        x.CheckMoveEqualsType("MINE")
        x.CheckMoveEqualsType("MINE")
        x.NewGame()
        x.CheckMoveEqualsType("MINE")
        x.CheckMoveEqualsType("MINE")
        x.CheckMoveEqualsType("MINE")
        x.CheckMoveEqualsType("MINE")
        x.CheckMoveEqualsType("MINE")
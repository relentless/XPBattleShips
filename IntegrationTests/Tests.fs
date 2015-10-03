module Tests

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

    member x.CheckMoveEquals(position:string) =
        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal ("{\"type\": \"ATTACK\", \"gridReference\" : \"" + position + "\"}")

    [<Test>]
    member x.``Start Get gives a 200`` () =
        x.StartRequest()
        |> getResponseCode |> should equal 200

    [<Test>]
    member x.``Move Get attacks increasing positions`` () =

        x.NewGame()
        x.CheckMoveEquals("A1")
        x.CheckMoveEquals("A2")

    [<Test>]
    member x.``Move starts again with each game`` () =

        x.NewGame()
        x.CheckMoveEquals("A1")
        x.NewGame()
        x.CheckMoveEquals("A1")

    [<Test>]
    member x.``Grid size taken from Start`` () =

        x.NewGame()
        x.CheckMoveEquals("A1")
        x.CheckMoveEquals("A2")
        x.CheckMoveEquals("B1")
        x.CheckMoveEquals("B2")

    [<Test>]
    member x.``Place post doesn't break anything`` () =
        createRequest Post "http://localhost/Bot/PLACE" 
        |> withBody "{\"gridReferences\" : [\"C12\", \"D12\"]}"
        |> getResponseCode |> should equal 200

//    [<Test>]
//    member x.``Place get with grid size under 8 does nothing useful`` () =
//        createRequest Get "http://localhost/Bot/PLACE" 
//        |> withBody "{\"gridReferences\" : [\"C12\", \"D12\"]}"
//        |> getResponseCode |> should equal 200
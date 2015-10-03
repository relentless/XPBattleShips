module Tests

open NUnit.Framework
open FsUnit
open HttpClient
open System



[<TestFixture>]
type ``Integration tests`` ()=

    [<Test>]
    member x.``Start Get gives a 200`` () =

        createRequest Post "http://localhost/Bot/START" 
        |> withBody "{ \"maxTurns\" : 10, \"gridSize\" : \"B2\", \"players\"  : [\"P1\",\"p2\"], \"ships\" : [\"S1\",\"S2\"], \"mineCount\" : 5}" 
        |> getResponseCode |> should equal 200

    [<Test>]
    member x.``Move Get attacks increasing positions`` () =

        createRequest Post "http://localhost/Bot/START" 
        |> withBody "{ \"maxTurns\" : 10, \"gridSize\" : \"B2\", \"players\"  : [\"P1\",\"p2\"], \"ships\" : [\"S1\",\"S2\"], \"mineCount\" : 5}" 
        |> getResponseCode |> ignore

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"A1\"}"

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"A2\"}"

    [<Test>]
    member x.``Move starts again with each game`` () =

        createRequest Post "http://localhost/Bot/START" 
        |> withBody "{ \"maxTurns\" : 10, \"gridSize\" : \"B2\", \"players\"  : [\"P1\",\"p2\"], \"ships\" : [\"S1\",\"S2\"], \"mineCount\" : 5}" 
        |> getResponseCode |> ignore

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"A1\"}"

        createRequest Post "http://localhost/Bot/START" 
        |> withBody "{ \"maxTurns\" : 10, \"gridSize\" : \"B2\", \"players\"  : [\"P1\",\"p2\"], \"ships\" : [\"S1\",\"S2\"], \"mineCount\" : 5}" 
        |> getResponseCode |> ignore

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"A1\"}"

    [<Test>]
    member x.``Gri size taken from Start`` () =

        createRequest Post "http://localhost/Bot/START" 
        |> withBody "{ \"maxTurns\" : 10, \"gridSize\" : \"B2\", \"players\"  : [\"P1\",\"p2\"], \"ships\" : [\"S1\",\"S2\"], \"mineCount\" : 5}" 
        |> getResponseCode |> ignore

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"A1\"}"

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"A2\"}"

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"B1\"}"

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"B2\"}"
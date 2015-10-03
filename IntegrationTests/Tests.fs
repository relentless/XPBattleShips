module Tests

open NUnit.Framework
open FsUnit
open HttpClient
open System



[<TestFixture>]
type ``Integration tests`` ()=

    [<Test>]
    member x.``Start Get gives a 200`` () =

        let response = createRequest Post "http://localhost/Bot/START" |> withBody "hi" |> getResponseCode
        response |> should equal 200

    [<Test>]
    member x.``Move Get attacks increasing positions`` () =

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"A1\"}"

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"B1\"}"

    [<Test>]
    member x.``Move starts again with each game`` () =

        createRequest Post "http://localhost/Bot/START" |> withBody "hi" |> getResponseCode |> ignore

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"A1\"}"

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"B1\"}"

        createRequest Post "http://localhost/Bot/START" |> withBody "hi" |> getResponseCode |> ignore

        let response = createRequest Get "http://localhost/Bot/MOVE" |> getResponseBody
        response |> should equal "{\"type\": \"ATTACK\", \"gridReference\" : \"A1\"}"
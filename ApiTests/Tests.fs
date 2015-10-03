module Tests

open NUnit.Framework
open FsUnit
open HttpClient
open Nancy
open Nancy.Extensions
open Nancy.Hosting.Self
open System

let hostConfig = new HostConfiguration()
hostConfig.AllowChunkedEncoding <- false
// Enabling chunked encoding breaks HEAD requests if you're self-hosting.
// It also seems to mean the Content-Length isn't set in some cases.
hostConfig.UrlReservations<-UrlReservations(CreateAutomatically=true)

let nancyHost = 
    new NancyHost(
        hostConfig, 
        new Uri("http://localhost:80/TestServer/"))

[<TestFixture>]
type ``Integration tests`` ()=

    [<TestFixtureSetUp>]
    member x.fixtureSetup() =
        nancyHost.Start()

    [<Test>]
    // This needs to be run first, as the keep-alive is only set on the first call.  They seem to be run alphabetically.
    member x.``_if KeepAlive is true, Connection set to 'Keep-Alive' on the first request, but not subsequent ones`` () =

        let response = createRequest Get "http://localhost/Bot/START" |> getResponseCode
        response |> should equal 200


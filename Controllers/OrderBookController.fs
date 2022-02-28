namespace OrderBookApi.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open OrderBookRepository

[<AutoOpen>]
module Helpers =
    let asResponse (controller: ControllerBase) result =
        match result with
        | Some result -> result |> controller.Ok :> IActionResult
        | None -> controller.NotFound "Not found" :> IActionResult

[<ApiController>]
type OrderBookController (logger : ILogger<OrderBookController>) =
    inherit ControllerBase()

    [<Route("[controller]")>]
    member this.Get() : IActionResult =
        getBooks()
        |> asResponse this

    [<Route("[controller]/{securitySimbol}")>]
    member this.Get(securitySimbol) : IActionResult =
        getOrderBook securitySimbol
        |> asResponse this

    [<Route("[controller]/limitorder")>]
    [<HttpPost>]
    member this.Post(data : PostData) : IActionResult =
        data
        |> postLimitOrderBook
        |> asResponse this

    [<Route("[controller]/limitorder")>]
    [<HttpDelete>]
    member this.Delete(data:DeleteData) : IActionResult =
        data
        |> deleteLimitOrderBook
        |> asResponse this

    [<Route("[controller]/marketorder")>]
    [<HttpPost>]
    member this.Post(data : PostMarketOrder) : IActionResult =
        data
        |> postMarketOrderBook
        |> asResponse this

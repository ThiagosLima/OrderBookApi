namespace OrderBookApi.Controllers

open Microsoft.AspNetCore.Mvc
open OrderBookApi
open Orders
open Microsoft.Extensions.Logging

type PostData =
    { OrderId : OrderId
      SecuritySimbol: SecuritySimbol
      OrderDirection : OrderDirection
      Size : Size
      LimitPrice : LimitPrice }

type DeleteData =
  { OrderId : OrderId
    SecuritySimbol: SecuritySimbol
    OrderDirection : OrderDirection }

module OrderBookRepository =
    let mutable books =
        [ { SecuritySimbol = "INTC"; Name = "Intel Corporation"; Bids = []; Asks = [] }
          { SecuritySimbol = "APPL"; Name = "Apple Corporation"; Bids = []; Asks = [] } ]

    let updateBooks newBooks =
        books <- match newBooks with
                 | Some updatedBooks -> updatedBooks
                 | None -> books
        newBooks

    let getBooks() = Some books

    let getOrderBook securitySimbol =
        books |> List.tryFind(fun book -> book.SecuritySimbol = securitySimbol)

    let postLimitOrderBook (data:PostData) =
        tryCreateOrder data.OrderId data.Size data.LimitPrice
        |> Option.map (addLimitOrder books data.SecuritySimbol data.OrderDirection)
        |> updateBooks

    let deleteLimitOrderBook (data:DeleteData) =
        tryCancelLimitOrder books data.OrderId data.SecuritySimbol data.OrderDirection
        |> updateBooks

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
        OrderBookRepository.getBooks()
        |> asResponse this

    [<Route("[controller]/{securitySimbol}")>]
    member this.Get(securitySimbol) : IActionResult =
        OrderBookRepository.getOrderBook securitySimbol
        |> asResponse this

    [<Route("[controller]/limitorder")>]
    [<HttpPost>]
    member this.Post(data : PostData) : IActionResult =
        data
        |> OrderBookRepository.postLimitOrderBook
        |> asResponse this

    [<Route("[controller]/limitorder")>]
    [<HttpDelete>]
    member this.Delete(data:DeleteData) : IActionResult =
        data
        |> OrderBookRepository.deleteLimitOrderBook
        |> asResponse this

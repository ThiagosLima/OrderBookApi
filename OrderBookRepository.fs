module OrderBookRepository

open OrderBookApi
open Orders

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

type PostMarketOrder =
    { OrderId : OrderId
      SecuritySimbol: SecuritySimbol
      OrderDirection : OrderDirection
      Size : Size }

type PostMarketReturn =
    { Books : Books
      Market : Market }

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

let postMarketOrderBook (data:PostMarketOrder) : PostMarketReturn option =
    let (updatedBooks, market) =
        marketOrder books data.OrderId data.SecuritySimbol data.OrderDirection data.Size

    Some updatedBooks |> updateBooks |> ignore

    Some { Books = updatedBooks; Market = market }
module Orders

open System
open OrderBookApi

// Bids são ordenadas pelo BidPrice descendente, depois por Time ascendente
let insertBid (bid : Order) (bids : Order list) =
  let index = List.tryFindIndexBack (fun b -> bid.Price <= b.Price ) bids

  match index with
  | Some i ->
    if bid.Time > bids.[i].Time then
      List.insertAt (i+1) bid bids
    else
      List.insertAt (i) bid bids
  | None -> List.insertAt 0 bid bids

// Asks são ordenadas pelo AskPrice ascendente, depois por Time ascendente
let insertAsk (ask : Order) (asks : Order list) =
  let index = List.tryFindIndexBack (fun a -> ask.Price >= a.Price ) asks

  match index with
  | Some i -> 
    if ask.Time > asks.[i].Time then
      List.insertAt (i+1) ask asks
    else
      List.insertAt (i) ask asks
  | None -> List.insertAt 0 ask asks

let tryCreateOrder orderId size price : Order option =
    try
      Some { OrderId = string orderId
             Time = DateTime.UtcNow
             Size = size
             Price =  price }
    with
      | _ -> None

let addLimitOrder books securitySimbol orderDirection order =  
  match List.tryFindIndex (fun book -> book.SecuritySimbol = securitySimbol) books with
  | Some index ->
    let updatedBook = 
      match orderDirection with
      | "Buy"  -> { books.[index] with Bids = insertBid order books.[index].Bids }
      | _ -> { books.[index] with Asks = insertAsk order books.[index].Asks }
  
    List.updateAt index updatedBook books
  | None -> books
  
let cancelLimitOrder books orderId securitySimbol orderDirection =
  let index = List.findIndex (fun book -> book.SecuritySimbol = securitySimbol) books

  let updatedBook = 
    match orderDirection with
    | "Buy"  -> 
      { books.[index] with Bids = List.filter (fun b -> b.OrderId <> orderId) books.[index].Bids }
    | _ -> 
      { books.[index] with Asks = List.filter (fun b -> b.OrderId <> orderId) books.[index].Asks }
  
  List.updateAt index updatedBook books

let updateMarket (orders: Order list, market:Market) order:Order : Order list * Market =
  match market.Size with
  | 0 -> 
    (orders@[order], market)
  | _ ->
    match market.Size - order.Size with
    | result when result >= 0 -> 
      (orders, { market with Size = result
                             Shares = { SharePrice = order.Price; ShareSize = order.Size } :: market.Shares
                             TakenOrders = { Order = order.OrderId
                                             OrderStatus = Filled } :: market.TakenOrders })
    | result ->
      let updateSize = abs result
      let updatedOrder = { order with Size = updateSize }
      (orders@[updatedOrder], { market with Size = 0
                                            Shares = { SharePrice = order.Price; ShareSize = order.Size - updateSize } :: market.Shares
                                            TakenOrders = { Order = order.OrderId
                                                            OrderStatus = Unfilled updateSize } :: market.TakenOrders })

let formatShares shares =
  shares
  |> List.groupBy (fun share -> share.SharePrice)   
  |> List.map (fun (price, size) -> { SharePrice = price 
                                      ShareSize = (List.sumBy (fun s -> s.ShareSize) size) })

let getAverage (shares : Share list) : decimal =
  List.sumBy (fun share -> decimal (share.ShareSize)) shares
  |> (/) (List.sumBy (fun share -> (share.SharePrice * decimal share.ShareSize)) shares)
  

let marketOrder books orderId time securitySimbol orderDirection size =
  let index = List.findIndex (fun book -> book.SecuritySimbol = securitySimbol) books
  let initialMarket = { Size = size; TakenOrders = []; Shares = []; Average = 0m }

  let (book, market) = 
    match orderDirection with
    | "Sell"  -> 
      let (bids, market) = List.fold updateMarket ([], initialMarket) books.[index].Bids
      ({ books.[index] with Bids = bids }, market)
    | _ -> 
      let (asks, market) = List.fold updateMarket ([], initialMarket) books.[index].Asks
      ({ books.[index] with Asks = asks }, market)
    
  let status = if market.Size = 0 then Filled else Unfilled market.Size
  (List.updateAt index book books, { market with Shares = formatShares market.Shares
                                                          Average = getAverage (formatShares market.Shares)
                                                          TakenOrders = { Order = orderId
                                                                          OrderStatus = status } :: market.TakenOrders })

namespace OrderBookApi

open System

type SecuritySimbol = string

type OrderDirection = string
// type OrderDirection =
//   | Buy
//   | Sell

type LimitPrice = decimal

type Size = int

type OrderId = string

type Order =
    { OrderId : OrderId
      Time : DateTime
      Size : Size
      Price : LimitPrice }

type OrderBook =
    { SecuritySimbol: SecuritySimbol
      Name : string
      Bids : Order list
      Asks : Order list }

type Books = OrderBook list

type OrderStatus =
    | Filled
    | Unfilled of Size

type TakenOrder =
    { Order: OrderId
      OrderStatus: OrderStatus }

type Share =
    { SharePrice : decimal
      ShareSize: Size }

type Market =
    { Size : Size
      TakenOrders : TakenOrder list
      Shares : Share list
      Average : decimal }

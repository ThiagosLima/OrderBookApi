# Order Book API

This project replicate an order book. You can get more information about how it works in this [video](https://www.youtube.com/watch?v=Iaiw5iGjXbw).

## How to run

On terminal run the command:

```bash
$ dotnet restore
$ dotnet run
```

It will prompt on the terminal the `PORT` it will listen. Ex:

```bash
$ dotnet run
Compilando...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7295
```

So you can access the API on port `7295`.

## Endpoints

There are two endpoints on the API `limitorder` to access the limit order resouce and `marketorder` to access the market order resouce.

### GET orderbooks

There are an in memory list of order books.

Route: `https://localhost:PORT/orderbook`

Result example:

```json
[
  {
    "securitySimbol": "INTC",
    "name": "Intel Corporation",
    "bids": [],
    "asks": []
  },
  {
    "securitySimbol": "APPL",
    "name": "Apple Corporation",
    "bids": [],
    "asks": []
  }
]
```

## GET orderbook

To access a specific order book pass the `securitySimbol`.

Route: `https://localhost:PORT/orderbook/INTC`

Result example:

```json
{
  "securitySimbol": "INTC",
  "name": "Intel Corporation",
  "bids": [],
  "asks": []
}
```

## POST limit order

To add a limit order book.

Route: `https://localhost:PORT/orderbook/limitorder`

### To buy

Body example:

```json
{
  "OrderId": "0101",
  "SecuritySimbol": "INTC",
  "OrderDirection": "Buy",
  "Size": 200,
  "LimitPrice": 33.75
}
```

### To sell

Body example:

```json
{
  "OrderId": "0103",
  "SecuritySimbol": "INTC",
  "OrderDirection": "Sell",
  "Size": 300,
  "LimitPrice": 33.78
}
```

## DELETE limit order

To cancel a limit order book.

Route: `https://localhost:PORT/orderbook/limitorder`

Body example:

```json
{
  "OrderId": "0105",
  "SecuritySimbol": "INTC",
  "OrderDirection": "Sell"
}
```

## POST market order

To add a market order book.

Route: `https://localhost:PORT/orderbook/marketorder`

Body example:

```json
{
  "OrderId": "0106",
  "SecuritySimbol": "INTC",
  "OrderDirection": "Sell",
  "Size": 100
}
```

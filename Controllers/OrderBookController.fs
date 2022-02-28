namespace OrderBookApi.Controllers

open Microsoft.AspNetCore.Mvc
open OrderBookApi

[<ApiController>]
[<Route("[controller]")>]
type OrderBookController () =
  inherit ControllerBase()

  member _.Get() =
    [ { SecuritySimbol = "INTC"; Name = "Intel Corporation"; Bids = []; Asks = []} 
      { SecuritySimbol = "APPL"; Name = "Apple Corporation"; Bids = []; Asks = []} ] 

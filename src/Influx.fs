namespace Weather

module Influx =

    let Ok a: Choice<_, _> = Choice1Of2 a
    let Fail a: Choice<_, _> = Choice2Of2 a

    let inline ok _ = Ok ()

    let (|Ok|Fail|) =
        function
        | Choice1Of2 a -> Ok a
        | Choice2Of2 a -> Fail a

    let notFail =
        function
        | Ok x -> x
        | Fail err -> failwithf "unexpectedly return Fail %+A" err
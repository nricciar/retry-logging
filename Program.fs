namespace LogRetry

open System
open Logging

module Program =
    let private log = Log.create "LogRetry.Program"

    let retryableTask () =
        async {
            printfn "foo"
            raise (exn "foo")
        }

    [<EntryPoint>]
    let main argv =
        async {
            let reqId = Guid.NewGuid().ToString()
            let! ret =
                retryableTask ()
                |> Async.retryBackoff 10 Backoff.ExponentialBoundedRandomized
                |> Async.Catch
                |> Log.logException log reqId "Test"
                |> Log.logElapsed log reqId "Test"
            ()
        }
        |> Async.RunSynchronously

        0 // return an integer exit code

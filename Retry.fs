namespace LogRetry

module Backoff =

    // FIXME: not implemented
    let ExponentialBoundedRandomized = ()
    let None = ()

module Async =
    // Retries a funciton until the specified attempts are exhausted.
    // If the attempts are exhausted, the last exception is raised
    // Returns the function result on success, if the attempts are not exhausted
    let rec retryBackoff attempts backoff f =
        async {
            match! f |> Async.Catch with
            | Choice1Of2 ret -> return ret
            | Choice2Of2 exn ->
                if attempts > 1 then
                    return! retryBackoff (attempts - 1) backoff f
                else
                    raise exn
        }

    // Retries a function indefinitely, logging each occurrence of an exception.
    // Returns the function result on success. Exceptions that can be retried
    // indefinitely can be defined in the filter parameter, or () -> true for all exceptions
    let rec retryIndefinitely log (filter:exn -> bool) f =
        async {
            match! f |> Async.Catch with
            | Choice1Of2 ret -> return ret
            | Choice2Of2 exn ->
                if filter exn then
                    return! retryIndefinitely log filter f
                else
                    raise exn
        }
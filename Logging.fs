namespace LogRetry

module Logging =
    type Logger (name) =
        member x.Error s = 
            eprintf "[%A] %s - " System.DateTime.UtcNow name
            eprintfn s
        member x.Info s = 
            printf "[%A] %s - " System.DateTime.UtcNow name
            printfn s

    module Log =
        open System.Diagnostics
        let private sw = Stopwatch.StartNew()

        let create name = Logger(name)

        let logException (log:Logger) (id:string) (tag:string) f =
            async {
                let! result = f
                match result with
                | Choice1Of2 s -> ()
                | Choice2Of2 (ex:exn) -> log.Error "%s: Failure encountered %s - %A" tag id ex
                return result
            }
    
        let logElapsed (log:Logger) id tag f =
            async {
                let before = sw.ElapsedMilliseconds
                let! result = f
                let after = sw.ElapsedMilliseconds
                log.Info "%s: %s; Time taken=%dms" tag id (after-before)
                return result
            }
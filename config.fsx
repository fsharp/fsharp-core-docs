#r "_lib/Fornax.Core.dll"

open Config

let config =
    { Generators =
          [ { Script = "apiref.fsx"
              Trigger = Once
              OutputFile = NewFileName "index.html" }
            { Script = "lunr.fsx"
              Trigger = Once
              OutputFile = NewFileName "index.json" } ] }

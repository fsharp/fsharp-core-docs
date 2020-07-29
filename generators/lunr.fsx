#r "../_lib/Fornax.Core.dll"
#r "../packages/Newtonsoft.Json/lib/netstandard2.0/Newtonsoft.Json.dll"
#r "../packages/FSharp.Formatting/lib/netstandard2.0/FSharp.Formatting.ApiDocs.dll"

#if !FORNAX
#load "../loaders/apirefloader.fsx"
#load "../loaders/globalloader.fsx"
#endif

open Apirefloader
open FSharp.Formatting.ApiDocs
let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    let all = ctx.TryGetValues<AssemblyEntities>()
    let siteInfo = ctx.TryGetValue<SiteInfo>() |> Option.get
    let refs =
      match all with
      | None -> [| |]
      | Some all ->
        match List.ofSeq all with
        | [model] ->
            let model = { model.GeneratorOutput with
                                CollectionRootUrl = siteInfo.root_url + "/reference/FSharp.Core" }
            ApiDocs.GenerateSearchIndexFromModel model
        | _ ->
            [| |]

    refs
    |> Newtonsoft.Json.JsonConvert.SerializeObject

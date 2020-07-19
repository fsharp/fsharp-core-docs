#r "../_lib/Fornax.Core.dll"
#r "../packages/Newtonsoft.Json/lib/netstandard2.0/Newtonsoft.Json.dll"
#r "../packages/FSharp.Formatting/lib/netstandard2.0/FSharp.Formatting.ApiDocs.dll"

#if !FORNAX
#load "../loaders/apirefloader.fsx"
#load "../loaders/globalloader.fsx"
#endif

open Apirefloader
open FSharp.Formatting.ApiDocs

type Entry = {
    uri: string
    title: string
    content: string
}

let stripMicrosoft (str: string) =
    if str.StartsWith("Microsoft.") then
        str.["Microsoft.".Length ..]
    elif str.StartsWith("microsoft-") then
        str.["microsoft-".Length ..]
    else
        str

let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo>().Value
    let rootUrl = siteInfo.root_url
    let all = ctx.TryGetValues<AssemblyEntities>()
    let refs =
      match all with
      | None -> []
      | Some all ->
        all
        |> Seq.toList
        |> List.collect (fun n ->
          let generatorOutput = n.GeneratorOutput
          let allModules = n.Modules
          let allTypes = n.Types

          let gen =
              let ctn =
                  sprintf "%s \n %s" generatorOutput.AssemblyGroup.Name (generatorOutput.AssemblyGroup.Namespaces |> Seq.map (fun n -> n.Name) |> String.concat " ")
              {uri = (rootUrl + sprintf "/reference/%s/index.html" n.Label ); title = sprintf "%s - API Reference" n.Label; content = ctn }

          let mdlsGen =
              allModules
              |> Seq.map (fun m ->
                  let m = m.Info
                  let cnt =
                      sprintf "%s \n %s \n %s \n %s \n %s \n %s"
                          m.Name
                          m.Comment.FullText
                          (m.NestedModules |> List.map (fun m -> m.Name + " " + m.Comment.FullText ) |> String.concat " ")
                          (m.NestedTypes |> List.map (fun m -> m.Name + " " + m.Comment.FullText ) |> String.concat " ")
                          (m.ValuesAndFuncs |> List.map (fun m -> m.Name + " " + m.Comment.FullText ) |> String.concat " ")
                          (m.TypeExtensions |> List.map (fun m -> m.Name + " " + m.Comment.FullText ) |> String.concat " ")


                  {uri = rootUrl + sprintf "/reference/%s/%s.html" n.Label (stripMicrosoft m.UrlBaseName) ; title = m.Name; content = cnt }
              )

          let tsGen =
              allTypes
              |> Seq.map (fun m ->
                  let m = m.Info
                  let cnt =
                      sprintf "%s \n %s \n %s"
                          m.Name
                          m.Comment.FullText
                          (m.AllMembers |> List.map (fun m -> m.Name + " " + m.Comment.FullText ) |> String.concat " ")


                  {uri = rootUrl + sprintf "/reference/%s/%s.html" n.Label m.UrlBaseName ; title = m.Name; content = cnt }
              )
          [gen; yield! mdlsGen; yield! tsGen]
        )

    [|yield! refs|]
    |> Newtonsoft.Json.JsonConvert.SerializeObject


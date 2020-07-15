#r "../_lib/Fornax.Core.dll"
#r "../packages/FSharp.Formatting/lib/netstandard2.0/FSharp.Formatting.ApiDocs.dll"

open System
open System.IO
open FSharp.Formatting.ApiDocs

type ApiPageInfo<'a> = {
    ParentName: string
    ParentUrlName: string
    NamespaceName: string
    NamespaceUrlName: string
    Info: 'a
}

type AssemblyEntities = {
  Label: string
  Modules: ApiPageInfo<Module> list
  Types: ApiPageInfo<Type> list
  GeneratorOutput: ApiDocsModel
}

let rec collectModules pn pu nn nu (m: Module) =
    [
        yield { ParentName = pn; ParentUrlName = pu; NamespaceName = nn; NamespaceUrlName = nu; Info =  m}
        yield! m.NestedModules |> List.collect (collectModules m.Name m.UrlName nn nu )
    ]

let loader (projectRoot: string) (siteContent: SiteContents) =
    try
        let dir = Path.Combine(projectRoot, "packages", "FSharp.Core", "lib", "netstandard2.0")
        let xml = Path.Combine(dir, "FSharp.Core.xml")
        let dll = [ Path.Combine(System.AppContext.BaseDirectory, "FSharp.Core.dll") ]
        let sourceRepo = "https://github.com/dotnet/fsharp"
        let sourceFolder = "src/fsharp/FSharp.Core"
        let output = ApiDocs.GenerateModel(dll, markDownComments = false, publicOnly = true, xmlFile = xml, sourceRepo = sourceRepo, sourceFolder = sourceFolder)

        let allModules =
            output.AssemblyGroup.Namespaces
            |> List.collect (fun n ->
                List.collect (collectModules n.Name n.Name n.Name n.Name) n.Modules
            )
        let allTypes =
            [
                yield!
                    output.AssemblyGroup.Namespaces
                    |> List.collect (fun n ->
                        n.Types |> List.map (fun t -> {ParentName = n.Name; ParentUrlName = n.Name; NamespaceName = n.Name; NamespaceUrlName = n.Name; Info = t} )
                    )
                yield!
                    allModules
                    |> List.collect (fun n ->
                        n.Info.NestedTypes |> List.map (fun t -> {ParentName = n.Info.Name; ParentUrlName = n.Info.UrlName; NamespaceName = n.NamespaceName; NamespaceUrlName = n.NamespaceUrlName; Info = t}) )
            ]

        let entities = {
          Label = "FSharp.Core"
          Modules = allModules
          Types = allTypes
          GeneratorOutput = output
        }
        siteContent.Add entities
    with
    | ex ->
      printfn "%A" ex

    siteContent

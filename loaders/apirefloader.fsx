#r "../_lib/Fornax.Core.dll"
#load "../.paket/load/main.group.fsx"

open System
open System.IO
open FSharp.MetadataFormat
open Yaaf.FSharp.Scripting
open FSharp.Compiler
open FSharp.Compiler.SourceCodeServices
open FSharp.Formatting.Common

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
  GeneratorOutput: GeneratorOutput
}

let rec collectModules pn pu nn nu (m: Module) =
    [
        yield { ParentName = pn; ParentUrlName = pu; NamespaceName = nn; NamespaceUrlName = nu; Info =  m}
        yield! m.NestedModules |> List.collect (collectModules m.Name m.UrlName nn nu )
    ]

let loader (projectRoot: string) (siteContent: SiteContents) =
    Log.SetupSource [|Log.ConsoleListener() |] Log.source
    Log.traceEventf Diagnostics.TraceEventType.Verbose "whoo"
    try
        let dir = Path.Combine(projectRoot, "packages", "FSharp.Core", "lib", "netstandard2.0")
        let xml = Path.Combine(dir, "FSharp.Core.xml")
        let dll = Path.Combine(dir, "FSharp.Core.dll")
        
        let properties = [
            "project-name", "FSharp.Core"
        ]

        let sourceRepo = "https://github.com/dotnet/fsharp"
        let sourceFolder = "src/fsharp/FSharp.Core"
        let netcorerefs = Directory.EnumerateFiles("/usr/local/share/dotnet/packs/Microsoft.NETCore.App.Ref/3.1.0/ref/netcoreapp3.1/", "*.dll")
        let assemblyRefs = netcorerefs |> List.ofSeq |> List.map (sprintf "-r:%s")
        let otherFlags = ["--targetprofile:netstandard"] @ assemblyRefs
        let output = MetadataFormat.Generate(dll, parameters = properties, markDownComments = false, xmlFile = xml, publicOnly = true, sourceRepo = sourceRepo, otherFlags = otherFlags, sourceFolder = sourceFolder)

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
        
        if List.isEmpty allModules || List.isEmpty allTypes then failwithf "didn't get any api reference data: %A" output

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

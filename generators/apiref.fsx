#r "../_lib/Fornax.Core.dll"
#r "../packages/Newtonsoft.Json/lib/netstandard2.0/Newtonsoft.Json.dll"
#r "../packages/FSharp.Formatting/lib/netstandard2.0/FSharp.Formatting.ApiDocs.dll"

#if !FORNAX
#load "../loaders/apirefloader.fsx"
#endif

#load "partials/layout.fsx"

open System
open FSharp.Formatting.ApiDocs
open Html
open Apirefloader

let stripMicrosoft (str: string) =
    if str.StartsWith("Microsoft.") then
        str.["Microsoft.".Length ..]
    elif str.StartsWith("microsoft-") then
        str.["microsoft-".Length ..]
    else
        str

let getComment (c: DocComment) =
    sprintf """<div class="comment">%s</div>""" c.FullText

let formatMember (m: Member) =
    let hasCustomOp =
      m.Attributes
      |> List.exists (fun a -> a.FullName = "Microsoft.FSharp.Core.CustomOperationAttribute")

    let customOp =
      if hasCustomOp then
        m.Attributes
        |> List.tryFind (fun a -> a.FullName = "Microsoft.FSharp.Core.CustomOperationAttribute")
        |> Option.bind (fun a ->
          a.ConstructorArguments
          |> Seq.tryFind (fun x -> x :? string)
          |> Option.map (fun x -> x.ToString())
        )
        |> Option.defaultValue ""
      else
        ""

    tr [] [
        td [] [
            code [] [!! m.Name]
            br []

            if hasCustomOp then
              b [] [!! "CE Custom Operation: "]
              code [] [!!customOp]
              br []
            br []
            b [] [!! "Signature: "]
            !!m.Details.Signature
        ]
        td [] [!! (getComment m.Comment)]
    ]

let generateType ctx (page: ApiPageInfo<Type>) =
    let t = page.Info
    let body =
        div [Class "api-page"] [
            h2 [] [!! t.Name]
            b [] [!! "Namespace: "]
            a [Href (sprintf "%s.html" page.NamespaceUrlName) ] [!! page.NamespaceName]
            br []
            b [] [!! "Parent: "]
            a [Href (sprintf "%s.html" page.ParentUrlName)] [!! page.ParentName]
            span [] [!! (getComment t.Comment)]
            br []
            if not (String.IsNullOrWhiteSpace t.Category) then
                b [] [!! "Category:"]
                !!t.Category
                br []
            if not (t.Attributes.IsEmpty) then
                b [] [!! "Attributes:"]
                for a in t.Attributes do
                    br []
                    code [] [!! (a.Name)]
                br []

            table [] [
                tr [] [
                    th [ Width "35%" ] [!!"Name"]
                    th [ Width "65%"] [!!"Description"]
                ]
                if not t.Constructors.IsEmpty then tr [] [ td [ColSpan 3. ] [ b [] [!! "Constructors"]]]
                yield! t.Constructors |> List.map formatMember

                if not t.InstanceMembers.IsEmpty then tr [] [ td [ColSpan 3. ] [ b [] [!! "Instance Members"]]]
                yield! t.InstanceMembers |> List.map formatMember

                if not t.RecordFields.IsEmpty then tr [] [ td [ColSpan 3. ] [ b [] [!! "Record Fields"]]]
                yield! t.RecordFields |> List.map formatMember

                if not t.StaticMembers.IsEmpty then tr [] [ td [ColSpan 3. ] [ b [] [!! "Static Members"]]]
                yield! t.StaticMembers |> List.map formatMember

                if not t.StaticParameters.IsEmpty then tr [] [ td [ColSpan 3. ] [ b [] [!! "Static Parameters"]]]
                yield! t.StaticParameters |> List.map formatMember

                if not t.UnionCases.IsEmpty then tr [] [ td [ColSpan 3. ] [ b [] [!! "Union Cases"]]]
                yield! t.UnionCases |> List.map formatMember
            ]
        ]
    t.UrlName, Layout.layout ctx [body] t.Name

let generateModule ctx (page: ApiPageInfo<Module>) =
    let m = page.Info
    let body =
        div [Class "api-page"] [
            h2 [] [!!m.Name]
            b [] [!! "Namespace: "]
            a [Href (sprintf "%s.html" page.NamespaceUrlName) ] [!! page.NamespaceName]
            br []
            b [] [!! "Parent: "]
            a [Href (sprintf "%s.html" page.ParentUrlName)] [!! page.ParentName]
            span [] [!! (getComment m.Comment)]
            br []
            if not (String.IsNullOrWhiteSpace m.Category) then
                b [] [!! "Category:"]
                !!m.Category
                br []

            if not m.NestedTypes.IsEmpty then
                b [] [!! "Declared Types"]
                table [] [
                    tr [] [
                        th [ Width "35%" ] [!!"Type"]
                        th [ Width "65%"] [!!"Description"]
                    ]
                    for t in m.NestedTypes do
                        tr [] [
                            td [] [a [Href (sprintf "%s.html" (stripMicrosoft t.UrlName))] [!! t.Name ]]
                            td [] [!! (getComment t.Comment)]
                        ]
                ]
                br []

            if not m.NestedModules.IsEmpty then
                b [] [!! "Declared Modules"]
                table [] [
                    tr [] [
                        th [ Width "35%" ] [!!"Module"]
                        th [ Width "65%"] [!!"Description"]
                    ]
                    for t in m.NestedModules do
                        tr [] [
                            td [] [a [Href (sprintf "%s.html" (stripMicrosoft t.UrlName))] [!! t.Name ]]
                            td [] [!! (getComment t.Comment)]
                        ]
                ]
                br []

            if not m.ValuesAndFuncs.IsEmpty then
                b [] [!! "Values and Functions"]
                table [] [
                    tr [] [
                        th [ Width "35%" ] [!!"Name"]
                        th [ Width "65%"] [!!"Description"]
                    ]
                    yield! m.ValuesAndFuncs |> List.map formatMember
                ]
                br []

            if not m.TypeExtensions.IsEmpty then
                b [] [!! "Type Extensions"]
                table [] [
                    tr [] [
                        th [ Width "35%" ] [!!"Name"]
                        th [ Width "65%"] [!!"Description"]
                    ]
                    yield! m.TypeExtensions |> List.map formatMember
                ]
        ]
    m.UrlName, Layout.layout ctx [body] m.Name

let generateNamespace ctx (n: Namespace)  =
    let body =
        div [Class "api-page"] [
            h2 [] [!! (stripMicrosoft n.Name)]

            if not n.Types.IsEmpty then

                b [] [!! "Declared Types"]
                table [] [
                    tr [] [
                        th [ Width "35%" ] [!!"Type"]
                        th [ Width "65%"] [!!"Description"]
                    ]
                    for t in n.Types do
                        tr [] [
                            td [] [a [Href (sprintf "%s.html" (stripMicrosoft t.UrlName))] [!! t.Name ]]
                            td [] [!!(getComment t.Comment)]
                        ]
                ]
                br []

            if not n.Modules.IsEmpty then

                b [] [!! "Declared Modules"]
                table [] [
                    tr [] [
                        th [ Width "35%" ] [!!"Module"]
                        th [ Width "65%"] [!!"Description"]
                    ]
                    for t in n.Modules do
                        tr [] [
                            td [] [a [Href (sprintf "%s.html" (stripMicrosoft t.UrlName))] [!! t.Name ]]
                            td [] [!! (getComment t.Comment)]
                        ]
                ]
        ]
    n.Name, Layout.layout ctx [body] (n.Name)


let generate' (ctx : SiteContents)  =
    let all = ctx.TryGetValues<AssemblyEntities>()
    match all with
    | None ->
        printfn "no assembly entities found"
        []
    | Some all ->
      all
      |> Seq.toList
      |> List.collect (fun n ->
        let name = stripMicrosoft n.GeneratorOutput.AssemblyGroup.Name
        let namespaces =
          n.GeneratorOutput.AssemblyGroup.Namespaces
          |> List.map (generateNamespace ctx)

        let modules =
          n.Modules
          |> Seq.map (generateModule ctx)

        let types =
          n.Types
          |> Seq.map (generateType ctx)

        let ref =
          Layout.layout ctx [
            h1 [] [!! name ]
            table [] [
               tr [] [
                    th [ Width "35%" ] [!!"Namespace"]
                    th [ Width "65%"] [!!"Description"]
               ]

               let namespaces =
                   [
                       ("FSharp.Core", "Types, type abbreviations, and modules implicitly in scope for all F# code.")
                       ("FSharp.Collections", "Types and modules for F# core library collection types.")
                       ("FSharp.Control", "Types and functionality for asynchronous and event-driven programming in F#.")
                       ("FSharp.Core.CompilerServices", "Types and modules intrinsic to F# compilation.")
                       ("FSharp.Data.UnitSystems.SI.UnitNames", "Units of Measure based on the SI system.")
                       ("FSharp.Data.UnitSystems.SI.UnitSymbols", "Symbols defined as Units of Measure based on the SI system.")
                       ("FSharp.Linq", "Types and modules for working with F# query syntax and nullable value types.")
                       ("FSharp.Linq.QueryRunExtensions", "Modules used to support F# query syntax.")
                       ("FSharp.Linq.RuntimeHelpers", "Support for evaulating F# quotations via refection.")
                       ("FSharp.NativeInterop", "Access to the NativePtr module for low-level native interoperation in F#.")
                       ("FSharp.Quotations", "Types and modules used to represent and analyze F# quotations.")
                       ("FSharp.Reflection", "Types and extensions for using .NET reflection with F# types and values.")
                   ]
               
               for (name, desc) in namespaces do
                  tr [] [
                      td [] [a [Href (sprintf "%s.html" name)] [!! name]]
                      td [] [!! desc]
                  ]
            ]
          ] n.Label

        [("index" , ref); yield! namespaces; yield! modules; yield! types]
        |> List.map (fun (x, y) -> (sprintf "%s/%s" (stripMicrosoft n.Label) (stripMicrosoft x)), y)
      )


let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    try
        generate' ctx
        |> List.map (fun (n,b) -> n, (Layout.render ctx b))
    with
    | ex ->
        printfn "ERROR IN API REF GENERATION:\n%A" ex
        []

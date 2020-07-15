#r "../_lib/Fornax.Core.dll"

type SiteInfo = {
    title: string
    description: string
    theme_variant: string option
    root_url: string
}

let config = {
    title = "F# Core API Docs"
    description = "Reference docs for the FSharp.Core library"
    theme_variant = Some "green"
    root_url =
      #if WATCH
        "http://localhost:8080/"
      #else
        "https://fsprojects.github.io/fsharp-core-api-docs/"
      #endif
}

let loader (projectRoot: string) (siteContent: SiteContents) =
    siteContent.Add(config)
    siteContent

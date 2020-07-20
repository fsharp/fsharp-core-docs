#r "../_lib/Fornax.Core.dll"

type Shortcut = {
    title: string
    link: string
    icon: string
}

let loader (projectRoot: string) (siteContent: SiteContents) =
    siteContent.Add({title = "Home"; link = "/fsharp-core-api-docs"; icon = "fas fa-home"})
    siteContent.Add({title = "F# Software Foundation"; link = "http://fsharp.org"; icon = "fas fa-cubes"})
    siteContent.Add({title = "FSharp.Core source code"; link = "https://github.com/dotnet/fsharp"; icon = "fab fa-github"})
    siteContent.Add({title = "These docs"; link = "https://github.com/fsharp/fsharp-core-api-docs"; icon = "fab fa-github"})
    siteContent

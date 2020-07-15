#r "../../_lib/Fornax.Core.dll"
#if !FORNAX
#load "../../loaders/apirefloader.fsx"
#load "../../loaders/pageloader.fsx"
#load "../../loaders/globalloader.fsx"
#endif

open Html


let menu (ctx : SiteContents) (page: string) =
  let shortcuts = ctx.GetValues<Pageloader.Shortcut> ()
  let all = ctx.GetValues<Apirefloader.AssemblyEntities>()

  let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo>().Value
  let rootUrl = siteInfo.root_url


  let menuHeader =
    [
      li [ Id "menu-refs"; Class "dd-item menu-group-link menu-group-link-active"] [
        a [] [!! "API References"]
      ]
    ]



  let renderRefs =
    ul [Id "submenu-refs"; Class "submenu submenu-active" ] [
      for r in all ->
        li [] [
          a [Href (rootUrl + "/reference/" +  r.Label + "/index.html"); if r.Label = page then Class "active-link padding" else Class "padding" ] [
            !! r.Label
          ]
        ]
    ]

  let renderShortucuts =
    section [Id "shortcuts"] [
        h3 [] [!! "Shortcuts"]
        ul [] [
          for s in shortcuts do
            yield
              li [] [
                a [Class "padding"; Href s.link ] [
                  i [Class s.icon] []
                  !! s.title
                ]
              ]
        ]
      ]

  let renderFooter =
    section [Id "footer"] [
      !! """<p>Built with <a href="https://github.com/ionide/Fornax">Fornax</a>"""
    ]


  nav [Id "sidebar"] [
    div [Id "header-wrapper"] [
      div [Id "header"] [
        h2 [Id "logo"] [!! siteInfo.title]
      ]
      div [Class "searchbox"] [
        label [Custom("for", "search-by")] [i [Class "fas fa-search"] []]
        input [Custom ("data-search-input", ""); Id "search-by"; Type "search"; Placeholder "Search..."]
        span  [Custom ("data-search-clear", "")] [i [Class "fas fa-times"] []]
      ]
      script [Type "text/javascript"; Src (rootUrl + "/static/js/lunr.min.js")] []
      script [Type "text/javascript"; Src (rootUrl + "/static/js/auto-complete.js")] []
      script [Type "text/javascript";] [!! (sprintf "var baseurl ='%s'" rootUrl)]
      script [Type "text/javascript"; Src (rootUrl + "/static/js/search.js")] []
      script [Src (rootUrl + "/static/js/highlight.pack.js")] []
      script [] [!! "hljs.initHighlightingOnLoad();"]
    ]
    div [Class "highlightable"] [
      ul [Class "topics"] menuHeader
      renderRefs
      renderShortucuts
      renderFooter
    ]
  ]


#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html


let generate' (ctx : SiteContents) (page: string) =
    let commonPage =
        ctx.TryGetValues<Commonloader.Page> ()
        |> Option.defaultValue Seq.empty
        |> Seq.find (fun n -> n.file = page)

    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let (desc, title) =
        siteInfo
        |> Option.map (fun si -> (si.description, si.title))
        |> Option.defaultValue ("", "")

    Layout.layout ctx (defaultArg commonPage.title "") [
        div [Class "container"] [
            section [Class "articles"] [
                div [Class "column is-8 is-offset-2"] [
                    Layout.pageLayout commonPage
                ]
            ]
        ]
    ]

let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    generate' ctx page
    |> Layout.render ctx

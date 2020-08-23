#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html


let generate' (ctx : SiteContents) (page: string) =
    let post =
        ctx.TryGetValues<Postloader.Post> ()
        |> Option.defaultValue Seq.empty
        |> Seq.find (fun n -> n.file = page)

    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let (desc, title) =
        siteInfo
        |> Option.map (fun si -> (si.description, si.title))
        |> Option.defaultValue ("", "")

    Layout.layout ctx (defaultArg post.title "") [
        section [Class "hero is-info is-medium is-bold"] [
            div [Class "hero-body"] [
                div [Class "container has-text-centered"] [
                    h1 [Class "title"] [!!title]
                    span [] [!!desc]
                ]
            ]
        ]
        div [Class "container"] [
            section [Class "articles"] [
                div [Class "column is-8 is-offset-2"] [
                    Layout.postLayout false post
                ]
            ]
        ]
    ]

let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    generate' ctx page
    |> Layout.render ctx

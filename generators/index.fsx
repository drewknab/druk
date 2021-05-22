#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html

let generate' (ctx : SiteContents) (_: string) =
    let posts = ctx.TryGetValues<Postloader.Post> () |> Option.defaultValue Seq.empty
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let (desc, title) =
        siteInfo
        |> Option.map (fun si -> (si.description, si.title))
        |> Option.defaultValue ("", "")

    let psts =
        posts
        |> Seq.sortByDescending Layout.published
        |> Seq.toList
        |> List.map (Layout.postLayout true)

    Layout.layout ctx "Home" [
        div [Class "container"] [
            section [Class "articles"] [
                div [Class "column is-8 is-offset-2"] psts
            ]
        ]
    ]

let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    generate' ctx page
    |> Layout.render ctx

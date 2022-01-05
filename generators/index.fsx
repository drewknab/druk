#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html

let about = "I'm Drew Knab, senior full-stack developer at
             <a href='https://directlink.ai/'>directlink.</a>
             I play trading card games competitively sometimes and
             make music even less. I live in Syracuse, NY. It's fine.
             Sometimes I stream <a href='https://www.twitch.tv/rogerjmexico'>on twitch</a>."

let generate' (ctx : SiteContents) (_: string) =
    let posts = ctx.TryGetValues<Postloader.Post> () |> Option.defaultValue Seq.empty
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let (desc, title) =
        siteInfo
        |> Option.map (fun si -> (si.description, si.title))
        |> Option.defaultValue ("", "")

    let psts =
        posts
        |> Seq.sortByDescending (fun n -> n.published)
        |> Seq.truncate 5
        |> Seq.toList
        |> List.map (Layout.postLayout true)

    Layout.layout ctx "Home" [
        section [Class "masthead container"][
            div [Class "columns is-vcentered"] [
                div [Class "column is-two-fifths is-flex selfie-box "] [
                    img [Class "selfie"; Src "/images/me.jpg"; Alt "Drew Knab, Goober"]
                ]
                div [Class "column is-three-fifths"] [
                    p [Class "is-size-3 has-text-justified"][
                        !! (about)
                    ]
                ]
            ]
            hr [Class "masthead-divider"]
        ]
        section [Class "articles container"] [
            h3 [Class "latest-border is-size-3 has-text-right"] [
                !! "Latest"
            ]
            div [Class ""] psts
        ]
    ]

let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    generate' ctx page
    |> Layout.render ctx

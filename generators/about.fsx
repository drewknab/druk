#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html

let about = "I'm Drew Knab, full-stack developer at SIDEARM Sports.
             I play trading card games competatively sometimes and
             make music even less. I live in Syracuse, NY. It's fine.
             Sometimes I stream <a href='https://www.twitch.tv/rogerjmexico'>on twitch</a>.
             <p>
             Hero image credit: <a href='https://unsplash.com/photos/x48QL8gNYZ8'>Artur Rutkowski</a>
 "

let generate' (ctx : SiteContents) (_: string) =
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let desc =
        siteInfo
        |> Option.map (fun si -> si.description)
        |> Option.defaultValue ""


    Layout.layout ctx "Home" [
        section [Class "hero is-info is-medium is-bold"] [
            div [Class "hero-body"] [
                div [Class "container has-text-centered"] [
                    h1 [Class "title"] [!!desc]
                ]
            ]
        ]
        div [Class "container"] [
            section [Class "articles"] [
                div [Class "column is-8 is-offset-2"] [
                    div [Class "card article"] [
                        div [Class "card-content"] [
                            div [Class "content article-body"] [
                                !! about
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

let generate 
    (ctx : SiteContents)
    (projectRoot: string)
    (page: string) =
    generate' ctx page
    |> Layout.render ctx

#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html

let about = "I'm Drew Knab, full-stack developer at SIDEARM Sports.
             I play trading card games competatively sometimes and
             make music even less. I live in Syracuse, NY. It's fine.
             Sometimes I stream <a href='https://www.twitch.tv/rogerjmexico'>on twitch</a>.
 "

let test =
    System.Console.WriteLine "yay"

let generate' (ctx : SiteContents) (_: string) =
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    test
    let desc =
        siteInfo
        |> Option.map (fun si -> si.description)
        |> Option.defaultValue ""


    Layout.layout ctx "Home" [
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

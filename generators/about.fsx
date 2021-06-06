#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html

let about = "Lately I find myself working mostly in .NET technologies
             but I've delivered quality software in both Node
             and PHP with a spicy plethora of SQL and NoSQL databases.
             I've mostly worked with Vue and Angular with a tiny bit of
             React mixed in. There have even been some desktop applications
             mixed in on occasion."

let history = "I'm largely self-taught having started Web development in 2003.
               Got my for-real first professional job in 2013.
               I went to school for Journalism at SUNY Brockport because I
               wanted to be a writer for a minute there. The only downsides are
               I've never been formally graded on writing a compiler from
               scratch and my understanding of pointers in C is a little weak."

let generate' (ctx : SiteContents) (_: string) =
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let desc =
        siteInfo
        |> Option.map (fun si -> si.description)
        |> Option.defaultValue ""


    Layout.layout ctx "About" [
        div [Class "container"] [
            section [Class "articles"] [
                article [Class "story-wrapper column is-8 is-offset-2"] [
                    h2 [Class "is-size-3"] [
                        !! "About"
                    ]
                    p [] [
                        !! "Hello, I'm Drew. I'm a full-stack developer in a variety of stacks."
                    ]
                    h2 [Class "is-size-3"] [
                        !! "What I do"
                    ]
                    p [] [
                        !! about
                    ]
                    h2 [Class "is-size-3"] [
                        !! "History"
                    ]
                    p [] [
                        !! history
                    ]
                    h2 [Class "is-size-3"] [
                        !! "Other Details"
                    ]
                    ul [] [
                        li [] [
                            a [Href "uses.html"] [
                                !! "Uses"
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

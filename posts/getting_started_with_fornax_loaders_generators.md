---
layout: post
title: Learning About Loaders and Generators in Fornax
author: Drew
published: 2020-07-06
---
### What happened last time?

Last time we had a brief introduction to [Fornax](https://github.com/ionide/Fornax) (a static site generator written in F#). We went over a brief introduction of getting up and running as well as an overview of the files in a Fornax project. This time we're digging into a nuts and bolts by covering **loaders** and **generators**.

<!--more-->

#### Loaders
In order to talk about loaders we have to take a brief detour to talk about **SiteContents**. In Fornax, **SiteContents** is a type defined in the Fornax.Core.dll that can hold any arbitrary number of arbitrary lists of arbitrary objects. Normally, this is a collection of markdown files that are parsed into a collection of **Post** objects, but that's just the beginning.

Now that we have that out of the way, lets have a look at a loader from the project we made last time. Go ahead and take a peek at postloader.fsx. There's quite a lot to take in all at once, so let's break it down.

    type Post = {
        file: string
        link: string
        title: string option
        author: string option
        published: System.DateTime option
        tags: string list
        content: string
        summary: string
    }

We declare the post type. So that further down in our program we can add a collection of that type to SiteContents. F# has a fairly robust type system and there's a number of improvements we could make to ensure data integrity, but we'll leave it as it is. We'll jump down to the bottom of the file and take a look at the **loader** function.

    let loader (projectRoot: string) (siteContent: SiteContents) =
            let postsPath = Path.Combine(projectRoot, contentDir)
            Directory.GetFiles postsPath
            |> Array.filter (fun n -> n.EndsWith ".md")
            |> Array.map loadFile
            |> Array.iter siteContent.Add

        siteContent

We can see here that we harness .NET Framework to use Directory.GetFiles from the postsPath that we declare above it. If you're new to F#, the syntax may be a little confusing, but conceptually it's very similar to chaining array methods in JavaScript.

With the collection of files we return from Directory.GetFiles, we Array.filter the collection and return a new collection of only .md files. Then we iterate over that collection with Array.Map and loadFile function which we'll look at in more detail later. After maping the collection we finally Array.iter and add each object in the collection to siteContent and return siteContent after our additions.

    let loadFile n =
        let text = File.ReadAllText n

        let config = getConfig text
        let summary, content = getContent text
        let fileName = buildFileName n
        let file = Path.Combine(contentDir, fileName ".md").Replace("\\", "/")
        let link = "/" + Path.Combine(contentDir, fileName ".html").Replace("\\", "/")

        let filter = filterConfig config
        let title = filter "title" |> Option.map trimString
        let author = filter "author" |> Option.map trimString
        let published =
            filter "published" 
            |> Option.map (trimString >> System.DateTime.Parse)

        let tags =
            let tagsOpt =
                filter "tags"
                |> Option.map (trimString >> fun n -> n.Split ',' |> Array.toList)
            defaultArg tagsOpt []

        { file = file
        link = link
        title = title
        author = author
        published = published
        tags = tags
        content = content
        summary = summary }

The function loadFile does a lot of things I'll leave it to you to figure out the process of reading and parsing a given markdown file as passed in from the loader function. The thing I'd like to note, however, is that each time we iterate through this function we return an object that matches the shape of the Post type that exists at the top of the file.

Now that we've loaded our posts into SiteContents we can take a look at what generators do with that content.

#### Generators
Once we've churned through that loader and have our contents in memory we can generate some front end content. Let's have a look inside the post.fsx inside the generators folder.

This file is a lot shorter than postloader. Let's start at the bottom.

    let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
        generate' ctx page
        |> Layout.render ctx

Immediately we call **generate'** and pass in SiteContents and page then we pipe the value returned from that function to Layout.render. Layout.render is outside the scope of this article, but it lives in layout.fsx.

For now though, let's look at generate'. we'll look at it block by block.

    let generate' (ctx : SiteContents) (page: string) =
        let post =
            ctx.TryGetValues<Postloader.Post> ()
            |> Option.defaultValue Seq.empty
            |> Seq.find (fun n -> n.file = page)

First we try to find the post. We start by using TryGetValues method on SiteContents to pull back all posts that match the shape of the object Post that exists in the loader we looked at previously. Then we iterate through the sequence until we find one that matches the page we're currently iterating through.

        let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
        let (desc, title) =
            siteInfo
            |> Option.map (fun si -> (si.description, si.title))
            |> Option.defaultValue ("", "")

We do the same thing to grab SiteInfo (held in globalloader.fsx) and map any description and title we have stored.

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
This part is pretty different from what we've looked at previously. This is a feature of Fornax that harnesses an interesting feature of F# called **Domain Specific Language**. Instead of writing HTML in Fornax we use this DSL to simplify the templating process. If you've done work with HTML markup. I'll admit it feels a little awkward to use DSL to write a template, but once I got the hang of it I've found it pretty compelling.

From there generate' returns the DSL HtmlElement we constructed where it can be parsed and turned into a string in Layout.render when we finally output the file into the _public folder.

That's the basic overview of loaders and generators. We only scratched the surface of what we can do with loaders. Because we're working with F# scripts we can get our data from any source we might want.

RESTful API? For sure.

SQL DB? No problem.

CSV? Absolutely.

Similarly, we can output data in a generator in any number of formats. RSS, CSV, JSON, plain text, and so on.

That's enough for today I think. Next time, we'll take a look at creating our own loader using an external API.
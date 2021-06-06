---
layout: post
title: Writing Custom Loaders and Generators with Fornax
author: Drew
tags: fsharp, fornax
published: 2020-08-22
---

Last time we had an introduction to [Fornax](https://github.com/ionide/Fornax) loaders and generators. Today we're going to build a custom loader and generator using an external REST API.

<!--more-->

#### Before We Begin
To facilitate working with data from APIs or sources outside of markdown we're going to bring in a library. Because Fornax is, at heart, a small collection of F# scripts consumed by a library, we won't be dealing with a package manager. I went to [FSharp.Data](http://fsharp.github.io/FSharp.Data) and downloaded the precompiled binaries and dropped FSharp.Data.dll and FSharp.Data.DesignTime.dll into the _lib folder.

FSharp.Data is a collection of parsers and type providers that simplify common data routines and allows a more F# centric, functional first, way than if we were to use built in .NET Framework or C# libraries.

The API we're going to use is [Cat Facts](https://cat-fact.herokuapp.com/#/) because it's free, open, and is a collection of facts about cats.

#### Custom Cat Facts Loader
We'll make a new file in the loaders folder called catfacts.fsx.

At the top of the file we'll bring in our assembly references:
```fsharp 
    #r "../_lib/Fornax.Core.dll"
    #r "../_lib/FSharp.Data.dll"
    open FSharp.Data
``` 

That's it, now we can use FSharp.Data to handle our HTTP request and JSON parsing.

We'll declare a JsonProvider with the basic shape of the response from the Cat Facts API and the CatFacts type that we'll be using internally.

```fsharp
type CatFactsTemplate = JsonProvider<"""
{
    "all":
    [
        {
            "_id": "string",
            "text": "string"
        }
    ]
}""">

type CatFacts =
    { Id : string
      Text : string }
```

We could make both of these type definitions more elaborate. We get a lot more information out of the Cat Facts API, but this is enough for now.

To explain, CatFactsTemplate uses JsonProvider out of FSharp.Data. When we go to parse the JSON from the HTTP request and translates it into an F# object we can use.

Now that our data types are out of the way, let's dig into the loader function.

```fsharp
let loader (projectRoot: string) (siteContent: SiteContents) =
    let httpRequest = Http.RequestString("https://cat-fact.herokuapp.com/facts")
    let jsonResponse = CatFactsTemplate.Parse(httpRequest).All

    let facts = seq {
        for record in jsonResponse do
            { Id = record.Id ; Text = record.Text }
    }

    facts
    |> Seq.take 10
    |> Seq.iter (fun f -> siteContent.Add f)

    siteContent
```

Taken in steps we can see that we make an HTTP request to the API and pull back a list of facts. FSharp.Data handwaves away a lot of the details that we might have had to do if we used WebClient out of .NET Framework.

After that, we call CatFactsTemplate.Parse to take the JSON body and convert it into an F# object. We tack All on the end so we have only the Array of objects with Id and Text.

Under the hood, the Array we get out of the parse doesn't actually implement **IEnumberable<'T>** so we can't iterate over it by piping it into Seq.xyz. Instead we'll create an **IEnumerable<'T>** ourselves with:
```fsharp
let facts = seq {
    for record in jsonResponse do
        { Id = record.Id ; Text = record.Text }
}
```
This creates a sequence, which does implement IEnumberable<'T>, by looping through jsonResponse and returning our CatFacts type with the data defined in CatFactsTemplate.

```fsharp
facts
|> Seq.take 10
|> Seq.iter (fun f -> siteContent.Add f)

siteContent
```

Now that we have our sequence we can pipe that into Seq.take to cut down on the objects we're going to add into siteContent. Then we just Seq.iter to loop through our ten objects and add them to site content so we can generate HTML from them.

That's it, we now have a custom loader that consumes an external API.

#### Custom Cat Facts Generator
Now that we have all of our Cat Facts data loaded into SiteContents, let's bring it out into a generator. We'll make a new file called catfacts.fsx in the generators folder.

Then we'll whip up something that looks like this:
```fsharp
#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html

let catfactCard (fact : Catfactsloader.CatFacts) =
    div [Class "card article"] [
        div [Class "card-content"] [
            div [Class "media-content has-text-centered"] [
                !! fact.Text
            ]
        ]
    ]

let generate' (ctx : SiteContents) =
    let facts =
        ctx.TryGetValues<Catfactsloader.CatFacts>()
        |> Option.defaultValue Seq.empty
        |> Seq.toList
        |> List.map (catfactCard)

    Layout.layout ctx "Home" [
        div [Class "container"] [
            section [Class "articles"] [
                div [Class "column is-8 is-offset-2"] facts
            ]
        ]
    ]

let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    generate' ctx
    |> Layout.render ctx
```

We pull in the files we need at the top and open the module we'll be using, in this case HTML, to handle the Domain Specific Language (DSL) that we talked about in the last article.

```fsharp
let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    generate' ctx
    |> Layout.render ctx
```

Like we also talked about last time, generate gets called first by Fornax at build time. It calls another function called generate' and pipes the resulting HtmlElement content into the Layout.render function. Render then parses the template into a string for the resulting HTML document.

```fsharp
let generate' (ctx : SiteContents) =
    let facts =
        ctx.TryGetValues<Catfactsloader.CatFacts>()
        |> Option.defaultValue Seq.empty
        |> Seq.toList
        |> List.map (catfactCard)

    Layout.layout ctx "Home" [
        div [Class "container"] [
            section [Class "articles"] [
                div [Class "column is-8 is-offset-2"] facts
            ]
        ]
    ]
```

First, we try get to our CatFacts out of SiteContent with ctx.TryGetValues<Catfactsloader.CatFacts>(). Then we set a default value if we wind up not having records in SiteContents. Next, we turn the Sequence into a List. Finally, we map over the List<CatFacts> with the catfactCard function to return a List<HtmlElement>.

```fsharp
let catfactCard (fact : Catfactsloader.CatFacts) =
    div [Class "card article"] [
        div [Class "card-content"] [
            div [Class "media-content has-text-centered"] [
                !! fact.Text
            ]
        ]
    ]
```

Here we see catfactCard is just a function that takes a CatFact and returns an HtmlElement with CatFact.Text in a div. With that, we should have a working generator ready to build some content.

#### Final Steps
The last thing we have to do is register the generator in config.fsx in the root of our project.
```fsharp
let config = {
    Generators = [
        {Script = "less.fsx"; Trigger = OnFileExt ".less"; OutputFile = ChangeExtension "css" }
        {Script = "sass.fsx"; Trigger = OnFileExt ".scss"; OutputFile = ChangeExtension "css" }
        {Script = "post.fsx"; Trigger = OnFilePredicate postPredicate; OutputFile = ChangeExtension "html" }
        {Script = "staticfile.fsx"; Trigger = OnFilePredicate staticPredicate; OutputFile = SameFileName }
        {Script = "index.fsx"; Trigger = Once; OutputFile = NewFileName "index.html" }
        {Script = "about.fsx"; Trigger = Once; OutputFile = NewFileName "about.html" }
        {Script = "contact.fsx"; Trigger = Once; OutputFile = NewFileName "contact.html" }
    ]
}
```

This array of Generators determines how Fornax treats the generators we make. In this case we want to generate the file once by adding the following entry to the collection.

```fsharp
{Script = "catfacts.fsx"; Trigger = Once; OutputFile = NewFileName "catfacts.html" }
```

Script is the generator script we're using and Trigger defines how to handle the generation of the HTML file. We are choosing the built in Fornax value of Once but we could define our own logic for how to handle file generation. Then we have the OutputFile and we're going to call it catfacts.html.

With that, we can `fornax watch` and visit localhost:8080/catfacts.html to see our result.

Now that we've created a very simple loader and generator to consume an API we could expand that out to any arbitrary data source. We can see that Fornax gives us a very simple method of pulling in and transforming that data. Moving forward we could, for instance, bring in Tweets, dev.to interactions, CSV data, or arbitrary database connections. Any data we can manipulate in F# can be turned into static content.

Next time I think we'll look into some of the features of Fornax and what it's lacking from other static site generators.

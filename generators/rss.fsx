#r "../_lib/Fornax.Core.dll"
#if !FORNAX
#load "../loaders/postloader.fsx"
#load "../loaders/globalloader.fsx"
#endif

open System.Xml.Linq
open System.Net

let xn name = XName.Get name
let elem name (value : string) = XElement(xn name, value)
let xc (value : string) = XCData value
let buildElements (items : Postloader.Post list) =
    items
    |> List.sortBy (fun i -> i.published)
    |> List.map (fun i ->
        let content = xc i.content
        let description = elem "description" ("")
        description.Add(content)
        let xe =
            XElement(xn "item",
                elem "title" (WebUtility.HtmlEncode (defaultArg i.title "")),
                elem "link" i.link,
                elem "guid" i.link,
                elem "pubDate" (i.published.ToString())
            )

        xe.Add(description)

        xe
    )

let channelFeed
    (channelTitle : string)
    (channelLink : string)
    (channelDescription : string)
    (items : Postloader.Post list) =

    let elems = buildElements items

    XDocument(
        XDeclaration("1.0", "utf-8", "yes"),
        XElement(xn "rss",
            XAttribute(xn "version", "2.0"),
            elem "title" channelTitle,
            elem "link" channelLink,
            elem "description" channelDescription,
            elem "language" "en-us",
            XElement(xn "channel", elems)
        ) |> box
    ).ToString()



let generate' (ctx : SiteContents) (_ : string) =
    let (title, link, description) = 
        ctx.TryGetValue<Globalloader.SiteInfo> ()
        |> Option.map (fun si -> (si.title, si.url, si.description))
        |> Option.defaultValue ("", "", "")

    let posts =
        ctx.TryGetValues<Postloader.Post>()
        |> Option.defaultValue Seq.empty
        |> Seq.toList

    [
      "<?xml version='1.0' encoding='UTF-8' ?>"
      channelFeed title link description posts
    ] |> String.concat "\n"

let generate (ctx : SiteContents) (projectRoot : string) (page: string) =
    generate' ctx page

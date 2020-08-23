#r "../_lib/Fornax.Core.dll"
#r "../_lib/Markdig.dll"

open Markdig
open System.IO

type PostConfig = {
    disableLiveRefresh: bool
}

type Post = {
    file: string
    link : string
    title: string option
    author: string option
    published: System.DateTime option
    tags: string list
    content: string
    summary: string
}

let contentDir = "posts"

let markdownPipeline =
    MarkdownPipelineBuilder()
        .UsePipeTables()
        .UseGridTables()
        .Build()

let isSeparator (input : string) =
    input.StartsWith "---"

let isSummarySeparator (input: string) =
    input.Contains "<!--more-->"


///`fileContent` - content of page to parse. Usually whole content of `.md` file
///returns content of config that should be used for the page
let getConfig (fileContent : string) =
    let fileContent = fileContent.Split '\n'
    let fileContent = fileContent |> Array.skip 1 //First line must be ---
    let indexOfSeperator = fileContent |> Array.findIndex isSeparator
    let splitKey (line: string) = 
        let seperatorIndex = line.IndexOf(':')
        if seperatorIndex > 0 then
            let key = line.[.. seperatorIndex - 1].Trim().ToLower()
            let value = line.[seperatorIndex + 1 ..].Trim() 
            Some(key, value)
        else 
            None
    fileContent
    |> Array.splitAt indexOfSeperator
    |> fst
    |> Seq.choose splitKey
    |> Map.ofSeq

///`fileContent` - content of page to parse. Usually whole content of `.md` file
///returns HTML version of content of the page
let getContent (fileContent : string) =
    let fileContent = fileContent.Split '\n'
    let fileContent = fileContent |> Array.skip 1 //First line must be ---
    let indexOfSeperator = fileContent |> Array.findIndex isSeparator
    let _, content = fileContent |> Array.splitAt indexOfSeperator

    let summary, content =
        match content |> Array.tryFindIndex isSummarySeparator with
        | Some indexOfSummary ->
            let summary, _ = content |> Array.splitAt indexOfSummary
            summary, content
        | None ->
            content, content

    let summary = summary |> Array.skip 1 |> String.concat "\n"
    let content = content |> Array.skip 1 |> String.concat "\n"

    Markdown.ToHtml(summary, markdownPipeline),
    Markdown.ToHtml(content, markdownPipeline)

let trimString (str : string) =
    str.Trim().TrimEnd('"').TrimStart('"')

let buildFileName name suffix =
    (name.ToString() |> Path.GetFileNameWithoutExtension) + suffix 

let filterConfig config criteria = 
    config |> Map.tryFind criteria

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

let loader (projectRoot: string) (siteContent: SiteContents) =
    let postsPath = Path.Combine(projectRoot, contentDir)
    Directory.GetFiles postsPath
    |> Array.filter (fun n -> n.EndsWith ".md")
    |> Array.map loadFile
    |> Array.iter siteContent.Add

    siteContent.Add({disableLiveRefresh = true})
    siteContent

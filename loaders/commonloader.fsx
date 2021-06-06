#r "../_lib/Fornax.Core.dll"
#r "../_lib/Markdig.dll"

open Markdig
open System.IO

type PostConfig = {
    disableLiveRefresh: bool
}

type Page = {
    file: string
    link : string
    title: string option
    author: string option
    published: System.DateTime option
    updated: System.DateTime option
    content: string
}

let markdownPipeline =
    MarkdownPipelineBuilder()
        .UsePipeTables()
        .UseGridTables()
        .Build()

let contentDir = "pages"

let isSeparator (input : string) =
    input.StartsWith "---"

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

let getContent (fileContent : string) =
    let fileContent = fileContent.Split '\n'
    let fileContent = fileContent |> Array.skip 1
    let indexOfSeperator = fileContent |> Array.findIndex isSeparator
    let (_, fileContent) = fileContent |> Array.splitAt indexOfSeperator

    let content =
        fileContent
        |> Array.skip 1 
        |> String.concat "\n"

    Markdown.ToHtml(content, markdownPipeline)

let filterConfig config criteria = 
    config |> Map.tryFind criteria

let buildFileName name suffix =
    (name.ToString() |> Path.GetFileNameWithoutExtension) + suffix 

let trimString (str : string) =
    str.Trim().TrimEnd('"').TrimStart('"')

let loadFile n =
    let text = File.ReadAllText n

    let config = getConfig text
    let content = getContent text
    let fileName = buildFileName n
    let file = Path.Combine(contentDir, fileName ".md").Replace("\\", "/")
    let link = "/" + Path.Combine(fileName ".html").Replace("\\", "/")

    let filter = filterConfig config
    let title = filter "title" |> Option.map trimString
    let author = filter "author" |> Option.map trimString
    let published =
        filter "published" 
        |> Option.map (trimString >> System.DateTime.Parse)
    let updated =
        filter "updated" 
        |> Option.map (trimString >> System.DateTime.Parse)

    { file = file
      link = link
      title = title
      author = author
      published = published
      updated = updated
      content = content }

let loader (projectRoot: string) (siteContent: SiteContents) =
    let pagePath = Path.Combine(projectRoot, contentDir)
    Directory.GetFiles pagePath
    |> Array.filter (fun n -> n.EndsWith ".md")
    |> Array.map loadFile
    |> Array.iter siteContent.Add

    siteContent.Add({disableLiveRefresh = true})
    siteContent

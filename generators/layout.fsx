#r "../_lib/Fornax.Core.dll"
#if !FORNAX
#load "../loaders/postloader.fsx"
#load "../loaders/pageloader.fsx"
#load "../loaders/globalloader.fsx"
#endif

open Html

let injectWebsocketCode (webpage:string) =
    let websocketScript =
        """
        <script type="text/javascript">
          var wsUri = "ws://localhost:8080/websocket";
      function init()
      {
        websocket = new WebSocket(wsUri);
        websocket.onclose = function(evt) { onClose(evt) };
      }
      function onClose(evt)
      {
        console.log('closing');
        websocket.close();
        document.location.reload();
      }
      window.addEventListener("load", init, false);
      </script>
        """
    let head = "<head>"
    let index = webpage.IndexOf head
    webpage.Insert ( (index + head.Length + 1),websocketScript)

let layout (ctx : SiteContents) active bodyCnt =
    let pages = ctx.TryGetValues<Pageloader.Page> () |> Option.defaultValue Seq.empty
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let ttl =
          siteInfo
          |> Option.map (fun si -> si.title)
          |> Option.defaultValue ""

    let menuEntries =
        pages
        |> Seq.map (fun p ->
            let cls = "navbar-item"
                //if p.title = active then "navbar-item is-active"
                //else "navbar-item"
            a [Class cls; Href p.link] [!! p.title ]
        )
        |> Seq.toList

    html [] [
        head [] [
            meta [CharSet "utf-8"]
            meta [Name "viewport"; Content "width=device-width, initial-scale=1"]
            title [] [!! ttl]
            link [Rel "icon"; Type "image/png"; Sizes "32x32"; Href "/images/favicon.png"]
            link [Rel "stylesheet"; Href "https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css"]
            link [Rel "stylesheet"; Href "https://fonts.googleapis.com/css?family=Open+Sans"]
            link [Rel "stylesheet"; Type "text/css"; Href "/style/bulmaswatch.min.css"]
            link [Rel "stylesheet"; Type "text/css"; Href "/style/style.css"]
        ]
        body [] [
            nav [Class "navbar is-dark"] [
                div [Class "navbar-brand"] [
                    a [Class "navbar-item"; Href "/"] [
                        img [Src "/images/logo.png"; Alt "Logo"]
                    ]
                    span [Class "navbar-burger burger"; Custom ("data-target", "navbarMenu")] [
                        span [] []
                        span [] []
                        span [] []
                    ]
                ]
                div [Id "navbarMenu"; Class "navbar-menu"] menuEntries
            ]
            yield! bodyCnt
            script [Type "text/javascript"; Src "/js/main.js"] []
        ]
    ]

let render (ctx : SiteContents) cnt =
    let disableLiveRefresh =
        ctx.TryGetValue<Postloader.PostConfig> ()
        |> Option.map (fun n -> n.disableLiveRefresh)
        |> Option.defaultValue false

    cnt
    |> HtmlElement.ToString
    |> fun n -> if disableLiveRefresh then n else injectWebsocketCode n

let published (post: Postloader.Post) =
    post.published
    |> Option.defaultValue System.DateTime.Now
    |> fun n -> n.ToString("yyyy-MMM-dd").Split("-")
    |> fun m -> (m.[0], m.[1], m.[2])

let postLayout (useSummary: bool) (post: Postloader.Post) =
    let (year, month, day) = published post
    if useSummary then
        div [Class "summary-block"] [
            div [Class "columns is-vcentered"] [
                div [Class "column is-one-fifth"] [
                    div [Class "date has-text-centered"] [
                        div [Class "month is-size-5"] [!! month]
                        div [Class "day is-size-4"] [!! day]
                        div [Class "year is-size-5"] [!!year]
                    ]
                ]
                div [Class "column is-four-fifths"] [
                    p [Class "is-size-4 is-spaced "; ] [
                        a [Href post.link] [!! (defaultArg post.title "")]
                    ]
                    div [Class "content article-body"] [
                        !! post.summary
                    ]
                ]
            ]
        ]
    else
        div [Class "story-wrapper"] [
            p [Class "title is-spaced "; ] [
                !! (defaultArg post.title "")
            ]
            p [Class "is-6 article-subtitle "] [
                !! (sprintf "Published on %s %s, %s" month day year)
            ]
            div [Class "content article-body"] [
                !! (if useSummary then post.summary else post.content)
            ]

            div [Class "tombstone is-size-4 has-text-right"] [
                !! (sprintf "&#8718;")
            ]
        ]

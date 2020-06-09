#r "../_lib/Fornax.Core.dll"

type SiteInfo = {
    title: string
    description: string
    url: string
}

let loader (projectRoot: string) (siteContent: SiteContents) =
    siteContent.Add({title = "druk"; description = "Blog of Drew Knab"; url = "druk.dev"})

    siteContent

#r "../_lib/Fornax.Core.dll"

type SiteInfo = {
    title: string
    description: string
    url: string
}

let loader (projectRoot: string) (siteContent: SiteContents) =
    siteContent.Add({title = "Sample Fornax blog"; description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit"; url = "druk.dev"})

    siteContent

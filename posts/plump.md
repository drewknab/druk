---
layout: post
title: Getting Started with Fornax
author: Drew
published: 2020-06-03
---
# What is Fornax?

[Fornax](https://github.com/ionide/Fornax) is a static site generator written in F#. I think it's pretty neat. Fornax generates static sites through a pipeline of F# scripts called **loaders** that inject content into a global context that is then consumed by more F# scripts called **generators** that mold the content its final form. The workflow is decidedly hands-on with a distinct code first approach. This gives Fornax incredible flexibility.

<!--more-->

## Getting Started
Fornax is installed as a global .Net Core tool on the command line with:

`dotnet tool install fornax -g`

This will install Fornax. We can then create a new folder for our new Fornax site, then:

`fornax new`

This will create a new Fornax site using the default blog template.

Now that we have a default Fornax site set up we can run `fornax watch` which will spin up a little server that watches for changes to our project.

Let's take a look at the folder structure now that we have a working Fornax site.

- _lib
    - Any external libraries we might like to use go in here and can be referenced from the script files.
    - By default this contains:
        - Fornax.Core.dll
        - Markdig.dll
- _public
    - This is where the final production built files are created after a build.
- loaders
    - Contains all scripts that load data into the global context.
- generators
    - Contains all scripts that generate files in _public based on the global context created by **loaders**.
- images
    - Images used by our app
- js
    - JS used by our app
- posts
    - This is a domain specific folder used to hold blog posts written in markdown.
- style
    - cSS used by the app
- config.fsx
    - This script runs first, we use it to configure the behavior of our generators and add any additional data like secret keys or SQL connection strings.
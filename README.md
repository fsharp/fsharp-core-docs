# FSharp.Core documentation

https://fsprojects.github.io/fsharp-core-api-docs/reference/FSharp.Core/index.html

## Build steps:

* dotnet tool restore
* dotnet paket restore

from here, if you're trying to do a one-off build, run:
* dotnet fornax build
otherwise, for a more interactive hot-reload experience run:
* dotnet fornax watch

## CI Pipeline

This repo is published via GitHub Actions. On each push to master, `dotnet fornax build` is run, and the outputs (which are written to the `_public` directory) are pushed to the `gh-pages` branch. This repo is configured to host using GitHub Pages from this branch, so once the generated files are pushed the update is nearly-instant.

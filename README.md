# FSharp.Core documentation

https://fsharp.github.io/fsharp-core-api-docs/

## Build steps

Have a build of https://github.com/dotnet/fsharp in a parallel directory, e.g.

    git clone https://github.com/dotnet/fsharp --depth 1
    cd fsharp
    .\build

Then

    dotnet tool restore
    dotnet paket restore
    dotnet fsdocs build --sourcefolder ../fsharp

For iterative development use:

    dotnet fsdocs watch --sourcefolder ../fsharp --open reference/index.html

Change the `TargetPath` in our local `FSharp.Core.fsproj` if needed to picl up a different build of FSHarp.Core.

    <TargetPath>$(MSBuildThisFileDirectory)..\..\fsharp\artifacts\bin\FSharp.Core\Debug\netstandard2.0\FSharp.Core.dll</TargetPath>


## CI Pipeline

This repo is published via GitHub Actions. On each push to master, `dotnet fornax build` is run, and the outputs (which are written to the `_public` directory by fornax) are pushed to the `gh-pages` branch. This repo is configured to host using GitHub Pages from this branch, so once the generated files are pushed the update is nearly-instant.

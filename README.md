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

This repo is published via GitHub Actions. On each push to master, the docs are built, and the outputs (which are written to the `output` directory by fsdocs) are pushed to the `gh-pages` branch. This repo is configured to host using GitHub Pages from this branch, so once the generated files are pushed the update is nearly-instant.

To build the very latest and freshest docs using the latest `fsdocs` tooling the CI does this:

1. build dotnet/fsharp `feature/docs` branch (where we assume latest doc updates have been pushed)

2. builds `FSharp.Formatting` master branch

3. Uses that `FSharp.Formatting` tool to build the docs for the FSharp.Core built in step 1


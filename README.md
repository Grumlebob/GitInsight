# GitInsight

## Latest repository status

![Build & Test Workflow](https://github.com/Grumlebob/GitInsight/actions/workflows/buildAndTest.yml/badge.svg)

![Megalinter Workflow](https://github.com/Grumlebob/GitInsight/actions/workflows/mega-linter.yml/badge.svg)

[![BCH compliance](https://bettercodehub.com/edge/badge/Grumlebob/GitInsight?branch=master)](https://bettercodehub.com/)

## Set up the project

To run the program, first run the following in the terminal

```bash
dotnet ef database update
```

Afterwards open two terminals to run the backend and frontend separately.

```bash
dotnet run --project GitInsight
dotnet run --project GitInsight.Blazor
```

Head to <http://localhost:7011> to see the project.
If any errors occur, it may be because a process is already running on the api port (localhost:7273).

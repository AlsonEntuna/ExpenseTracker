project "ExpenseTracker.CurrencyConverter"
    kind "SharedLib"

    local lang = "C#"
    local framework = "net7.0-windows"

    language (lang)
    dotnetframework (netFramframeworkework)
    flags { "WPF" }
    location "../solutions/%{prj.name}"

    nuget
    {
    }

    links
    {
        "ExpenseTracker.Tools"
    }

    files
    {
        "**.cs",
        "**.xaml",
        "**.xaml.cs"
    }
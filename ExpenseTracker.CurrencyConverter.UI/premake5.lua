project "ExpenseTracker.CurrencyConverter.UI"
    kind "SharedLib"

    local lang = "C#"
    local framework = "net7.0-windows"

    language (lang)
    dotnetframework (netFramframeworkework)
    flags { "WPF" }
    location "../solutions/%{prj.name}"

    nuget
    {
        "%{NugetPackages.Mvvm}"
    }

    links
    {
        "ExpenseTracker.CurrencyConverter"
    }

    files
    {
        "**.cs",
        "**.xaml",
        "**.xaml.cs"
    }
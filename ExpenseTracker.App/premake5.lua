project "ExpenseTracker.App"
    kind "WindowedApp"

    local lang = "C#"
    local framework = "net7.0-windows"

    language (lang)
    dotnetframework (netFramframeworkework)
    flags { "WPF" }
    location "../solutions/%{prj.name}"
    targetdir "../bin"

    nuget
    {
        "%{NugetPackages.DataVis}",
        "%{NugetPackages.Mvvm}",
        "%{NugetPackages.Json}"
    }

    links
    {
        "ExpenseTracker.Wpf",
        "ExpenseTracker.CurrencyConverter.UI",
        "ExpenseTracker.Tools"
    }

    framework
    {
        "Microsoft.NETCore.App"
    }

    files
    {
        "**.cs",
        "**.xaml",
        "**.xaml.cs",
        "**.ico"
    }
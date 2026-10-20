# Architecture

This page explains how the ExpenseTracker code is organized. It's written for developers. The user guide is the [ExpenseTracker Wiki](https://alsonentuna.github.io/ExpenseTracker-Wiki/).

## Overview
ExpenseTracker is a .NET 9 WPF desktop application that uses the **MVVM** pattern.

- **Views** (`.xaml`) contain only layout and bindings.
- **ViewModels** inherit from `ExpenseTracker.Wpf.ViewModel` (an `INotifyPropertyChanged` base class with `SetProperty` / `RaisePropertyChanged`). They use `RelayCommand` from `CommunityToolkit.Mvvm` for commands.
- **Data** classes such as `VariableExpense` and `DataEntry` are also `ViewModel`s, so the UI can bind to them directly. They are saved to disk as JSON with Newtonsoft.Json, through `ExpenseTracker.Tools.JsonUtils`.

## ExpenseTracker.App layout
| Folder | Contents |
|---|---|
| `MainWindow.xaml` / `MainWindowViewModel.cs` | App shell. Handles navigation between pages, creating/opening expenses, and the update check. |
| `Data/` | Data model and services: `DataManager`, `AppInstance`, `VariableExpense`, `DataEntry`, `Categories`, `Configuration`, reports and savings. |
| `ViewModels/` | View models for the main pages: Homepage, Expenses, Report, PiggyBank and Tools & Preferences. |
| `View/` | The XAML views and windows. `View/Templates` holds the expense view, `View/Tools` the category and payment-channel editors, and `View/PiggyBank` the savings UI. |
| `Environment/Constants.cs` | File names and folder names used by the app. |
| `Utils/DataUtils.cs` | Default categories and payment channels. |

### Navigation
`MainWindowViewModel` owns one view model per page and exposes a `CurrentView` property. The menu commands (`HomepageViewCommand`, `ExpenseViewCommand`, `PiggyBankViewCommand` and `ToolsViewCommand`) swap `CurrentView`. The `DataTemplate`s in `App.xaml` then choose the matching view.

### Singletons
- `DataManager.Instance` loads and saves the app configuration (`config.cfg`) and the categories/payment channels (`categories.json`). It raises `ProvidersLoadedEvent` once loading has finished.
- `AppInstance.Connection` holds app-wide services such as the `CurrencyConverter` and the main currency. It also keeps a registry of view models, which you can look up with `GetEditorViewModel<T>()`.

## Startup flow
1. `MainWindowViewModel` is created and subscribes to `DataManager.ProvidersLoadedEvent`.
2. `DataManager.LoadAppConfiguration()` loads (or creates) `config.cfg` and `categories.json`, migrating old category data if needed, and then raises `ProvidersLoadedEvent`.
3. `InitializeData()` goes through `Config.DataLocations` (the paths of previously opened `.exp` files). For each one it deserializes a `VariableExpense`, migrates legacy data, and adds it to `ExpenseControlViewModel`.
4. **Release builds only:** `AppClientUpdater` checks GitHub Releases. If the latest tag doesn't match the app's version, it downloads the installer and asks the user whether to install it.

## Data and file locations
| File | What it holds | Release location | Debug location |
|---|---|---|---|
| `config.cfg` | List of opened expense file paths | `%LocalAppData%\ExpenseTracker\` | `<exe folder>\_data\` |
| `categories.json` | Expense categories and payment channels | `%LocalAppData%\ExpenseTracker\` | `<exe folder>\_data\` |
| `savingsdata.sav` | PiggyBank savings | `%LocalAppData%\ExpenseTracker\` | `<exe folder>\_data\` |
| `*.exp` | One `VariableExpense` (name, budget, due date, currency, entries) | Wherever the user saves it | Wherever the user saves it |
| `currency_conversion.json` | Cached exchange rates | `%LocalAppData%\ExpenseTracker\_cache\` | same |
| `ExpenseTrackerSetup.exe` | Downloaded update installer | `%LocalAppData%\ExpenseTracker\downloads\` | n/a (the updater is off in Debug) |

All of these files are JSON. Expense files are linked to the app through `Configuration.DataLocations`: saving or opening an `.exp` file adds its path to that list.

## Libraries
- **ExpenseTracker.Wpf** holds the shared look and feel. `Resources/MainTheme.xaml` merges every style under `Resources/Theme/`. The project also contains the converters, simple input dialogs (`NameDialog`, `NumDialog`, `CalendarDialog`) and `EditableTextBlock`.
- **ExpenseTracker.Tools** contains helpers with no UI dependency (`JsonUtils`, `ListUtils`, `PathUtils`).
- **ExpenseTracker.CurrencyConverter** provides `CurrencyConverter.GetCurrencyConversion(from, to)`, which calls `https://open.er-api.com/v6/latest/{from}`. Results are cached as `ConversionData`, keyed like `EUR_USD`.
- **ExpenseTracker.CurrencyConverter.UI** is the converter control and its view model.
- **ApplicationUpdater** contains `AppClientUpdater`, which uses Octokit to read releases from `AlsonEntuna/ExpenseTracker`.

## Building a release
1. Bump `<AssemblyVersion>` in `ExpenseTracker.App/ExpenseTracker.App.csproj`.
2. Bump `MyAppVersion` in `Release/BuildCompiler_ExpenseTracker.iss` to the same version.
3. Build the solution in **Release** (or run `Release/build_expense_tracker_release_x64.bat`). The output goes to `bin/Release/`.
4. Compile `Release/BuildCompiler_ExpenseTracker.iss` with Inno Setup. This produces `Release/Setup/ExpenseTrackerSetup.exe`.
5. Publish a GitHub release tagged `v<major>.<minor>.<build>` (for example `v1.8.8`) and attach the installer as the **first** asset. The updater compares against that tag and downloads `Assets[0]`.

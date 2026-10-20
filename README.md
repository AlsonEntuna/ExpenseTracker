# ExpenseTracker
[![Version](https://img.shields.io/github/v/release/AlsonEntuna/ExpenseTracker)](https://github.com/AlsonEntuna/ExpenseTracker/releases/latest)
- A WPF Application that I'm creating because of my own needs based on my own experience on how to handle bills and payments.
- Trying to implement my own libraries from the ground up and trying to make them as useful for my future projects.
- Read more about this tool here on my website [Alson Entuna - Expense Tracker](https://alsonentuna.github.io/expense-tracker.html)
- New documentation/wiki can be found here: [ExpenseTracker-Wiki](https://alsonentuna.github.io/ExpenseTracker-Wiki/)

## Tracker Main Dashboard
Main dashboard of you finances; supporting different currencies, Categories, Payment Channels, and many more
![ExpenseTracker](/Assets/ExpenseTracker-App.png)

### Expense Report Window
A comprehensive report of you expenses for the month
![ExpenseTracker](/Assets/ExpenseTracker-Report.png)

#### Breakdown Chart
![ExpenseTracker](/Assets/ExpenseTracker-Report_BreakdownChart.png)

### PiggyBank
If you want to step out from expenses and wanted to track and save for something you love.
![ExpenseTracker](/Assets/PiggyBankWindow.png)

## Requirements
- Windows 10/11
- Visual Studio 2026 (v18), or Visual Studio 2022 v17.13+ (needed to open the `.slnx` solution)
  - Workload: **.NET desktop development**
- .NET 9.0 SDK
- NuGet Package Manager - packages are restored automatically on build
- [Inno Setup](https://jrsoftware.org/isinfo.php) - only needed if you want to build the installer

## Project Structure
| Project | Type | Description |
|---|---|---|
| [ExpenseTracker.App](ExpenseTracker.App) | WPF app (`ExpenseTracker.exe`) | The main application: windows, views, view models and the expense/savings data model. |
| [ExpenseTracker.Wpf](ExpenseTracker.Wpf) | WPF library | Reusable UI pieces: the `ViewModel` base class, themes/styles, converters, dialogs (name, number, calendar) and custom controls. |
| [ExpenseTracker.Tools](ExpenseTracker.Tools) | Class library | Small non-UI helpers: JSON (de)serialization, list helpers and `%LocalAppData%` path helpers. |
| [ExpenseTracker.CurrencyConverter](ExpenseTracker.CurrencyConverter) | Class library | Fetches exchange rates from [open.er-api.com](https://open.er-api.com) and caches them locally. |
| [ExpenseTracker.CurrencyConverter.UI](ExpenseTracker.CurrencyConverter.UI) | WPF library | The currency converter control used inside the app. |
| [ApplicationUpdater](ApplicationUpdater) | Class library | Self-updater: checks GitHub Releases (via Octokit) and downloads the newest installer. |
| [Tools/ReleaseCompiler](Tools/ReleaseCompiler) | Console app | Helper that kicks off a Release build. |

Dependency overview:
```
ExpenseTracker.App
 ├── ExpenseTracker.Wpf
 ├── ExpenseTracker.CurrencyConverter.UI ──► ExpenseTracker.CurrencyConverter, ExpenseTracker.Wpf
 ├── ExpenseTracker.CurrencyConverter    ──► ExpenseTracker.Tools
 ├── ApplicationUpdater                  ──► ExpenseTracker.Tools
 └── ExpenseTracker.Tools
```

Other folders:
- `Assets/` - screenshots, icons and design source files
- `Docs/` - developer documentation and diagrams
- `ExpenseTracker.Test/data/` - sample `.exp` expense files for testing/debugging
- `Release/` - Inno Setup script and build script for the installer

## Getting Started (Development)
1. Clone the repository
   ```
   git clone https://github.com/AlsonEntuna/ExpenseTracker.git
   ```
2. Open `ExpenseTracker.slnx` in Visual Studio.
3. Set **ExpenseTracker.App** as the startup project.
4. Build and run (`F5`). All projects build into the shared `bin/` folder at the repository root.

Notes on **Debug** vs **Release** builds:
- **Debug** stores the config, categories and savings data in a `_data` folder next to the executable, so it won't touch your real data. You can open the sample files in `ExpenseTracker.Test/data/` to try things out.
- **Release** stores data in `%LocalAppData%\ExpenseTracker\` and checks GitHub for updates on startup.

For a deeper look at how the code is organized, see [Docs/Architecture.md](Docs/Architecture.md).

## Branches
- `master` - latest released version
- `develop` - ongoing development; feature branches (`dev/<feature>`) are merged here first

---
## ExpenseTracker Wiki
Documentation is available via this external link: [ExpenseTracker Wiki](https://alsonentuna.github.io/ExpenseTracker-Wiki/)

## Release
You can simply try out the tool by going to the release page here: [Release](https://github.com/AlsonEntuna/ExpenseTracker/releases)
Starting from version `1.2` the tool comes with a built-in self updater that everytime when a release is published in GitHub releases, it'll automatically download it for you.

## Contact
If you have any questions/feedback about the tool/repository, please raise a bug in the GitHub Repository or contact me via [email](mailto:alson.entuna@outlook.com).

## Donate
You can support here by donating here [PayPal Donation](https://www.paypal.com/donate/?hosted_button_id=A2EJ7MEJ3QSPL) or using the QR Code below

![ExpenseTracker](/Assets/PayPal_Donate.png)
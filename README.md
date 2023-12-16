# ExpenseTracker
[![Version](https://img.shields.io/github/v/release/AlsonEntuna/ExpenseTracker)](https://github.com/AlsonEntuna/ExpenseTracker/releases/latest)
- A WPF Application that I'm creating because of my own needs based on my own experience on how to handle bills and payments.
- Trying to implement my own libraries from the ground up and trying to make them as useful for my future projects.
- Read more about this tool here on my website [Alson Entuna - Expense Tracker](https://alsonentuna.github.io/expense-tracker.html)

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
- Visual Studio Version 17 (2022)
- .Net 7.0
- Nuget Package Manager - to install additional packages used by the tool

## Libraries
- [ExpenseTracker.App](https://github.com/AlsonEntuna/ExpenseTracker/tree/master/ExpenseTracker)
- [ExpenseTracker.Wpf](https://github.com/AlsonEntuna/ExpenseTracker/tree/master/WPFWrappers)
- [ExpenseTracker.Tools](https://github.com/AlsonEntuna/ExpenseTracker/tree/master/ExpenseTracker.Tools)
- [ExpenseTracker.CurrencyConverter](https://github.com/AlsonEntuna/ExpenseTracker/tree/master/ExpenseTracker.CurrencyConverter)
- [ExpenseTracker.CurrencyConverter.UI](https://github.com/AlsonEntuna/ExpenseTracker/tree/master/ExpenseTracker.CurrencyConverter.UI)
- [ApplicationUpdater](https://github.com/AlsonEntuna/ExpenseTracker/tree/master/ApplicationUpdater)

---
## Documentation
Documentation is available via this external link: [ExpenseTracker Documentation](https://alsonentuna.notion.site/Expense-Tracker-Documentation-a3cf8c5dd56d44f1a26e7cd72ea1d154)

## Release
You can simply try out the tool by going to the release page here: [Release](https://github.com/AlsonEntuna/ExpenseTracker/releases)
Starting from version `1.2` the tool comes with a built-in self updater that everytime when a release is published in GitHub releases, it'll automatically download it for you.

## Contact
If you have any questions/feedback about the tool/repository, please raise a bug in the GitHub Repository or contact me via [email](mailto:alson.entuna@outlook.com).


<div id="donate-button-container">
    <div id="donate-button"></div>
    <script src="https://www.paypalobjects.com/donate/sdk/donate-sdk.js" charset="UTF-8"></script>
    <script>
        PayPal.Donation.Button({
        env:'production',
        hosted_button_id:'A2EJ7MEJ3QSPL',
        image: {
        src:'https://www.paypalobjects.com/en_US/DK/i/btn/btn_donateCC_LG.gif',
        alt:'Donate with PayPal button',
        title:'PayPal - The safer, easier way to pay online!',
        }
        }).render('#donate-button');
    </script>
</div>
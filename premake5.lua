include "nugetpackages.lua"

workspace "ExpenseTrackerTest"
	architecture "x64"
	location "solutions"
	startproject "ExpenseTracker.App"
	
	configurations
	{ 
		"Debug",
		"Release"
	}

-- Libs
group "Libs"
	include "ApplicationUpdater"
	include "ExpenseTracker.Wpf"
	include "ExpenseTracker.Tools"
group ""
group "Libs/CurrencyConverter"
	include "ExpenseTracker.CurrencyConverter"
	include "ExpenseTracker.CurrencyConverter.UI"
group ""

-- Include other projects / solutions
include "ExpenseTracker.App"
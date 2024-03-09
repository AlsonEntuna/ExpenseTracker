include "thirdparty.lua"

workspace "ExpenseTracker"
	architecture "x64"
	location "solutions"
	startproject "ExpenseTracker.App"
	
	configurations
	{ 
		"Debug", 
		"Release"
	}
	
	local lang = "C++"
	local dialect = "C++20"
	
	local buildname = "%{prj.name}_%{cfg.buildcfg}"
	local builddir = ("bin/" .. buildname)
	local intermediate = ("intermediate/" .. buildname)
	local solutionlocations = "src/solutions/%{prj.name}"

-- ThirdParty solutions
group "Libs"
	include "thirdparty/imgui"
	include "thirdparty/usd"
group ""

-- Include other projects / solutions
include "src/Editor"
include "src/CautionEngine"
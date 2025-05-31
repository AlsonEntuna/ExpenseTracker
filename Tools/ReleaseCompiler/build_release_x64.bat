@echo OFF
echo "Starting build for ExpenseTracker Release"
call "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.com" "../../../../../ExpenseTracker.sln" /build Release /out "build_log.log"
echo "Build completed..."
pause
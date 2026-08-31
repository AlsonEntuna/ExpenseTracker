@echo OFF
echo "Starting build for ExpenseTracker Release"
call "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\devenv.com" "../ExpenseTracker.slnx" /build Release /out "build_log.log"
echo "Build completed..."
pause
@echo off
REM Скрипт для тестирования всех Windows-приложений в проекте
REM Этот скрипт должен запускаться на Windows с установленным .NET 8.0 SDK

echo ===== Тестирование совместимости .NET версий =====
echo.

echo Проверка сборки всех проектов...
dotnet build

if %ERRORLEVEL% neq 0 (
    echo ОШИБКА: Сборка завершилась с ошибками
    pause
    exit /b 1
)

echo.
echo ===== Сборка успешна! =====
echo.

echo Тестирование консольного приложения...
timeout /t 3 /nobreak > nul
start "Console App" cmd /c "dotnet run --project ProductManagementSystem.ConsoleApp && pause"

echo.
echo Тестирование Windows Forms приложения...
timeout /t 2 /nobreak > nul
start "WinForms App" dotnet run --project ProductManagementSystem.WinFormsApp

echo.
echo Тестирование WPF приложения...
timeout /t 2 /nobreak > nul
start "WPF App" dotnet run --project ProductManagementSystem.WpfApp

echo.
echo ===== Все приложения запущены =====
echo Проверьте, что все окна открылись без ошибок и текст отображается корректно.
echo.
pause
@echo off
setlocal enabledelayedexpansion

echo ==========================================
echo   ICT Master Suite - Auto Commit Quick
echo ==========================================
echo.

REM Go to this script's directory (project root)
cd /d "%~dp0"
echo [1/4] Diretório do projeto: %cd%
echo.

echo [2/4] Executando git add . . .
git add .
if errorlevel 1 (
    echo [ERRO] Falha no git add.
    pause
    exit /b 1
)
echo [OK] git add concluído.
echo.

for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format \"yyyy-MM-dd HH:mm:ss\""') do set "NOW=%%i"
set "COMMIT_MSG=auto-commit !NOW!"

echo [3/4] Executando git commit com mensagem automática:
echo        "!COMMIT_MSG!"
git commit -m "!COMMIT_MSG!"
if errorlevel 1 (
    echo [ERRO] Falha no git commit.
    pause
    exit /b 1
)
echo [OK] git commit concluído.
echo.

echo [4/4] Executando git push . . .
git push
if errorlevel 1 (
    echo [ERRO] Falha no git push.
    pause
    exit /b 1
)
echo [OK] git push concluído.
echo.

echo Processo quick finalizado com sucesso.
echo ==========================================
pause
exit /b 0

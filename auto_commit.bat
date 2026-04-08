@echo off
setlocal

echo ==========================================
echo   ICT Master Suite - Auto Commit
echo ==========================================
echo.

REM Go to this script's directory (project root)
cd /d "%~dp0"
echo [1/5] Diretório do projeto: %cd%
echo.

echo [2/5] Executando git add . . .
git add .
if errorlevel 1 (
    echo [ERRO] Falha no git add.
    pause
    exit /b 1
)
echo [OK] git add concluído.
echo.

set /p COMMIT_MSG=Digite a mensagem do commit: 
if "%COMMIT_MSG%"=="" (
    echo [ERRO] Mensagem de commit não pode ser vazia.
    pause
    exit /b 1
)
echo.

echo [3/5] Executando git commit . . .
git commit -m "%COMMIT_MSG%"
if errorlevel 1 (
    echo [ERRO] Falha no git commit.
    pause
    exit /b 1
)
echo [OK] git commit concluído.
echo.

echo [4/5] Executando git push . . .
git push
if errorlevel 1 (
    echo [ERRO] Falha no git push.
    pause
    exit /b 1
)
echo [OK] git push concluído.
echo.

echo [5/5] Processo finalizado com sucesso.
echo ==========================================
pause
exit /b 0

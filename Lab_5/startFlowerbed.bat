@echo off
set /a flowerbed_capacity=8
start Lab_4\bin\Debug\Lab_4.exe flowerbed %flowerbed_capacity%
pause

taskkill /im Lab_4.exe /f
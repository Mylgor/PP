@echo off
set /a flowerbed_capacity=8
set /a flower_count=5

start Lab_4\bin\Debug\Lab_4.exe flowerbed %flowerbed_capacity%
for /l %%A in (1, 1, %flower_count%) do (
	start Lab_4\bin\Debug\Lab_4.exe flower
)
start Lab_4\bin\Debug\Lab_4.exe gardener
start Lab_4\bin\Debug\Lab_4.exe gardener

pause

taskkill /im Lab_4.exe /f
echo off
cd  "C:\\Users\\Zoe\\Documents\\SubMain"

:loop

FOR /l %%x IN (1,1,30) DO (
         ECHO %%x > "Ghis%%x.txt"
      ping 127.255.255.255 -n 1 -w 10000> nul
)


goto loop
 
PAUSE
cd..
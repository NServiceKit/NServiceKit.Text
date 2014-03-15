
REM SET BUILD=Debug
SET BUILD=Release

COPY ..\src\NServiceKit.Text\bin\%BUILD%\*.* ..\..\NServiceKit\release\latest\
COPY ..\src\NServiceKit.Text\bin\%BUILD%\*.* ..\..\NServiceKit\release\latest\NServiceKit.Text\
COPY ..\src\NServiceKit.Text\bin\%BUILD%\*.* ..\..\NServiceKit\lib
COPY ..\src\NServiceKit.Text\bin\%BUILD%\*.* ..\..\NServiceKit.Contrib\lib
COPY ..\src\NServiceKit.Text\bin\%BUILD%\*.* ..\..\NServiceKit.Redis\lib
COPY ..\src\NServiceKit.Text\bin\%BUILD%\*.* ..\..\NServiceKit.OrmLite\lib
COPY ..\src\NServiceKit.Text\bin\%BUILD%\*.* ..\..\NServiceKit.Examples\lib
COPY ..\src\NServiceKit.Text\bin\%BUILD%\*.* ..\..\NServiceKit.RedisWebServices\lib

COPY ..\src\NServiceKit.Text.SL5\bin\%BUILD%\*.* ..\..\NServiceKit\lib\sl5

MD ..\NuGet\lib\net35
COPY ..\src\NServiceKit.Text\bin\%BUILD%\*.* ..\NuGet\lib\net35

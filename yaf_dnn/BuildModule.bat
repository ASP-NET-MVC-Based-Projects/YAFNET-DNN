@SET FrameworkSDKDir=
@SET PATH=%FrameworkDir%;%FrameworkSDKDir%;%PATH%
@SET LANGDIR=EN
@SET MSBUILDPATH="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MsBuild.exe"

.nuget\nuget.exe restore YAF.DNN.Module.sln

%MSBUILDPATH% YAF.DNN.Module.sln /p:Configuration=Release /t:Clean;Build /p:WarningLevel=0;CreateDnnPackages=true /flp1:logfile=errors.txt;errorsonly %1 %2 %3 %4 %5 %6 %7 %8 %9 
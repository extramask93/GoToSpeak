# GoToSpeak
Project is divided into 2 separate projects: server side in c# "GoToSpeak" and client(GUI) side "GoToSpeak-SPA". 
# Running Server Side:
1. Install Visual Studio Code,
2. Install DotNet Core 2.2 : https://dotnet.microsoft.com/download/dotnet-core/2.2
2. git clone https://github.com/extramask93/GoToSpeak.git
3. git checkout identity
4. cd GoToSpeak
5. code .
6. Go to addons tab and isntall:
  * C# for visual studio code
  * c# extensions
  * NuGet Package Manager,
7. restore nuget dependecies
8. Go to terminal in vs terminal:
* *cd GoToSpeak*
* *dotnet ef migrations add <some_name>*
* *dotnet ef database update*,
* *dotnet watch run*
* ->ctrl+c to stop server
# Running Client Side:
1. Install node.js
3. cd into GoToSpeak-SPA
* *npm install* (sudo if linux)
* *npm install bootswatch*
* To run: ->*ng serve*

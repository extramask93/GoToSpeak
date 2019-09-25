# GoToSpeak
Project is divided into 2 separate projects: server side in c# "GoToSpeak" and client(GUI) side "GoToSpeak-SPA". 
# Running Server Side:
1. Install Visual Studio Code,
2. git clone https://github.com/extramask93/GoToSpeak.git
3. git checkout identity
4. cd GoToSpeak
5. code .
Go to addons tab and isntall:
5.1. *C# for visual studio code
5.2. *c# extensions
5.3. *NuGet Package Manager,
6. restore nuget dependecies
Go to terminal in vs terminal:
6.1.->cd GoToSpeak
6.3.->dotnet ef migrations add <jakas_nazwa>
6.2.->dotnet ef database update,
6.3.->dotnet watch run
->ctrl+c to stop server
# Running Client Side:
1. Install node.js
2. cd into GoToSpeak-SPA
3. ->npm install (sudo if linux)
4. ->npm install bootswatch
4. To run: ->ng serve

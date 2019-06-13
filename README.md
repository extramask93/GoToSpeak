# GoToSpeak
Project is divided into 2 separate projects: server side in c# "GoToSpeak" and client(GUI) side "GoToSpeak-SPA". 
# Running Server Side:
1. Install Visual Studio Code,
2. git clone https://github.com/extramask93/GoToSpeak.git
3. cd GoToSpeak
4. code .
Go to addons tab and isntall:
	*C# for visual studio code
	*c# extensions
	*NuGet Package Manager,
restore nuget dependecies
Go to build in vs terminal:
->cd GoToSpeak
->dotnet ef database update
before first run uncomment database seeding statement->Startup.cs->seed.SeedUsers();
->dotnet watch run
->ctrl+c to stop server
# Running Client Side:
1. Install node.js
2. cd into GoToSpeak-SPA
3. ->npm install (sudo if linux)
4. ->npm install bootswatch
4. To run: ->ng serve
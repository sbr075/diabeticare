## Mobile application implementation for Diabeticare
This is the **unfinished** version of Diabeticare for mobile devices.
All programming and testing was done on Windows using an Android emulator with the help of [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
**NB!** These guidelines are for Windows using Visual Studio 2022
  
## Table of Contents
- [Setup](#Setup)
  - [Dependencies Installation](#Dependencies-Installation)
    - [Current Dependencies](#Current-Dependencies)
    
## Setup
There are four steps required to test the application
1. Download the source code and extract it to a folder on your computer
2. Open Visual Studio 2022, select 'Open a project or solution', and navigate to the Diabeticare folder until you find 'Diabeticare.sln'
3. Create a virtual device or connect your device to the computer
4. Build the program and open the application (either by running the simulator or on your device)
**NB!** The Diabeticare backend server needs to be running to test the application
**NB!** The current iteration of the application has a flag to enable HTTP communication and is unsafe. **DO NOT USE IN DEPLOYMENT**

### Dependencies Installation
When you open Visual Studio 2022 all dependencies to run the application should automatically be installed
To see what dependencies are installed use the NuGet package manager found in Visual Studio 2022
> Tools -> NuGet Package Manager -> Mange NuGet Packages for Solution...

#### Current Dependencies
1. Microcharts (0.9.5.9)
2. Microcharts.Forms (0.9.5.9)
3. NETStandard.Library (2.0.3)
4. Newtonsoft.Json (13.0.1)
5. Refractored.MvvmHelpers (1.6.2)
6. sqlite-net-pcl (1.8.116)
7. System.Security.Cryptography.Algorithms (4.3.1)
8. Xamarin.CommunityToolkit (2.0.1)
9. Xamarin.Essentials (1.7.2)
10. Xamarin.Forms (5.0.0.2401)

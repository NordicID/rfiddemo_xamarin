## rfiddemo_xamarin
Build RFID apps for Android, iOS, and Windows by using Visual Studio(2019). This sample has been built using NurApi (.NET Standard 2.0 Class Library) defining types and methods for creating sophisticate RFID applications for the Nordic ID readers. The NurApi allows to be called by any .NET implementation that supports that version of .NET Standard.
  
### NurApi libraries by Nordic ID
**NidRefs** folder contains NuGet packages and reference assemblies needed for the app development.
 
- `NordicID.NurApi.Net.dll` .NET Standard 2.0 Class Library
- `NordicID.NurApi.Android.dll` Android related functions like BLE scanner and transport.
- `NordicID.NurApi.Support.dll` Helper classes for complicated RFID tasks like Sensor Tag reading, Tag Locating and Tag Data Translation operations.

`NordicID.NurApi.Net.dll` is code compatible with NurApiDotNet class library for Windows .NETFramework. using same namespace: `NurApiDotNet`

Development of these libraries  are ongoing. Please check updates frequently on this page. In case you need help, please contact to NordicID support. (support@nordicid.com)
####  Running sample on HH83
Connecting to internal RFID module: Go to Connection page, press “TCP CONNECTION” button and type: `127.0.0.1:6734/?name=HH83`
This will be changed in future releases as 'internal" connection.

## Release 22.4.2020
**Note! Major changes compared to release 1.3.2020**
Applications developed using previous versions of NurApi libraries are not directly compatible with this release.  Changes concerns mainly for the transport and device discovery functionalities.

- Prebuilt *apk [RFID Demo Xamarin version 2.0.0 for Android](https://github.com/NordicID/rfiddemo_xamarin/releases/tag/v2.0.0)
- New versions. `NurApi.Net 3.0.0` `NurApi.Android 2.0.0` `NurApi.Support 2.0.0`
- This sample developed and tested using Android.
-  iOS support comes later. 
- UWP works but there is some image displaying and text input issues.

## Release 1.3.2020
- Initial release versions. `..NurApi.Net.Dll 2.0.0` `..Android.dll 1.0.0` `..Support.Dll 1.0.0`
- This sample developed and tested using Android.
-  iOS and UWP support comes later. 
- BLE and TCP transport for readers supported.
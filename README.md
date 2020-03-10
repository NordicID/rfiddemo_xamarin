## rfiddemo_xamarin
 Build RFID apps for Android, iOS, and Windows devices by using Visual Studio(2019). This sample has been built using NurApi (.NET Standard 2.0 Class Library) defining types and methods for creating sophisticate RFID applications for the Nordic ID readers. The NurApi allows to be called by any .NET implementation that supports that version of .NET Standard.
  
### Reference assemblies by Nordic ID
**NidRefs** folder contains latest reference assemblies needed for the app development.
 **Currently, these assemblies and demo is under intensive development.** New features and fixes will be updated frequently. There are main features implemented and app development can be started using these assemblies.

- `NordicID.NurApi.Net.dll` .NET Standard 2.0 Class Library
- `NordicID.NurApi.Android.dll` (Android 8.1 Oreo)
- `NordicID.NurApi.Support.dll` contains helper classes for complicated RFID tasks like Sensor Tag reading, Tag Locating and Tag Data Translation operations.

In the future, these assemblies going to be publish as NuGet packages at Nuget.org.
`NordicID.NurApi.Net.dll` is code compatible with NurApiDotNet class library for Windows .NETFramework. using same namespace: `NurApiDotNet`

 Prebuilt folder contains `com.nordicid.rfiddemo_xamarin-Signed.apk` for ready to install in to the Android devices.
 
As mentioned earlier, development of libraries are still ongoing. Please check updates frequently on this page. In case you need help, please contact to NordicID support. (support@nordicid.com)
####  Running sample on HH83
Connecting to internal RFID module: Go to Connection page, press “TCP CONNECTION” button and type: `127.0.0.1:6734/?name=HH83`
This will be changed in future releases as 'internal" connection.

### Release notes (1.3.2020)
- Initial release versions. `..NurApi.Net.Dll 2.0.0` `..Android.dll 1.0.0` `..Support.Dll 1.0.0`
- This sample developed and tested using Android.
-  iOS and UWP support comes later. 
- BLE and TCP transport for readers supported.
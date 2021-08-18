## rfiddemo_xamarin
Build RFID apps for Android, iOS, and Windows by using Visual Studio(2019). This sample has been built using NurApi (.NET Standard 2.0 Class Library) defining types and methods for creating sophisticate RFID applications for the Nordic ID readers. The NurApi allows to be called by any .NET implementation that supports that version of .NET Standard.
  
### NurApi libraries by Nordic ID
**NidRefs** folder contains NuGet packages and reference assemblies needed for the app development.
 
- `NordicID.NurApi.Net.dll` .NET Standard 2.0 Class Library
- `NordicID.NurApi.Android.dll` Android related functions like BLE scanner and transport.
- `NordicID.NurApi.iOS.dll` IOS related functions like BLE scanner and transport.
- `NordicID.NurApi.Support.dll` Helper classes for complicated RFID tasks like Sensor Tag reading, Tag Locating and Tag Data Translation operations.
- `NordicID.UpdateLib.Net.dll` .NET Standard 2.0 Class Library for updating Nordic ID reader firmware. *(Note: Currently,  EXA device firmware cannot update with this library.)*

`NordicID.NurApi.Net.dll` is code compatible with NurApiDotNet class library for Windows .NETFramework. using same namespace: `NurApiDotNet`

Development of these libraries  are ongoing. Please check updates frequently on this page. In case you need help, please contact to NordicID support. (support@nordicid.com)
####  Connecting to reader
Run demo and select "Connection" from main menu. The device discovery will be activated and trying to search available Bluetooth LE (EXA) devices and other readers attached to the TCP network. Select device from list to connect. The NordicID HHxx devices has Integrated reader. Connecting to the Integrated reader programmatically:
`Uri intUri = new Uri("int://integrated_reader/?name=" + "Integrated reader");
App.Nur.Connect(newUri);`

## Release 18.8.2021
#### `RFIDDemo App v2.4.0`
 - Library updates
- This sample disconnect reader connection when app enter to inactive state (OnSleep())
 - Prebuilt apk (Android) and *.ipa (iOS)  in Prebuilt folder.
#### `NurApi.Net v3.0.5`
 - Uri null check
  #### `NordicId.NurApi.iOS v1.0.2`
  - Fixed reader transport bug were frequent connect/disconnect causes transport failures.


## rfiddemo_xamarin
Build RFID apps for Android, iOS, and Windows by using Visual Studio(2022). This sample has been built using NurApi (.NET Standard 2.0 Class Library) defining types and methods for creating sophisticate RFID applications for the Nordic ID readers. The NurApi allows to be called by any .NET implementation that supports that version of .NET Standard.
  
### Documentation
See https://github.com/NordicID/nur_sample_csharp for documentation

## Release 19.8.2021
#### `RFIDDemo App v2.4.1`
- Library updates 
- More iOS DarkMode friendly
  #### `NordicId.NurApi.iOS v1.0.3`
  - Fixed reader BLE discovery bug from NurApi.iOS.


## Release 18.8.2021
#### `RFIDDemo App v2.4.0`
 - Library updates.
- This sample disconnect reader connection when app enter to inactive state (OnSleep())
 - Prebuilt apk (Android) and *.ipa (iOS)  in Prebuilt folder.
#### `NurApi.Net v3.0.5`
 - Uri null check
  #### `NordicId.NurApi.iOS v1.0.2`
  - Fixed reader transport bug were frequent connect/disconnect causes transport failures.

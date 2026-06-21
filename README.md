# CapCognition .NET MAUI Samples

Practical .NET MAUI samples for the CapCognition SDK.

This repository demonstrates how to use CapCognition in mobile applications for real-time camera capture, barcode recognition, QR code recognition, license plate recognition, YOLO-based object detection and custom computer vision workflows.

CapCognition provides .NET and .NET MAUI SDKs for building camera-based recognition applications on Android and iOS.

## What you can learn from this repository

This sample project shows how to:

* Build a .NET MAUI camera application
* Use live camera preview in a MAUI app
* Capture images from the device camera
* Run barcode and QR code recognition
* Run license plate recognition
* Use CapCognition recognition processors in a mobile app
* Integrate YOLO-based object detection workflows
* Build mobile computer vision applications with C#
* Structure a .NET MAUI recognition app for Android and iOS
* Connect camera capture, processing and result display in one application

## Typical use cases

This repository is useful for developers who want to build:

* .NET MAUI barcode scanner apps
* QR code scanner apps for Android and iOS
* Mobile license plate recognition applications
* ANPR / ALPR mobile apps
* Parking control applications
* Vehicle access control apps
* Mobile computer vision tools
* Camera-based inspection apps
* YOLO-based mobile object detection demos
* Custom recognition workflows in C#

## Technologies used

* .NET MAUI
* C#
* Android
* iOS
* CapCognition MAUI SDK
* Camera capture
* Barcode recognition
* QR code recognition
* License plate recognition
* YOLO object detection
* Computer vision
* Image processing

## Supported platforms

The CapCognition .NET MAUI samples are intended for mobile platforms:

| Platform | Status                                    |
| -------- | ----------------------------------------- |
| Android  | Supported                                 |
| iOS      | Supported                                 |
| Windows  | Not supported for MAUI camera recognition |
| macOS    | Not supported for MAUI camera recognition |

The sample focuses on mobile camera-based recognition scenarios.

## Requirements

* .NET SDK
* .NET MAUI workload
* Visual Studio, Visual Studio Code or JetBrains Rider
* Android SDK for Android builds
* Xcode and macOS for iOS builds
* A physical Android or iOS device for camera testing
* A CapCognition license or trial configuration

Camera-based recognition should normally be tested on a real device. Emulators and simulators often have limited or no camera functionality.

## Getting started

Clone the repository:

```bash
git clone https://github.com/CapCognition/NetMaui-samples.git
cd NetMaui-samples
```

Restore dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

Run the app on Android:

```bash
dotnet build -t:Run -f net10.0-android
```

For iOS, build and run the project from macOS with Xcode tooling available.

## Opening the project in an IDE

You can open the solution file directly:

```text
NetMaui-samples.sln
```

Recommended development environments:

* Visual Studio with .NET MAUI workload
* Visual Studio Code with C# Dev Kit and .NET MAUI tooling
* JetBrains Rider with MAUI support

## Camera permissions

The application requires camera permissions on mobile devices.

On Android, make sure the app has camera permission in the Android manifest.

On iOS, make sure the app contains a camera usage description in the iOS platform configuration.

A typical iOS camera permission text looks like:

```xml
<key>NSCameraUsageDescription</key>
<string>This app uses the camera for barcode, QR code and license plate recognition.</string>
```

## Recognition workflow

A typical CapCognition MAUI recognition workflow contains:

1. A camera preview
2. Frame capture from the device camera
3. One or more recognition processors
4. Barcode, QR code, license plate or YOLO detection
5. Result handling
6. Optional visual overlays
7. Display of recognition results in the app UI

This makes it possible to build mobile apps that process live camera input and react to detected objects, codes or license plates.

## Barcode and QR code recognition

Use the barcode recognition samples as a starting point if you want to build:

* QR code scanner apps
* Barcode scanner apps
* Inventory scanning tools
* Ticket validation apps
* Access control apps
* Mobile data capture workflows

The recognition processor can be connected to the camera pipeline and used to process captured frames.

## License plate recognition

Use the license plate recognition samples as a starting point if you want to build:

* Mobile ANPR apps
* Mobile ALPR apps
* Parking enforcement apps
* Vehicle access control apps
* Gate control apps
* Field inspection tools

License plate recognition can be combined with camera overlays, result validation and backend APIs.

## YOLO object detection

The YOLO-related samples can be used as a starting point for custom object detection workflows.

Typical scenarios include:

* Detecting custom objects in camera frames
* Running trained YOLO models in a mobile app
* Combining object detection with barcode or license plate recognition
* Building mobile inspection workflows
* Creating AI-assisted camera applications

## Project structure

| Path                     | Purpose                                         |
| ------------------------ | ----------------------------------------------- |
| `NetMaui-samples.sln`    | Solution file                                   |
| `NetMaui-samples.csproj` | .NET MAUI project file                          |
| `MauiProgram.cs`         | MAUI app startup and service registration       |
| `App.xaml`               | Application resources                           |
| `App.xaml.cs`            | Application startup code                        |
| `Platforms/`             | Platform-specific Android and iOS configuration |
| `Resources/`             | App icons, fonts, images and raw assets         |
| `Views/`                 | Application pages and UI views                  |
| `Properties/`            | Launch settings and project properties          |

## License setup

The sample may contain placeholders for CapCognition license configuration.

Look for code sections where CapCognition features are initialized and replace placeholder values with your own CapCognition license or trial configuration.

Depending on the packages used in your application, this may include initialization for:

```csharp
// Example only - adapt this to the actual package and license configuration used in your project.
BarcodeRecognition.Use(/* Your license */);
LicensePlateDetection.Use(/* Your license */);
YoloModelDetection.Use(/* Your license */);
```

## Performance notes

Mobile recognition workloads can be performance-sensitive.

For production applications, consider:

* Reusing recognition processors instead of recreating them for every frame
* Reducing the processed frame resolution where possible
* Processing only selected frames instead of every camera frame
* Avoiding heavy work on the UI thread
* Using asynchronous processing
* Keeping overlays lightweight
* Testing performance on real target devices
* Handling camera lifecycle events carefully

This is especially important for:

* Real-time barcode scanning
* Continuous license plate recognition
* YOLO-based object detection
* Older Android devices
* Long-running camera sessions

## Recommended production considerations

Before using the sample code in a production app, review:

* Camera permission handling
* App lifecycle handling
* Error handling
* Device orientation handling
* Background and foreground transitions
* Performance on low-end devices
* Recognition timeout handling
* License validation
* Offline model deployment
* Network connectivity requirements
* Privacy and data protection requirements

## Useful links

* Website: https://capcognition.com
* Documentation: https://docu.capcognition.com
* Pricing: https://capcognition.com/page/pricing
* GitHub organization: https://github.com/CapCognition
* .NET LTS samples: https://github.com/CapCognition/NetLTS-samples

## Related CapCognition topics

* .NET MAUI camera capture
* .NET MAUI barcode scanning
* .NET MAUI QR code recognition
* .NET MAUI license plate recognition
* Mobile ANPR
* Mobile ALPR
* Android camera recognition
* iOS camera recognition
* YOLO object detection in .NET MAUI
* Computer vision in C#
* Mobile image processing
* Parking control apps
* Access control apps

## License

This sample repository is licensed under the MIT License.

CapCognition SDK packages may require their own license depending on the package and usage scenario.

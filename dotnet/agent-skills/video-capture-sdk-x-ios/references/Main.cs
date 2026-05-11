using SimpleVideoCaptureX;

// .NET for iOS top-level program — wires the AppDelegate as the UIApplicationDelegate.
// AppDelegate constructs the root UIViewController inline (no separate
// MainViewController.cs file in this sample); the camera preview and controls live
// directly on vc.View built up in AppDelegate.FinishedLaunching.
UIApplication.Main(args, null, typeof(AppDelegate));

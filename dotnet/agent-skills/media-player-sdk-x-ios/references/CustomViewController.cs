using System.Diagnostics;
using VisioForge.Core.UI.Apple;

namespace MediaPlayer;

using UIKit;
using Foundation;

/// <summary>
/// Hosts the VideoView, the SELECT FILE / PLAY buttons, and the position slider.
/// Also acts as the IUIDocumentPickerDelegate for picking the media file.
/// </summary>
public class CustomViewController : UIViewController, IUIDocumentPickerDelegate
{
    private UIDocumentPickerViewController _documentPicker;
    private NSUrl _sourceFileUrl;
    private UIWindow _window;

    public UIButton SelectButton { get; private set; }
    public UIButton PlayButton { get; private set; }
    public UISlider PositionSlider { get; private set; }
    public UIWindow Window => _window;

    /// <summary>
    /// Initializes a new instance, building the VideoView at full window size and
    /// returning it via <paramref name="videoView"/> so AppDelegate can hand it to MediaPlayerCoreX.
    /// </summary>
    public CustomViewController(UIWindow window, out UIView videoView)
    {
        _window = window;
        var rect = new CGRect(0, 0, _window.Frame.Width, _window.Frame.Height);
        videoView = AddVideoView(rect);
        AddButtons();
    }

    /// <summary>
    /// Open the document picker. UTType.Content / Item / Data covers every common
    /// container (mp4, mkv, mov, mp3, flac, ...) without per-format whitelisting.
    /// </summary>
    public void OpenFilePicker()
    {
        try
        {
            var contentTypes = new UniformTypeIdentifiers.UTType[]
            {
                UniformTypeIdentifiers.UTTypes.Content,
                UniformTypeIdentifiers.UTTypes.Item,
                UniformTypeIdentifiers.UTTypes.Data
            };
            // asCopy: true — UIKit copies the picked file into the app's temp directory and
            // hands back a freely-readable URL, so we don't have to bracket every read with
            // StartAccessingSecurityScopedResource / StopAccessingSecurityScopedResource.
            _documentPicker = new UIDocumentPickerViewController(contentTypes, asCopy: true)
            {
                Delegate = this,
                ModalPresentationStyle = UIModalPresentationStyle.FullScreen
            };

            PresentViewController(_documentPicker, true, null);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    [Export("documentPicker:didPickDocumentsAtURLs:")]
    public void DidPickDocument(UIDocumentPickerViewController controller, NSUrl[] urls)
    {
        _sourceFileUrl = urls.FirstOrDefault();
        controller.DismissViewController(true, null);
    }

    [Export("documentPickerWasCancelled:")]
    public void WasCancelled(UIDocumentPickerViewController controller)
    {
        controller.DismissViewController(true, null);
    }

    private UIView AddVideoView(CGRect rect)
    {
        // VisioForge.Core.UI.Apple.VideoView is the iOS renderer the SDK accepts via
        // (IVideoView) cast in MediaPlayerCoreX(IVideoView). It IS a UIView, so it
        // can be used anywhere in your view hierarchy.
        var videoView = new VideoView(rect);
        View.AddSubview(videoView);
        return videoView;
    }

    private void AddButtons()
    {
        nfloat newTop = _window.Bounds.Height - 50;

        SelectButton = new UIButton(new CGRect(20, newTop, 150, 30))
        {
            BackgroundColor = UIColor.Gray,
            AutoresizingMask = UIViewAutoresizing.None,
            VerticalAlignment = UIControlContentVerticalAlignment.Center,
            HorizontalAlignment = UIControlContentHorizontalAlignment.Center
        };
        SelectButton.SetTitle("SELECT FILE", UIControlState.Normal);
        View!.AddSubview(SelectButton);

        PlayButton = new UIButton(new CGRect(180, newTop, 100, 30))
        {
            BackgroundColor = UIColor.Gray,
            AutoresizingMask = UIViewAutoresizing.None,
            VerticalAlignment = UIControlContentVerticalAlignment.Center,
            HorizontalAlignment = UIControlContentHorizontalAlignment.Center
        };
        PlayButton.SetTitle("PLAY", UIControlState.Normal);
        View!.AddSubview(PlayButton);

        PositionSlider = new UISlider(
            new CGRect(
                PlayButton.Frame.Right + 30,
                newTop,
                _window.Bounds.Width - SelectButton.Bounds.Width - PlayButton.Bounds.Width - 50,
                30))
        {
            AutoresizingMask = UIViewAutoresizing.None,
            Value = 0,
            MinValue = 0,
            MaxValue = 100
        };
        View!.AddSubview(PositionSlider);
    }

    public NSUrl GetURL() => _sourceFileUrl;
}

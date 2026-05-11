using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;

namespace VE_Console_Sample
{
    /// <summary>
    /// Headless batch sample: cut the middle 10 s out of an input MP4 and
    /// re-encode the result to a new MP4. Demonstrates the pieces every
    /// console host needs from Video Edit SDK .NET:
    ///   - VideoEditCore construction without an IVideoView (no preview surface).
    ///   - Setting Mode = Convert and Video_Renderer.VideoRenderer = None so the
    ///     engine does not try to attach to a UI thread.
    ///   - A TaskCompletionSource that fires on OnStop / OnError, so Main can
    ///     await pipeline completion before returning (otherwise the process
    ///     exits while the muxer is still flushing — see SKILL.md pitfall #4).
    /// </summary>
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: VEConsoleSample <input.mp4> <output.mp4>");
                return 2;
            }

            var input = args[0];
            var output = args[1];

            if (!File.Exists(input))
            {
                Console.Error.WriteLine($"Input file not found: {input}");
                return 2;
            }

            var core = new VideoEditCore();
            core.Mode = VideoEditMode.Convert;

            // To register a purchased licence, uncomment below and point at your
            // .vflicense file. Without this, the SDK runs in 30-day trial mode.
            //
            // var cert = File.ReadAllBytes("path/to/your.vflicense");
            // await core.SetLicenseCertificateAsync(cert);

            // No window, no preview surface. Required for headless console hosts —
            // without it the engine tries to spin up a renderer and fails silently.
            core.Video_Renderer.VideoRenderer = VideoRendererMode.None;

            // Take seconds 5..15 of the input. For a full transcode (no cut),
            // pass TimeSpan.Zero for both — the SDK reads the whole file.
            var cutStart = TimeSpan.FromSeconds(5);
            var cutStop = TimeSpan.FromSeconds(15);

            await core.Input_AddVideoFileAsync(
                new VideoSource(input, cutStart, cutStop, VideoEditStretchMode.Letterbox));

            await core.Input_AddAudioFileAsync(
                new AudioSource(input, cutStart, cutStop, string.Empty));

            core.Output_Filename = output;
            core.Output_Format = new MP4Output();

            // Console hosts MUST await OnStop before disposing — StartAsync only
            // kicks off the pipeline; completion is signalled asynchronously.
            var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            core.OnProgress += (_, e) =>
                Console.Out.WriteLine($"Progress: {e.Progress}%");

            core.OnStop += (_, e) =>
                done.TrySetResult(e.Successful);

            core.OnError += (_, e) =>
            {
                Console.Error.WriteLine($"Error: {e.Message}");
                done.TrySetResult(false);
            };

            Console.WriteLine($"Encoding {input} -> {output} (cut {cutStart}..{cutStop})");
            await core.StartAsync();

            var ok = await done.Task;

            core.Stop();
            core.Dispose();

            if (!ok)
            {
                Console.Error.WriteLine("Encode failed.");
                return 1;
            }

            Console.WriteLine("Done.");
            return 0;
        }
    }
}

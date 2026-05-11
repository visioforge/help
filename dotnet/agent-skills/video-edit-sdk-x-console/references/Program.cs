using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

namespace VEX_Console_Sample
{
    /// <summary>
    /// Headless batch sample for Video Edit SDK X (cross-platform "X" edition).
    /// Cuts seconds 5..15 from an input video file and re-encodes the result to
    /// a new MP4. Demonstrates the pieces every console host needs:
    ///   - VisioForgeX.InitSDKAsync() before any VideoEditCoreX construction
    ///     (mandatory; first call builds the GStreamer registry cache).
    ///   - VideoEditCoreX with the no-argument constructor (no IVideoView —
    ///     there is no UI thread to attach to).
    ///   - A TaskCompletionSource that fires on OnStop / OnError so Main can
    ///     await pipeline completion before returning (otherwise the process
    ///     exits while the muxer is still flushing — see SKILL.md pitfall #2).
    ///   - VisioForgeX.DestroySDK() on shutdown.
    /// </summary>
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: VEXConsoleSample <input.mp4> <output.mp4>");
                return 2;
            }

            var input = args[0];
            var output = args[1];

            if (!File.Exists(input))
            {
                Console.Error.WriteLine($"Input file not found: {input}");
                return 2;
            }

            var outputDir = Path.GetDirectoryName(output);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Console.Error.WriteLine($"Output directory does not exist: {outputDir}");
                return 2;
            }

            // Mandatory engine boot. First run on a fresh machine builds the
            // GStreamer plugin-registry cache (~2-5 s); subsequent launches are
            // instant. Skipping this is the #1 source of "DLL not found" /
            // "no element X" failures — see SKILL.md pitfall #1.
            await VisioForgeX.InitSDKAsync();

            var core = new VideoEditCoreX();

            // To register a purchased licence, uncomment below and point at
            // your .vflicense file. Without this, the SDK runs in 30-day trial.
            //
            // var cert = File.ReadAllBytes("path/to/your.vflicense");
            // await core.SetLicenseCertificateAsync(cert);

            // Take seconds 5..15 of the input. For a full transcode (no cut),
            // pass TimeSpan.Zero for both — the SDK reads the whole file.
            var cutStart = TimeSpan.FromSeconds(5);
            var cutStop = TimeSpan.FromSeconds(15);

            core.Input_AddVideoFile(new VideoFileSource(input, cutStart, cutStop));
            core.Input_AddAudioFile(new AudioFileSource(input, cutStart, cutStop));

            // X-family Output_Format takes the filename in its constructor —
            // no separate Output_Filename property like the legacy SDK.
            core.Output_Format = new MP4Output(output);

            // Tells the engine it is running in a console host (no message
            // pump). Required so internal callbacks dispatch directly instead
            // of trying to marshal onto a non-existent UI synchronisation
            // context.
            core.ConsoleUsage = true;

            // Console hosts MUST await OnStop before disposing — Start() only
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
            core.Start();

            var ok = await done.Task;

            core.Stop();
            core.Dispose();
            VisioForgeX.DestroySDK();

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

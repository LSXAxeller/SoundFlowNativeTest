using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Providers;
using SoundFlow.Enums;

// The path to the audio file will be provided as a command-line argument.
if (args.Length == 0)
{
    Console.WriteLine("Error: Please provide the path to an audio file as a command-line argument.");
    return 1; // Return a non-zero exit code for error
}
var audioFilePath = args[0];

if (!File.Exists(audioFilePath))
{
    Console.WriteLine($"Error: Audio file not found at '{audioFilePath}'");
    return 1;
}

try
{
    Console.WriteLine($"Initializing audio engine...");
    using var audioEngine = new MiniAudioEngine(48000, Capability.Playback); 
    Console.WriteLine("Audio engine initialized successfully.");

    Console.WriteLine($"Loading audio file: {audioFilePath}");
    var player = new SoundPlayer(new StreamDataProvider(File.OpenRead(audioFilePath)));

    Mixer.Master.AddComponent(player);
    player.Play();
    Console.WriteLine("Playback started. Waiting for audio to finish...");

    while (player.State == PlaybackState.Playing)
    {
        Thread.Sleep(100); // Avoid busy-waiting
    }

    Console.WriteLine("Playback finished.");
    player.Stop();
    Mixer.Master.RemoveComponent(player);

    Console.WriteLine("Program finished successfully.");
    return 0; // Success
}
catch (Exception ex)
{
    Console.WriteLine($"An exception occurred: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    return 1; // Return a non-zero exit code to fail the workflow
}
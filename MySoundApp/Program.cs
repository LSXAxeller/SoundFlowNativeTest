using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Providers;
using SoundFlow.Enums;
using SoundFlow.Structs;

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
    Console.WriteLine("Initializing audio engine context...");
    using var audioEngine = new MiniAudioEngine();
    Console.WriteLine("Audio engine context initialized.");

    var audioFormat = new AudioFormat
    {
        SampleRate = 48000,
        Channels = 2,
        Format = SampleFormat.F32
    };

    Console.WriteLine("Initializing default playback device...");
    var defaultDevice = audioEngine.PlaybackDevices.FirstOrDefault();
    using var playbackDevice = audioEngine.InitializePlaybackDevice(defaultDevice, audioFormat);
    Console.WriteLine($"Device initialized: {playbackDevice.Info?.Name}");

    Console.WriteLine($"Loading audio file: {audioFilePath}");
    var dataProvider = new StreamDataProvider(audioEngine, audioFormat, File.OpenRead(audioFilePath));
    var player = new SoundPlayer(audioEngine, audioFormat, dataProvider);
    
    playbackDevice.MasterMixer.AddComponent(player);
    
    playbackDevice.Start();
    player.Play();
    
    Console.WriteLine("Playback started. Waiting for audio to finish...");
    while (player.State == PlaybackState.Playing)
    {
        Thread.Sleep(100); // Avoid busy-waiting
    }

    Console.WriteLine("Playback finished.");
    player.Stop();
    playbackDevice.MasterMixer.RemoveComponent(player);

    Console.WriteLine("Program finished successfully.");
    return 0; // Success
}
catch (Exception ex)
{
    Console.WriteLine($"An exception occurred: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    return 1; // Return a non-zero exit code to fail the workflow
}
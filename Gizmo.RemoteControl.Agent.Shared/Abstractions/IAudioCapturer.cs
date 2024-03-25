namespace Gizmo.RemoteControl.Agent.Shared.Abstractions;

public interface IAudioCapturer
{
    event EventHandler<byte[]> AudioSampleReady;
    void ToggleAudio(bool toggleOn);
}

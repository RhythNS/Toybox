/// <summary>
/// The project implentation is quite exploitative. Many actions can result in either the player being able
/// to move whilst the zombies cant or vice versa.
/// </summary>
public interface IPausable
{
    void Pause();

    void Resume();
}

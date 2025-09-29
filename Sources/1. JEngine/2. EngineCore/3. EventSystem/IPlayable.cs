
namespace J2y
{
    public interface IPlayable
    {
        void Play();
        void Pause();
        void Resume();
        void Stop();
        void Finish();
    }

    public interface ISoundPlayable : IPlayable
    {
        void FastForward();
        void Rewind();
    }

}

using R3;
using R3.Triggers;
using ObservableCollections;

namespace HikanyanLaboratory.Audio
{
    public interface ICriVolume
    {
        ReactiveProperty<float> Volume { get; }
        void SetVolume(float volume);
    }
}
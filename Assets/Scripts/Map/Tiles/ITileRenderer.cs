using UnityEngine;

namespace UFB.Map
{
    public interface ITileRenderer
    {
        float Height { get; }
        Coordinates Coordinates { get; }
        GameObject GameObject { get; }
        void SetHeight(float newHeight);
        void SetTint(Color color);
        void Reset();
    }
}

using Unity.VisualScripting;
using UnityEngine;

namespace UFB.Core
{
    public abstract class MonoBehaviourService : MonoBehaviour, IService
    {

        protected void Reset()
        {
            name = GetType().Name;
        }
    }
}
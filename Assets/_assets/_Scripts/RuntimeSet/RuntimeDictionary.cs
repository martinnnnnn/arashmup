using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arashmup
{
    public abstract class RuntimeDictionary<Key, T> : ScriptableObject
    {
        public Dictionary<Key, T> Items = new Dictionary<Key, T>();

        public void Add(Key key, T thing)
        {
            if (Items.ContainsKey(key))
            {
                Debug.LogError("Key already exists in the runtime dictionary");
            }
            else
            {
                Items[key] = thing;
            }
        }

        public void Remove(Key key)
        {
            if (Items.ContainsKey(key))
            {
                Items.Remove(key);
            }
                
        }
    }
}
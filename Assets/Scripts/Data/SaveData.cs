using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu]
public class SaveData : ScriptableObject
{
    [Serializable]
    public class KeyValuePairLists<T>
    {
        public List<string> keys = new List<string>();
        public List<T> values = new List<T>();

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public void TrySetValue(string key, T value)
        {
            int index = keys.FindIndex(x => x == key);
            if (index > -1)
            {
                values[index] = value;
            }
            else
            {
                keys.Add(key);
                values.Add(value);
            }
        }

        public bool TryGetValue(string key, ref T value)
        {
            int index = keys.FindIndex(x => x == key);
            if (index > -1)
            {
                value = values[index];
                return true;
            }
            return false;
        }
    }

    public KeyValuePairLists<bool> boolKeyValuePairLists = new KeyValuePairLists<bool>();
    public KeyValuePairLists<int> intKeyValuePairLists = new KeyValuePairLists<int>();
    public KeyValuePairLists<string> stringKeyValuePairLists = new KeyValuePairLists<string>();
    public KeyValuePairLists<Vector3> vector3KeyValuePairLists = new KeyValuePairLists<Vector3>();
    public KeyValuePairLists<Quaternion> quaternionKeyValuePairLists = new KeyValuePairLists<Quaternion>();

    public void reset()
    {
        boolKeyValuePairLists.Clear();
        intKeyValuePairLists.Clear();
        stringKeyValuePairLists.Clear();
        vector3KeyValuePairLists.Clear();
        quaternionKeyValuePairLists.Clear();
    }

    private void save<T>(KeyValuePairLists<T> lists, string key, T value)
    {
        lists.TrySetValue(key, value);
    }

    private bool load<T>(KeyValuePairLists<T> lists, string key, ref T value)
    {
        return lists.TryGetValue(key, ref value);
    }

    public void save(string key, bool value)
    {
        save(boolKeyValuePairLists, key, value);
    }

    public void save(string key, int value)
    {
        save(intKeyValuePairLists, key, value);
    }

    public void save(string key, string value)
    {
        save(stringKeyValuePairLists, key, value);
    }

    public void save(string key, Vector3 value)
    {
        save(vector3KeyValuePairLists, key, value);
    }

    public void save(string key, Quaternion value)
    {
        save(quaternionKeyValuePairLists, key, value);
    }

    public bool load(string key, ref bool value)
    {
        return load(boolKeyValuePairLists, key, ref value);
    }

    public bool load(string key, ref int value)
    {
        return load(intKeyValuePairLists, key, ref value);
    }

    public bool load(string key, ref string value)
    {
        return load(stringKeyValuePairLists, key, ref value);
    }

    public bool load(string key, ref Vector3 value)
    {
        return load(vector3KeyValuePairLists, key, ref value);
    }

    public bool load(string key, ref Quaternion value)
    {
        return load(quaternionKeyValuePairLists, key, ref value);
    }
}

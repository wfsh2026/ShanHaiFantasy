using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class HashList<T> : IEnumerable<T> {
    private readonly List<T> list;
    private readonly Dictionary<Type, int> dict;

    public int Count {
        get {
            return list.Count;
        }
    }

    public HashList() : this(4) {
    }

    public HashList(int capacity) {
        list = new List<T>(capacity);
        dict = new Dictionary<Type, int>(capacity);
    }

    public T this[int index] {
        get {
            if (index < 0 || index >= list.Count) {
                Debug.LogError("HashList index is out of range.");
                return default(T);
            }

            return list[index];
        }
    }

    public int Find(Type type) {
        if (type == null) {
            Debug.LogError("HashList type is null.");
            return -1;
        }

        if (dict.TryGetValue(type, out int index)) {
            return index;
        }

        for (int i = 0; i < list.Count; ++i) {
            Type itemType = list[i].GetType();
            if (itemType == type || itemType.IsSubclassOf(type)) {
                return i;
            }
        }

        return -1;
    }

    public int Find(string name) {
        if (string.IsNullOrEmpty(name)) {
            Debug.LogError("HashList name is invalid.");
            return -1;
        }

        for (int i = 0; i < list.Count; ++i) {
            if (list[i].GetType().Name == name) {
                return i;
            }
        }

        return -1;
    }

    public bool TryGetValue(Type type, out T value) {
        int index = Find(type);
        if (index < 0) {
            value = default(T);
            return false;
        }

        value = list[index];
        return true;
    }

    public bool TryGetValue(string name, out T value) {
        int index = Find(name);
        if (index < 0) {
            value = default(T);
            return false;
        }

        value = list[index];
        return true;
    }

    public void Add(T value) {
        if (ReferenceEquals(value, null)) {
            Debug.LogError("HashList add value is null.");
            return;
        }

        if (Find(value.GetType()) >= 0) {
            Debug.LogError("HashList add value already exists.");
            return;
        }

        list.Add(value);
        dict[value.GetType()] = list.Count - 1;
    }

    public void Remove(Type type) {
        int index = Find(type);
        if (index < 0) {
            Debug.LogError("HashList remove value is not found.");
            return;
        }

        list.RemoveAt(index);
        dict.Clear();
        for (int i = 0; i < list.Count; ++i) {
            dict[list[i].GetType()] = i;
        }
    }

    public void Remove(T value) {
        if (ReferenceEquals(value, null)) {
            Debug.LogError("HashList remove value is null.");
            return;
        }

        Remove(value.GetType());
    }

    public void Clear() {
        list.Clear();
        dict.Clear();
    }

    public IEnumerator<T> GetEnumerator() {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

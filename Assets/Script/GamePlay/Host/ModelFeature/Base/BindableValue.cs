using System;
using System.Collections.Generic;

public sealed class BindableValue<T> {
    private event Action<T> ValueChanged;
    private T value;

    public BindableValue() {
        value = default(T);
    }

    public BindableValue(T defaultValue) {
        value = defaultValue;
    }

    public T Value {
        get {
            return value;
        }
    }

    public void SetValue(T newValue) {
        if (EqualityComparer<T>.Default.Equals(value, newValue)) {
            return;
        }

        value = newValue;
        NotifyValueChanged();
    }

    public void Bind(Action<T> listener, bool invokeImmediately = true) {
        if (listener == null) {
            return;
        }

        ValueChanged -= listener;
        ValueChanged += listener;

        if (invokeImmediately) {
            listener.Invoke(value);
        }
    }

    public void Unbind(Action<T> listener) {
        if (listener == null) {
            return;
        }

        ValueChanged -= listener;
    }

    public void ClearListeners() {
        ValueChanged = null;
    }

    private void NotifyValueChanged() {
        if (ValueChanged != null) {
            ValueChanged.Invoke(value);
        }
    }
}

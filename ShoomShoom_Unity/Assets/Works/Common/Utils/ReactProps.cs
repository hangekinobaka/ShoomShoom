using System;
using System.Collections.Generic;
using UniRx;

public class ReactProps<T>
{
    private readonly ReactiveProperty<T> _state = new ReactiveProperty<T>();

    public ReactProps() { }
    public ReactProps(T value) => SetState(value);

    public IObservable<T> State => _state;
    public void SetState(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_state.Value, value)) return;
        _state.Value = value;
    }
    public T GetState()
    {
        return _state.Value;
    }

}

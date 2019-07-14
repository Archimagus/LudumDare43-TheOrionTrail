using System;
using UnityEngine;

#pragma warning disable 0649
[System.Serializable]
public abstract class TVariable : ScriptableObject
{
	public abstract void Reset();
}
public abstract class TVariable<T> : TVariable
{
	[SerializeField]
	protected T Value;
	private T _currentValue;
	public event EventHandler<ReferenceChangedEventHandler<T>> Changed;
	public T CurrentValue
	{
		get
		{
			return _currentValue;
		}
		set
		{
			if (value != null && value.Equals(_currentValue))
				return;
			SetValue(value);
		}
	}
	private void Awake()
	{
		Reset();
	}
	void OnValidate()
	{
		Reset();
	}
	public override void Reset()
	{
		CurrentValue = Value;
	}
	public static implicit operator T(TVariable<T> i) { return i.CurrentValue; }

	protected virtual void SetValue(T val)
	{
		var oldValue = _currentValue;
		_currentValue = val;
		Changed?.Invoke(this, new ReferenceChangedEventHandler<T>(oldValue, _currentValue));
	}
}
[System.Serializable]
public abstract class VarReference
{

}
[System.Serializable]
public abstract class TReference<T, V> : VarReference where V : TVariable<T>
{
	[SerializeField] private bool UseConstant = true;
	[SerializeField] private T ConstantValue;
	[SerializeField] private V Variable;
	private bool _changeRegistered;
	event Action<ReferenceChangedEventHandler<T>> _changed;
	public event Action<ReferenceChangedEventHandler<T>> Changed
	{
		add
		{
			_changed += value;
			if (_changeRegistered == false && UseConstant == false)
			{
				_changeRegistered = true;
				Variable.Changed += (s, e) => _changed?.Invoke(e);
			}
		}
		remove
		{
			_changed -= value;
		}
	}

	public TReference(T initial)
	{
		ConstantValue = initial;
	}
	public T Value
	{
		get
		{
			validate();
			return (UseConstant || Variable == null) ? ConstantValue : Variable.CurrentValue;
		}
		set
		{
			validate();
			if (value.Equals(Value))
				return;

			var oldValue = Value;
			if (UseConstant)
				ConstantValue = value;
			else
				Variable.CurrentValue = value;

			_changed?.Invoke(new ReferenceChangedEventHandler<T>(oldValue, Value));
		}
	}

	private void validate()
	{
		if (UseConstant == false)
		{
			if (Variable == null)
			{
				Debug.LogWarning("No Variable registered");
				return;
			}
		}
	}
	public static implicit operator T(TReference<T, V> i) { return i.Value; }
}

public class ReferenceChangedEventHandler<T> : EventArgs
{
	public T OldValue;
	public T NewValue;
	internal ReferenceChangedEventHandler(T oldValue, T newValue)
	{
		OldValue = oldValue;
		NewValue = newValue;
	}
}
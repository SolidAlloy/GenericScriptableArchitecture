namespace GenericScriptableArchitecture
{
    using System;
    using System.IO;
    using UnityEngine;

    [Serializable]
    internal class EventHolder<T>
    {
        [SerializeField] internal ScriptableEvent<T> _event;
        [SerializeField] internal Variable<T> _variable;
        [SerializeField] internal VariableInstancer<T> _variableInstancer;
        [SerializeField] internal EventInstancer<T> _eventInstancer;
        [SerializeField] internal EventHolder.EventType _type = EventHolder.EventType.ScriptableEvent;
        [SerializeField] internal bool _notifyCurrentValue;

        public void AddListener(ScriptableEventListener<T> listener)
        {
            switch (_type)
            {
                case EventHolder.EventType.ScriptableEvent:
                    _event?.AddListener(listener);
                    break;

                case EventHolder.EventType.Variable:
                    _variable?.AddListener(listener, _notifyCurrentValue);
                    break;

                case EventHolder.EventType.VariableInstancer:
                    _variableInstancer?.AddListener(listener, _notifyCurrentValue);
                    break;

                case EventHolder.EventType.EventInstancer:
                    _eventInstancer?.AddListener(listener);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener), "Unknown enum value when adding listener to the event holder.");
            }
        }

        public void RemoveListener(ScriptableEventListener<T> listener)
        {
            switch (_type)
            {
                case EventHolder.EventType.ScriptableEvent:
                    _event?.RemoveListener(listener);
                    break;

                case EventHolder.EventType.Variable:
                    _variable?.RemoveListener(listener);
                    break;

                case EventHolder.EventType.VariableInstancer:
                    _variableInstancer?.RemoveListener(listener);
                    break;

                case EventHolder.EventType.EventInstancer:
                    _variableInstancer?.RemoveListener(listener);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener), "Unknown enum value when adding listener to the event holder.");
            }
        }
    }
}
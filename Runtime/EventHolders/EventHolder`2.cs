namespace GenericScriptableArchitecture
{
    using System;
    using System.IO;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    internal class EventHolder<T1, T2>
    {
        [SerializeField] private ScriptableEvent<T1, T2> _event;
        [SerializeField] private VariableWithHistory<T1> _variable;
        [SerializeField] private VariableInstancerWithHistory<T1> _variableInstancer;
        [SerializeField] private EventInstancer<T1, T2> _eventInstancer;

        [SerializeField] private EventType _type = EventType.ScriptableEvent;
        [SerializeField] private bool _notifyCurrentValue;

        public void AddListener(ScriptableEventListener<T1, T2> listener)
        {
            switch (_type)
            {
                case EventType.ScriptableEvent:
                    _event?.AddListener(listener);
                    break;

                case EventType.Variable:
                    _variable?.AddListener(listener as ScriptableEventListener<T1, T1>, _notifyCurrentValue);
                    break;

                case EventType.VariableInstancer:
                    _variableInstancer?.AddListener(listener as ScriptableEventListener<T1, T1>, _notifyCurrentValue);
                    break;

                case EventType.EventInstancer:
                    _eventInstancer?.AddListener(listener);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener), "Unknown enum value when adding listener to the event holder.");
            }
        }

        public void RemoveListener(ScriptableEventListener<T1, T2> listener)
        {
            switch (_type)
            {
                case EventType.ScriptableEvent:
                    _event?.RemoveListener(listener);
                    break;

                case EventType.Variable:
                    _variable?.RemoveListener(listener as ScriptableEventListener<T1, T1>);
                    break;

                case EventType.VariableInstancer:
                    _variableInstancer?.RemoveListener(listener as ScriptableEventListener<T1, T1>);
                    break;

                case EventType.EventInstancer:
                    _eventInstancer?.RemoveListener(listener);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener), "Unknown enum value when adding listener to the event holder.");
            }
        }
    }
}
namespace GenericScriptableArchitecture
{
    using System;
    using System.IO;
    using UnityEngine;
    using Object = UnityEngine.Object;

    internal class EventHolderBaseTwo { }

    [Serializable]
    internal class EventHolder<T1, T2> : EventHolderBaseTwo
    {
        [SerializeField] private ScriptableEvent<T1, T2> _event;
        [SerializeField] private VariableWithHistory<T1> _variable;
        [SerializeField] private VariableInstancerWithHistory<T1> _variableInstancer;

        [SerializeField] private EventTypes _type = EventTypes.ScriptableEvent;
        [SerializeField] private bool _notifyCurrentValue;

        public bool DrawObjectField = true;

        public IBaseEvent Event
        {
            get
            {
                return _type switch
                {
                    EventTypes.ScriptableEvent => _event,
                    EventTypes.Variable => _variable,
                    EventTypes.VariableInstancer => _variableInstancer,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            set
            {
                if (value is ScriptableEvent<T1, T2> @event)
                {
                    _type = EventTypes.ScriptableEvent;
                    _event = @event;
                }
                else if (value is VariableWithHistory<T1> variable)
                {
                    _type = EventTypes.Variable;
                    _variable = variable;
                }
                else if (value is VariableInstancerWithHistory<T1> variableInstancer)
                {
                    _type = EventTypes.VariableInstancer;
                    _variableInstancer = variableInstancer;
                }
                else
                {
                    throw new InvalidDataException($"Tried to set an event that is neither of EventTypes: {value.GetType()}");
                }
            }
        }

        public void AddListener(ScriptableEventListener<T1, T2> listener)
        {
            switch (_type)
            {
                case EventTypes.ScriptableEvent:
                    _event?.AddListener(listener);
                    break;

                case EventTypes.Variable:
                    _variable?.AddListener(listener as ScriptableEventListener<T1, T1>, _notifyCurrentValue);
                    break;

                case EventTypes.VariableInstancer:
                    _variableInstancer?.AddListener(listener as ScriptableEventListener<T1, T1>, _notifyCurrentValue);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener), "Unknown enum value when adding listener to the event holder.");
            }
        }

        public void RemoveListener(ScriptableEventListener<T1, T2> listener)
        {
            switch (_type)
            {
                case EventTypes.ScriptableEvent:
                    _event?.RemoveListener(listener);
                    break;

                case EventTypes.Variable:
                    _variable?.RemoveListener(listener as ScriptableEventListener<T1, T1>);
                    break;

                case EventTypes.VariableInstancer:
                    _variableInstancer?.RemoveListener(listener as ScriptableEventListener<T1, T1>);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener), "Unknown enum value when adding listener to the event holder.");
            }
        }
    }
}
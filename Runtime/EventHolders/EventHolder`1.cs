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
        [SerializeField] internal EventType _type = EventType.ScriptableEvent;
        [SerializeField] internal bool _notifyCurrentValue;

        public IBaseEvent Event
        {
            get
            {
                return _type switch
                    {
                        EventType.ScriptableEvent => _event,
                        EventType.Variable => _variable,
                        EventType.VariableInstancer => _variableInstancer,
                        EventType.EventInstancer => _eventInstancer,
                        _ => throw new ArgumentOutOfRangeException()
                    };
            }
            set
            {
                if (value is ScriptableEvent<T> @event)
                {
                    _type = EventType.ScriptableEvent;
                    _event = @event;
                }
                else if (value is Variable<T> variable)
                {
                    _type = EventType.Variable;
                    _variable = variable;
                }
                else if (value is VariableInstancer<T> variableInstancer)
                {
                    _type = EventType.VariableInstancer;
                    _variableInstancer = variableInstancer;
                }
                else if (value is EventInstancer<T> eventInstancer)
                {
                    _type = EventType.EventInstancer;
                    _eventInstancer = eventInstancer;
                }
                else
                {
                    throw new InvalidDataException($"Tried to set the event that wasn't either ScriptableEvent or Variable: {value.GetType()}");
                }
            }
        }

        public void AddListener(ScriptableEventListener<T> listener)
        {
            switch (_type)
            {
                case EventType.ScriptableEvent:
                    _event?.AddListener(listener);
                    break;

                case EventType.Variable:
                    _variable?.AddListener(listener, _notifyCurrentValue);
                    break;

                case EventType.VariableInstancer:
                    _variableInstancer?.AddListener(listener, _notifyCurrentValue);
                    break;

                case EventType.EventInstancer:
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
                case EventType.ScriptableEvent:
                    _event?.RemoveListener(listener);
                    break;

                case EventType.Variable:
                    _variable?.RemoveListener(listener);
                    break;

                case EventType.VariableInstancer:
                    _variableInstancer?.RemoveListener(listener);
                    break;

                case EventType.EventInstancer:
                    _variableInstancer?.RemoveListener(listener);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener), "Unknown enum value when adding listener to the event holder.");
            }
        }
    }
}
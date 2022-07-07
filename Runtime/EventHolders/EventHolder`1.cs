namespace GenericScriptableArchitecture
{
    using System;
    using System.IO;
    using UnityEngine;
    using Object = UnityEngine.Object;

    internal class EventHolderBaseOne { }

    [Serializable]
    internal class EventHolder<T> : EventHolderBaseOne
    {
        [SerializeField] internal ScriptableEvent<T> _event;
        [SerializeField] internal Variable<T> _variable;
        [SerializeField] internal VariableInstancer<T> _variableInstancer;
        [SerializeField] internal EventTypes _type = EventTypes.ScriptableEvent;
        [SerializeField] internal bool _notifyCurrentValue;

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
                if (value is ScriptableEvent<T> @event)
                {
                    _type = EventTypes.ScriptableEvent;
                    _event = @event;
                }
                else if (value is Variable<T> variable)
                {
                    _type = EventTypes.Variable;
                    _variable = variable;
                }
                else if (value is VariableInstancer<T> variableInstancer)
                {
                    _type = EventTypes.VariableInstancer;
                    _variableInstancer = variableInstancer;
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
                case EventTypes.ScriptableEvent:
                    _event?.AddListener(listener);
                    break;

                case EventTypes.Variable:
                    _variable?.AddListener(listener, _notifyCurrentValue);
                    break;

                case EventTypes.VariableInstancer:
                    _variableInstancer?.AddListener(listener, _notifyCurrentValue);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener), "Unknown enum value when adding listener to the event holder.");
            }
        }

        public void RemoveListener(ScriptableEventListener<T> listener)
        {
            switch (_type)
            {
                case EventTypes.ScriptableEvent:
                    _event?.RemoveListener(listener);
                    break;

                case EventTypes.Variable:
                    _variable?.RemoveListener(listener);
                    break;

                case EventTypes.VariableInstancer:
                    _variableInstancer?.RemoveListener(listener);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener), "Unknown enum value when adding listener to the event holder.");
            }
        }
    }
}
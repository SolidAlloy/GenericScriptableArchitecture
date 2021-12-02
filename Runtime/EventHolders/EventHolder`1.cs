namespace GenericScriptableArchitecture
{
    using System;
    using System.IO;
    using UnityEngine;

    internal class EventHolderBaseOne { }

    [Serializable]
    internal class EventHolder<T> : EventHolderBaseOne
    {
        [SerializeField] private ScriptableEvent<T> _event;
        [SerializeField] private Variable<T> _variable;
        [SerializeField] private EventTypes _type = EventTypes.ScriptableEvent;
        [SerializeField] private bool _notifyCurrentValue;

        public bool DrawObjectField = true;

        public BaseEvent Event
        {
            get => _type == EventTypes.ScriptableEvent ? _event : _variable;
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

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener),
                        "Unknown enum value when adding listener to the event holder.");
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

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener),
                        "Unknown enum value when adding listener to the event holder.");
            }
        }
    }
}
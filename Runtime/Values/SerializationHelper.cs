namespace GenericScriptableArchitecture
{
    using ExtEvents.OdinSerializer;

    public static class SerializationHelper
    {
        public static T CreateCopy<T>(T value)
        {
            CustomSerialization.SerializeValueToBinary(value, out var bytes, out var referencedUnityObjects);
            return CustomSerialization.DeserializeValue<T>(bytes, referencedUnityObjects);
        }
    }
}
namespace Quark.Business.Common.Tools
{
    public static class RequestMapper
    {
        public static T Map<T>(T source, T destination)
        {
            var type = typeof(T);
            foreach (var property in type.GetProperties())
            {
                if (property.CanRead && property.CanWrite)
                {
                    var destinationValue = property.GetValue(destination);
                    var defaultValue = GetDefault(property.PropertyType);

                    if (destinationValue == null ||
                        destinationValue.Equals(defaultValue) ||
                        (property.PropertyType == typeof(string) && string.IsNullOrEmpty(destinationValue as string)))
                    {
                        var sourceValue = property.GetValue(source);
                        property.SetValue(destination, sourceValue);
                    }
                }
            }
            return destination;
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}

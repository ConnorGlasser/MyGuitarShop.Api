namespace MyGuitarShop.API.Mappers
{
    public static class AutoReflectionMapper
    {
        public static TTarget? Map<TSource, TTarget> (TSource? source)
            where TTarget : new()
        {
            if (source == null)
            {
                return default;
            }

            var target = new TTarget();
            var sourceProps = typeof(TSource).GetProperties();
            var targetProps = typeof(TTarget).GetProperties();

            // for each property in all of the properties
            foreach(var sourceProp in sourceProps)
            {
                // look in each of the targets to find one with the same name and type
                var targetProp = targetProps.FirstOrDefault(p => p.Name == sourceProp.Name &&
                                                                 p.PropertyType == sourceProp.PropertyType &&
                                                                 p.CanWrite);

                // if it isn't found it will be null. 
                if (targetProp == null) continue;
                // Otherwise...

                // get a value from the property
                var value = sourceProp.GetValue(source);
                // and set the value in the target to that avalue
                targetProp.SetValue(target, value);
            }

            return target;
        }
    }
}

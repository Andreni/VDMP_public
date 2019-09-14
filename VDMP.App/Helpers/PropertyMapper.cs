using System.Linq;

namespace VDMP.App.Helpers
{
    internal class PropertyMapper
    {
        // Found on stackoverflow: by Yargicx
        public static void CopyPropertiesTo<T, TU>(T source, TU dest)
        {
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            var destProps = typeof(TU).GetProperties()
                .Where(x => x.CanWrite)
                .ToList();

            foreach (var sourceProp in sourceProps)
                if (destProps.Any(x => x.Name == sourceProp.Name))
                {
                    var p = destProps.First(x => x.Name == sourceProp.Name);
                    if (p.CanWrite) p.SetValue(dest, sourceProp.GetValue(source, null), null);
                }
        }
    }
}
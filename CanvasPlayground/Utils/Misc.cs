using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CanvasPlayground.Physics.Scenes;

namespace CanvasPlayground.Utils
{
    public class Misc
    {
        public static Type[] GetTypes()
        {
            Type[] types;
            try
            {
                types = Assembly.GetExecutingAssembly().GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }
            return types;
        }

        public static Type GetTypesByName(string name)
        {
            return GetTypes().FirstOrDefault(o => typeof(IScene).IsAssignableFrom(o) && o.Name.EndsWith(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}

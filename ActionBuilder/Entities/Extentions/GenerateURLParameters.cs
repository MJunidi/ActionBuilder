using System.Reflection;

namespace ActionBuilder.Entities.Extentions
{
    public static partial class Extension
    {
        public static string GenerateUrlParameter(this object REQ)
        {
            string RES = "";
            PropertyInfo[] Props = REQ.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                if (prop.GetValue(REQ, null) != null)
                {
                    RES += $"/{prop.GetValue(REQ, null)}";
                }
            }

            return RES;
        }
    }
}

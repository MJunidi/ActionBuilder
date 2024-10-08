using System.Reflection;

namespace ActionBuilder.Entities.Extentions
{
    public static partial class Extension
    {
        public static string GenerateQueryString(this object REQ)
        {
            string RES = "";
            PropertyInfo[] Props = REQ.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                if (prop.GetValue(REQ, null) != null)
                {
                    RES += prop.Name + "=" + prop.GetValue(REQ, null) + "&";
                }
            }

            if (!string.IsNullOrEmpty(RES))
            {
                RES = RES.Remove(RES.Length - 1);
            }

            return RES;
        }
    }
}

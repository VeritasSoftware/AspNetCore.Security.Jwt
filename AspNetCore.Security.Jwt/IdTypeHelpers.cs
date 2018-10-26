using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;

namespace AspNetCore.Security.Jwt
{
    public static class IdTypeHelpers
    {
        private static Dictionary<string, string> claimTypes = null;

        /// <summary>
        /// Loads the Claim Types into a dictionary reflectively. Called in AddSecurity extension only once on start up.
        /// </summary>
        public static void LoadClaimTypes()
        {
            claimTypes = new Dictionary<string, string>();

            Type t = typeof(ClaimTypes);
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (FieldInfo fi in fields)
            {
                claimTypes.Add(fi.Name, fi.GetValue(null).ToString());
            }
        }

        /// <summary>
        /// To Claim Types extensions. Gets the ClaimTypes for a specified IdType
        /// </summary>
        /// <param name="idType"></param>
        /// <returns></returns>
        public static string ToClaimTypes(this IdType idType)
        {
            if (claimTypes == null)
            {
                LoadClaimTypes();
            }

            return claimTypes[idType.ToString()];
        }
    }
}

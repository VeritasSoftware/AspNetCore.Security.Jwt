using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace AspNetCore.Security.Jwt
{
    internal static class IdTypeHelpers
    {
        internal static readonly Dictionary<string, string> claimTypes = new Dictionary<string, string>();        

        /// <summary>
        /// Loads the Claim Types into a dictionary reflectively. Called in AddSecurity extension only once on start up.
        /// </summary>
        internal static void LoadClaimTypes()
        {
            if (claimTypes != null && claimTypes.Any())
            {
                return;
            }

            Type t = typeof(ClaimTypes);
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (FieldInfo fi in fields)
            {
                claimTypes.Add(fi.Name, fi.GetValue(null).ToString());
            }
        }        
    }
}

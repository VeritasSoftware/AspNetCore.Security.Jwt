using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace AspNetCore.Security.Jwt
{
    internal static class IdTypeHelpers
    {
        internal static readonly Dictionary<string, string> claimTypesDictionary = new Dictionary<string, string>();

        internal static Dictionary<string, string> ClaimTypes => claimTypesDictionary;

        /// <summary>
        /// Loads the Claim Types into a dictionary reflectively. Called in AddSecurity extension only once on start up.
        /// </summary>
        internal static void LoadClaimTypes()
        {
            if (ClaimTypes != null && ClaimTypes.Any())
            {
                return;
            }

            Type t = typeof(ClaimTypes);
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (FieldInfo fi in fields)
            {
                ClaimTypes.Add(fi.Name, fi.GetValue(null).ToString());
            }
        }        
    }
}

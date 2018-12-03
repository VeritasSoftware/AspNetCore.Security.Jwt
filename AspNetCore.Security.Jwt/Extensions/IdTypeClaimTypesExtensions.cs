using System.Linq;

namespace AspNetCore.Security.Jwt
{
    public static class IdTypeClaimTypesExtensions
    {
        /// <summary>
        /// To Claim Types extensions. Gets the ClaimTypes for a specified IdType
        /// </summary>
        /// <param name="idType"></param>
        /// <returns>ClaimTypes</returns>
        public static string ToClaimTypes(this IdType idType)
        {
            var claimTypes = IdTypeHelpers.ClaimTypes;

            if (claimTypes == null || !claimTypes.Any())
            {
                IdTypeHelpers.LoadClaimTypes();

                claimTypes = IdTypeHelpers.ClaimTypes;
            }

            return claimTypes[idType.ToString()];
        }
    }
}

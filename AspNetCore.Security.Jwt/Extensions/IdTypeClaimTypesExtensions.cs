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
            var claimTypes = IdTypeHelpers.claimTypes;

            if (claimTypes == null)
            {
                IdTypeHelpers.LoadClaimTypes();

                claimTypes = IdTypeHelpers.claimTypes;
            }

            return claimTypes[idType.ToString()];
        }
    }
}

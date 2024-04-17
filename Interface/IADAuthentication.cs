using ActiveDirectoryLibrary.Model;

namespace ActiveDirectoryLibrary.Interface
{
    public interface IADAuthentication
    {
        /// <summary>
        /// LDAP Address
        /// </summary>
        string? LdapAddress { get; set; }

        /// <summary>
        /// LDAP Password
        /// </summary>
        string? LdapPassword { get; set; }

        /// <summary>
        /// LDAP Username
        /// </summary>
        string? LdapUsername { get; set; }

        /// <summary>
        /// Checks team members login credentials against the Active Directory Domain Services.
        /// </summary>
        /// <param name="model">Login model</param>
        /// <returns>True if valid , otherwise false if search results yield no results</returns>
        bool LDAPLogin(LoginViewModel model);

        /// <summary>
        ///  Determines if the team members details are valid against the Active Directory Domain Services.
        /// </summary>
        /// <param name="cn">Common Name</param>
        /// <returns>an object of type RegisterViewModel</returns>
        RegisterViewModel ValidateTeamMemberDetails(string cn);
    }
}


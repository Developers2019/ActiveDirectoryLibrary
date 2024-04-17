using ActiveDirectoryLibrary.Interface;
using ActiveDirectoryLibrary.Model;
using System.DirectoryServices;

namespace ActiveDirectoryLibrary.Service
{
    public class ADAuthentication : IADAuthentication
    {
        public string? LdapAddress { get; set; }
        public string? LdapUsername { get; set; }
        public string? LdapPassword { get; set; }

        public ADAuthentication()
        {
        }

        public ADAuthentication(
            string ldapAddress, 
            string ldapUsername, 
            string ldapPassword)
        {
            LdapAddress = ldapAddress;
            LdapUsername = ldapUsername;
            LdapPassword = ldapPassword;
        }

        /// <summary>
        ///  Determines if the team members details are valid against the Active Directory Domain Services.
        /// </summary>
        /// <param name="cn">Common Name</param>
        /// <returns>an object of type RegisterViewModel</returns>
        public RegisterViewModel ValidateTeamMemberDetails(string cn)
        {
            try
            {
                string username = cn.Trim();

                DirectoryEntry de = new(LdapAddress, LdapUsername, LdapPassword);

                DirectorySearcher ds = new(de);

                RegisterViewModel usernameCheck = username.Trim().IndexOf('@') <= 0
                    ? GetTeamMemberInformation(ds, username)
                    : new RegisterViewModel();

                return usernameCheck;
            }
            catch (Exception)
            {
                return new RegisterViewModel();
            }
        }

        /// <summary>
        /// Gets the team member's information from the Active Directory Domain Services
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns>an object of type RegisterViewModel</returns>
        private static RegisterViewModel GetTeamMemberInformation(
            DirectorySearcher ds, 
            string username)
        {
            try
            {
                DirectorySearcher search = Filter(ds, username);

                SearchResult searchResult = search.FindOne();

                if (searchResult == null)
                {
                    return new RegisterViewModel 
                    {
                        ErrorMessage = "No record found", 
                        Error = true 
                    };

                }

                RegisterViewModel user = SetUserDetails(searchResult);
        
                return user;
            }
            catch (Exception)
            {
                return new RegisterViewModel 
                { 
                    ErrorMessage = "No record found", 
                    Error = true 
                };
            }
        }

        /// <summary>
        /// Checks team members login credentials against the Active Directory Domain Services.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>True if valid , otherwise false if search results yield no results</returns>
        public bool LDAPLogin(LoginViewModel model)
        {
            try
            {
                string username = model.Email.Trim().ToLower();
                DirectoryEntry directoryEntry;

#if DEBUG
                string[] det = model.Password.Split('|');

                directoryEntry = model.Password.Contains('|')
                                      ? new DirectoryEntry(LdapAddress, det[0], det[1])
                                      : new DirectoryEntry(LdapAddress, username, model.Password);
#else
                directoryEntry = new DirectoryEntry(LdapAddress, username, model.Password);

#endif
                //Performs queries against Active Directory Domain Services
                DirectorySearcher ds = new(directoryEntry);

                bool usernameCheck = model.Username.Trim().IndexOf('@') <= 0 && (FilterByUserName(ds, username));

                return usernameCheck;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Filters the Active Directory Domain Services by username
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns>True if valid , otherwise false if search results yield no results.</returns>
        private static bool FilterByUserName(
            DirectorySearcher ds, 
            string username)
        {
            try
            {
                //Performs queries against Active Directory Domain Services
                DirectorySearcher search = Filter(ds, username);

                SearchResult? searchResult = search.FindOne();

                bool isValid = searchResult != null;

                return isValid;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Filter applied against the Active Directory Domain Services
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="username"></param>
        /// <returns>an object of type DirectorySearcher</returns>
        private static DirectorySearcher Filter(
            DirectorySearcher ds, 
            string username)
        {
            ds.Filter = $"(&((&(objectCategory=Person)(objectClass=User)))(samaccountname={username}))";
            ds.SearchScope = SearchScope.Subtree;
            ds.ServerTimeLimit = TimeSpan.FromSeconds(90);
            return ds;
        }

        private static RegisterViewModel SetUserDetails(SearchResult searchResult)
        {

            string? name = searchResult.GetDirectoryEntry().Properties["GivenName"].Value?.ToString() ?? "";

            string? surname = searchResult.GetDirectoryEntry().Properties["SN"].Value?.ToString()
                           ?? searchResult.GetDirectoryEntry().Properties["cn"].Value!.ToString()!.Split(' ')[1];

            string? email = searchResult.GetDirectoryEntry().Properties["mail"].Value?.ToString() ?? "";
                   

            string department = searchResult.GetDirectoryEntry().Properties["department"].Value?.ToString() ?? "";

            RegisterViewModel user = new()
            {
                FullName = name.ToString(),
                LastName = surname.ToString(),
                Email = email.ToString(),
                Department = department.ToString(),
            };

            return user;


        }
    }
}
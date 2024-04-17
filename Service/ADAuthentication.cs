using ActiveDirectoryLibrary.Interface;
using ActiveDirectoryLibrary.Model;
using System.DirectoryServices;

namespace ActiveDirectoryLibrary.Service
{
    using System;
    using System.DirectoryServices;

    public class ADAuthentication : IADAuthentication
    {
        public string? LdapAddress { get; set; }
        public string? LdapUsername { get; set; }
        public string? LdapPassword { get; set; }

        public ADAuthentication()
        {
        }

        public ADAuthentication(string ldapAddress, string ldapUsername, string ldapPassword)
        {
            LdapAddress = ldapAddress;
            LdapUsername = ldapUsername;
            LdapPassword = ldapPassword;
        }

        public RegisterViewModel ValidateTeamMemberDetails(string cn)
        {
            try
            {
                string username = cn.Trim();

                DirectoryEntry de = new(LdapAddress, LdapUsername, LdapPassword);

                DirectorySearcher ds = new(de);

                RegisterViewModel usernameCheck = username.IndexOf('@') <= 0
                    ? GetTeamMemberInformation(ds, username)
                    : new RegisterViewModel();

                return usernameCheck;
            }
            catch (Exception)
            {
                return new RegisterViewModel();
            }
        }

        private RegisterViewModel GetTeamMemberInformation(DirectorySearcher ds, string username)
        {
            try
            {
                DirectorySearcher search = Filter(ds, username);

                SearchResult? searchResult = search.FindOne();

                if (searchResult == null)
                {
                    return new RegisterViewModel
                    {
                        ErrorMessage = "No record found",
                        Error = true
                    };
                }

                return SetUserDetails(searchResult);
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

        public bool LDAPLogin(LoginViewModel model)
        {
            try
            {
                string username = model.Email.Trim().ToLower();
                var directoryEntry = new DirectoryEntry(LdapAddress, username, model.Password);

                DirectorySearcher ds = new(directoryEntry);

                return username.IndexOf('@') <= 0 && FilterByUserName(ds, username);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool FilterByUserName(DirectorySearcher ds, string username)
        {
            try
            {
                DirectorySearcher search = Filter(ds, username);

                SearchResult? searchResult = search.FindOne();

                return searchResult != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static DirectorySearcher Filter(DirectorySearcher ds, string username)
        {
            ds.Filter = $"(&((&(objectCategory=Person)(objectClass=User)))(samaccountname={username}))";
            ds.SearchScope = SearchScope.Subtree;
            ds.ServerTimeLimit = TimeSpan.FromSeconds(90);
            return ds;
        }

        private RegisterViewModel SetUserDetails(SearchResult searchResult)
        {
            string name = searchResult.GetDirectoryEntry().Properties["GivenName"].Value?.ToString() ?? "";
            string surname = searchResult.GetDirectoryEntry().Properties["SN"].Value?.ToString()
                            ?? searchResult.GetDirectoryEntry().Properties["cn"].Value!.ToString()!.Split(' ')[1];
            string email = searchResult.GetDirectoryEntry().Properties["mail"].Value?.ToString() ?? "";
            string department = searchResult.GetDirectoryEntry().Properties["department"].Value?.ToString() ?? "";

            return new RegisterViewModel
            {
                FullName = name,
                LastName = surname,
                Email = email,
                Department = department
            };
        }
    }

}
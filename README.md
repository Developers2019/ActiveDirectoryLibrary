# ActiveDirectoryLibrary NuGet Package

## Overview

The ADAuthentication NuGet package provides functionality for validating team member details and authenticating against Active Directory Domain Services. 
It includes a class `ADAuthentication` that implements the `IADAuthentication` interface.

## Installation

You can install the ActiveDirectoryLibrary NuGet package via NuGet Package Manager or Package Manager Console:

```bash
PM> Install-Package ActiveDirectoryLibrary.04.17.24.001
```

## Usage

### Creating an Instance

You can create an instance of `ADAuthentication` using the LDAP credentials during instantiation.

var adAuth = new ADAuthentication("ldapAddress", "ldapUsername", "ldapPassword");
```

### Validating Team Member Details

To validate team member details against Active Directory, use the `ValidateTeamMemberDetails` method.

```csharp
var result = adAuth.ValidateTeamMemberDetails(username: "cn");
```

### LDAP Login

To perform LDAP login authentication, use the `LDAPLogin` method with a `LoginViewModel` object.

```csharp
var loginModel = new LoginViewModel { Email = "email", Password = "password", Username = "username" };
var isAuthenticated = adAuth.LDAPLogin(loginModel);
```

## Documentation

For detailed documentation of the methods and properties provided by the `ADAuthentication` class, please refer to the XML comments within the source code.

## Support

For any issues, bugs, or feature requests, please create an issue on the [GitHub repository](https://github.com/Developers2019/ActiveDirectoryLibrary).

## License

This package is licensed under the [MIT License](https://opensource.org/licenses/MIT).

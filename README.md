---
services: active-directory
platforms: dotnet
author: jmprieur
---

# Calling a web API in an ASP.NET Core web application using Azure AD
This sample shows how to build an MVC web application that uses Azure AD for sign-in using the OpenID Connect protocol, and then calls a web API under the signed-in user's identity using tokens obtained via OAuth 2.0. This sample uses the OpenID Connect ASP.Net middleware and ADAL .Net running on ASP.NET Core.

For more information about how the protocols work in this scenario and other scenarios, see [Authentication Scenarios for Azure AD](http://go.microsoft.com/fwlink/?LinkId=394414).

> This sample has finally been updated to ASP.NET Core 1.1.  Looking for previous versions of this code sample? Check out the tags on the [releases](../../releases) GitHub page.

## How To Run This Sample

Getting started is simple!  To run this sample you will need:
- Install .NET Core for Windows by following the instructions at [dot.net/core](https://dot.net/core), which will include Visual Studio 2017.
- An Internet connection
- An Azure Active Directory (Azure AD) tenant. For more information on how to get an Azure AD tenant, please see [How to get an Azure AD tenant](https://azure.microsoft.com/en-us/documentation/articles/active-directory-howto-tenant/) 
- A user account in your Azure AD tenant. This sample will not work with a Microsoft account, so if you signed in to the Azure portal with a Microsoft account and have never created a user account in your directory before, you need to do that now.

### Step 1:  Clone or download this repository

From your shell or command line:

`git clone https://github.com/Azure-Samples/active-directory-dotnet-webapp-webapi-openidconnect-aspnetcore.git`

### Step 2:  Register the sample with your Azure Active Directory tenant

There are two projects in this sample.  Each needs to be separately registered in your Azure AD tenant.

#### Register the TodoListService web API

1. Sign in to the [Azure portal](https://portal.azure.com).
2. On the top bar, click on your account and under the **Directory** list, choose the Active Directory tenant where you wish to register your application.
3. Click on **More Services** in the left hand nav, and choose **Azure Active Directory**.
4. Click on **App registrations** and choose **Add**.
5. Enter a friendly name for the application, for example 'TodoListService' and select 'Web Application and/or Web API' as the Application Type. For the sign-on URL, enter the base URL for the sample, which is by default `https://localhost:44351`. Click on **Create** to create the application.
6. While still in the Azure portal, choose your application, click on **Settings** and choose **Properties**.
7. Find the Application ID value and copy it to the clipboard.
8. For the App ID URI, enter https://\<your_tenant_name\>/TodoListService, replacing \<your_tenant_name\> with the name of your Azure AD tenant.

#### Register the TodoListWebApp web application

1. Sign in to the [Azure portal](https://portal.azure.com).
2. On the top bar, click on your account and under the **Directory** list, choose the Active Directory tenant where you wish to register your application.
3. Click on **More Services** in the left hand nav, and choose **Azure Active Directory**.
4. Click on **App registrations** and choose **Add**.
5. Enter a friendly name for the application, for example 'TodoListWebApp' and select 'Web Application and/or Web API' as the Application Type. For the sign-on URL, enter the base URL for the sample, which is by default `https://localhost:44371/signin-oidc`. 
6. While still in the Azure portal, choose your application, click on **Settings** and choose **Properties**.
7. Find the Application ID value and copy it to the clipboard.
8. On the same page, change the `Logout Url` property to `https://localhost:44371/Account/EndSession`.  This is the default single sign out URL for this sample. 
9. From the Settings menu, choose **Keys** and add a key - select a key duration of either 1 year or 2 years. When you save this page, the key value will be displayed, copy and save the value in a safe location - you will need this key later to configure the project in Visual Studio - this key value will not be displayed again, nor retrievable by any other means, so please record it as soon as it is visible from the Azure Portal.
10. Configure Permissions for your application - in the Settings menu, choose the 'Required permissions' section, click on **Add**, then **Select an API**, and type 'TodoListService' in the textbox. Then, click on  **Select Permissions** and select 'Access TodoListService'.

### Step 3:  Configure the sample to use your Azure AD tenant

#### Configure the TodoListService project

1. Open the solution in Visual Studio 2017.
2. Open the `appsettings.json` file.
3. Find the `Tenant` property and replace the value with your AAD tenant name, e.g. contoso.onmicrosoft.com.
4. Find the `Audience` property and replace the value with the App ID URI you registered earlier, for example `https://<your_tenant_name>/TodoListService`.

#### Configure the TodoListWebApp project

1. Open the solution in Visual Studio 2017.
2. Open the `appsettings.json` file.
3. Find the `Tenant` property and replace the value with your AAD tenant name, e.g. contoso.onmicrosoft.com.
4. Find the `ClientId` property and replace the value with the Client ID for the TodoListWebApp from the Azure portal.
5. Find the `ClientSecret` and replace the value with the key for the TodoListWebApp from the Azure portal.
6. If you changed the base URL of the TodoListWebApp sample, find the `PostLogoutRedirectUri` property and replace the value with the new base URL of the sample.
8. Find the `TodoListResourceId` property and replace the value with the App ID URI registered for the TodoListService, for example `https://<your_tenant_name>/TodoListService`.

### Step 4:  Trust the IIS Express SSL certificate

Since the web API is SSL protected, the client of the API (the web app) will refuse the SSL connection to the web API unless it trusts the API's SSL certificate.  Use the following steps in Windows Powershell to trust the IIS Express SSL certificate.  You only need to do this once.  If you fail to do this step, calls to the TodoListService will always throw an unhandled exception where the inner exception message is:

"The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel."

To configure your computer to trust the IIS Express SSL certificate, begin by opening a Windows Powershell command window as Administrator.

Query your personal certificate store to find the thumbprint of the certificate for `CN=localhost`:

```
PS C:\windows\system32> dir Cert:\LocalMachine\My


    Directory: Microsoft.PowerShell.Security\Certificate::LocalMachine\My


Thumbprint                                Subject
----------                                -------
C24798908DA71693C1053F42A462327543B38042  CN=localhost
```

Next, add the certificate to the Trusted Root store:

```
PS C:\windows\system32> $cert = (get-item cert:\LocalMachine\My\C24798908DA71693C1053F42A462327543B38042)
PS C:\windows\system32> $store = (get-item cert:\Localmachine\Root)
PS C:\windows\system32> $store.Open("ReadWrite")
PS C:\windows\system32> $store.Add($cert)
PS C:\windows\system32> $store.Close()
```

You can verify the certificate is in the Trusted Root store by running this command:

`PS C:\windows\system32> dir Cert:\LocalMachine\Root`

### Step 5:  Run the sample

Clean the solution, rebuild the solution, and run it.  You might want to go into the solution properties and set both projects as startup projects, with the service project starting first.

Explore the sample by signing in, To Do List link, adding items to the To Do list, signing out, and starting again.

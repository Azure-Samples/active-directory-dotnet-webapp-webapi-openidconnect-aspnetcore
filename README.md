---
services:
platforms:
author: azure
---

# WebApp-WebAPI-OpenIdConnect-AspNet5
This sample shows how to build an MVC web application that uses Azure AD for sign-in using the OpenID Connect protocol, and then calls a web API under the signed-in user's identity using tokens obtained via OAuth 2.0. This sample uses the OpenID Connect ASP.Net OWIN middleware and ADAL .Net running on ASP.NET 5.

For more information about how the protocols work in this scenario and other scenarios, see [Authentication Scenarios for Azure AD](http://go.microsoft.com/fwlink/?LinkId=394414).

## How To Run This Sample

To run this sample you will need:
- [Visual Studio 2015 CTP6](http://www.visualstudio.com/downloads/visual-studio-2015-ctp-vs)
- An Internet connection
- An Azure subscription (a free trial is sufficient)

Every Azure subscription has an associated Azure Active Directory tenant.  If you don't already have an Azure subscription, you can get a free subscription by signing up at [http://wwww.windowsazure.com](http://www.windowsazure.com).  All of the Azure AD features used by this sample are available free of charge.

### Step 1:  Clone or download this repository

From your shell or command line:

`git clone https://github.com/AzureADSamples/WebApp-WebAPI-OpenIDConnect-DotNet.git`

### Step 2:  Create a user account in your Azure Active Directory tenant

If you already have a user account in your Azure Active Directory tenant, you can skip to the next step.  This sample will not work with a Microsoft account, so if you signed in to the Azure portal with a Microsoft account and have never created a user account in your directory before, you need to do that now.  If you create an account and want to use it to sign-in to the Azure portal, don't forget to add the user account as a co-administrator of your Azure subscription.

### Step 3:  Register the sample with your Azure Active Directory tenant

There are two projects in this sample.  Each needs to be separately registered in your Azure AD tenant.

#### Register the TodoListService web API

1. Sign in to the [Azure management portal](https://manage.windowsazure.com).
2. Click on Active Directory in the left hand nav.
3. Click the directory tenant where you wish to register the sample application.
4. Click the Applications tab.
5. In the drawer, click Add.
6. Click "Add an application my organization is developing".
7. Enter a friendly name for the application, for example "TodoListService", select "Web Application and/or Web API", and click next.
8. For the sign-on URL, enter the base URL for the sample, which is by default `https://localhost:44321`.
9. For the App ID URI, enter `https://<your_tenant_name>/TodoListService`, replacing `<your_tenant_name>` with the name of your Azure AD tenant.  Click OK to complete the registration.
10. While still in the Azure portal, click the Configure tab of your application.
11. Find the Client ID value and copy it aside, you will need this later when configuring your application.

#### Register the TodoListWebApp web application

1. Sign in to the [Azure management portal](https://manage.windowsazure.com).
2. Click on Active Directory in the left hand nav.
3. Click the directory tenant where you wish to register the sample application.
4. Click the Applications tab.
5. In the drawer, click Add.
6. Click "Add an application my organization is developing".
7. Enter a friendly name for the application, for example "TodoListWebApp", select "Web Application and/or Web API", and click next.
8. For the sign-on URL, enter the base URL for the sample, which is by default `https://localhost:44322/`.  NOTE:  It is important, due to the way Azure AD matches URLs, to ensure there is a trailing slash on the end of this URL.  If you don't include the trailing slash, you will receive an error when the application attempts to redeem an authorization code.
9. For the App ID URI, enter `https://<your_tenant_name>/TodoListWebApp`, replacing `<your_tenant_name>` with the name of your Azure AD tenant.  Click OK to complete the registration.
10. While still in the Azure portal, click the Configure tab of your application.
11. Find the Client ID value and copy it aside, you will need this later when configuring your application.
12. Create a new key for the application.  Save the configuration so you can view the key value.  Save this aside for when you configure the project in Visual Studio.
13. In "Permissions to Other Applications", click "Add Application."  Select "Other" in the "Show" dropdown, and click the upper check mark.  Locate & click on the TodoListService, and click the bottom check mark to add the application.  Select "Access TodoListService" from the "Delegated Permissions" dropdown, and save the configuration.

### Step 4:  Configure the sample to use your Azure AD tenant

#### Configure the TodoListService project

1. Open the solution in Visual Studio 2015.
2. Open the `config.json` file.
3. Find the `Tenant` property and replace the value with your AAD tenant name, e.g. contoso.onmicrosoft.com.
4. Find the app key `Audience` and replace the value with the App ID URI you registered earlier, for example `https://<your_tenant_name>/TodoListService`.

#### Configure the TodoListWebApp project

1. Open the solution in Visual Studio 2015.
2. Open the `config.json` file.
3. Find the `Tenant` property and replace the value with your AAD tenant name, e.g. contoso.onmicrosoft.com.
4. Find the `ClientId` property and replace the value with the Client ID for the TodoListWebApp from the Azure portal.
5. Find the `AppKey` and replace the value with the key for the TodoListWebApp from the Azure portal.
6. If you changed the base URL of the TodoListWebApp sample, find the `PostLogoutRedirectUri` property and replace the value with the new base URL of the sample.
7. Find the property `TodoListBaseAddress` and make sure it has the correct value for the address of the TodoListService project.
8. Find the `TodoListResourceId` property and replace the value with the App ID URI registered for the TodoListService, for example `https://<your_tenant_name>/TodoListService`.

### Step 5:  Trust the IIS Express SSL certificate

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

### Step 6:  Run the sample

Clean the solution, rebuild the solution, and run it.  You might want to go into the solution properties and set both projects as startup projects, with the service project starting first.

Explore the sample by signing in, To Do List link, adding items to the To Do list, signing out, and starting again.

## How To Deploy This Sample to Azure

Coming soon.

## About The Code

Coming soon.

## How To Recreate This Sample

First, in Visual Studio 2015 create an empty solution to host the  projects.  Then, follow these steps to create each project.

### Creating the TodoListService Project

1. In the solution, create a new "ASP.NET 5 MVC Web API" project called TodoListService.
2. Enable SSL on the project by following the steps outlined in the below section.
2. Add the `Microsoft.AspNet.Security.OAuthBearer` and `Microsoft.Framework.ConfigurationModel.Json` NuGets to the project.
2. Create a new `Models` folder, and add a new class to it called `TodoItem.cs`.  Copy the implementation of TodoItem from this sample into the class.
3. Delete the existing `ValuesController.cs`, and add a new Web API controller class called `TodoListController`.
4. Copy the implementation of the TodoListController from this sample into the controller.  Don't forget to add the `[Authorize]` attribute to the class.
5. In `TodoListController` resolving missing references by adding `using` statements for `System.Collections.Concurrent`, `TodoListService.Models`, `System.Security.Claims`.
6. Add a new ASP.NET Configuration File called `config.json` to the project.  Replace its contents with those of the sample.
7. Replace the implementation of `Startup.cs` with that of the sample, resolving any missing references such as `Microsoft.Framework.ConfigurationModel`.

### Creating the TodoListWebApp Project

1. In Visual Studio 2015 CTP6, create a new "ASP.NET 5 Preview Starter Web" application.
2. Enable SSL for the application by following the steps in section at the bottom of this page.
5. Add the `Microsoft.AspNet.Security.OpenIdConnect` ASP.Net OWIN middleware NuGet to the project.  Remember to enable prerelease versions in the NuGet package manager.  Also add the prerelease version of `Microsoft.AspNet.Session` to the project.
5. Remove a few excess files that come with the template - they are not needed for this sample.  Delete the `Migrations` folder, the `Views/Account` folder, the `Models` folder, and the `Compiler` folder.
6. Replace the implementation of the `Controllers\AccountController.cs` class with the one from the project, resolving any excess or missing using statements.
6. In `Views\Shared`, replace the implementation of `_LoginPartial.cshtml` and `_Layout.cshtml` with the ones from the sample.
7. Replace the contents of `config.json` with the one from the sample.
6. Replace the contents of `Startup.cs` with the one from the sample, resolving any excess or missing using statements.  Note that you need to change the class to a `partial` class so you can implement the `ConfigureAuth` method.
7. Create a new folder in the project called `App_Start`, and within it create the `Startup.Auth.cs` class.  Again, make sure it's a partial class and copy in the code from the sample.  This is where the identity related configuration code occurs.
8. Add a `Models` folder to the project, and create the `TodoItem.cs` class in it.  Copy its simple implementation from the sample.
9. Add a `Utils` folder to the project, and create a new class inside it called `NaiveSessionCache.cs`.  The `NaiveSessionCache` extends the ADAL token cache, and is used here to store tokens in session storage.
7. Add a new controller to the project, the `TodoListController.cs`.  Also create a corresponding `TodoList` view.  Copy the implementations in from the sample. Note the use of the `[Authorize]` tag on the `TodoListController` class, to enforce authentication on the methods in this class.
13. Almost done!  Follow the steps in "How To Run This Sample" above to register the application in your AAD tenant.

### Enable SSL in Visual Studio 2015 CTP6
These steps are temporarily necessary to enable SSL only for Visual Studio 2015 CTP6: First, hit F5 to run the application.  Once you see the homepage, you may close the browser and stop IIS Express.  In a text editor, open the file `%userprofile%\documents\IISExpress\config\applicatoinhost.confg`.  Find the entry for your app in the `<sites>` node.  Add an https protocol binding to this entry for a port between 44300 and 44399, similar to the following:

```
<site name="WebApplication1" id="2">
	<application path="/" applicationPool="Clr4IntegratedAppPool">
        	<virtualDirectory path="/" physicalPath="c:\users\billhie\documents\visual studio 2015\Projects\WebApplication1\WebApplication1" />
        </application>
        <bindings>
            <binding protocol="http" bindingInformation="*:53756:localhost" />
            <binding protocol="https" bindingInformation="*:44300:localhost" />
        </bindings>
    </site>
```
Save and close the file.  In Visual Studio, open the properties page of your web app.  In the Debug menu, enable the Launch Browser checkbox and enter the same URL as the protocol binding you added, e.g. `https://localhost:44300/`.  Your app will now run at this address.

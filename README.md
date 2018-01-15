---
services: active-directory
platforms: dotnet
author: jmprieur
level: 300
client: ASP.NET Core 2.0
service: ASP.NET Core 2.0
endpoint: AAD V1
---
# Calling a web API in an ASP.NET Core web application using Azure AD
## About this sample
### Scenario
You expose a Web API and you want to protect it so that only authenticated user can access it. 

This sample presents a Web API running on ASP.NET Core 2.0, protected by Azure AD OAuth Bearer Authentication. The Web API is access by an ASP.NET Core 2.0 Web application, in the name of the signed-in user. 
The ASP.NET Web application uses the OpenID Connect ASP.NET Core middleware and the Active Directory Authentication Library (ADAL.Net) to obtain a JWT access token for the signed-in user through the [OAuth 2.0](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-protocols-oauth-code) protocol. The access token is sent to the ASP.NET Core Web API, which authorizes the user using the ASP.NET JWT Bearer Authentication middleware.

### more information
For more information about how the protocols work in this scenario and other scenarios, see [Authentication Scenarios for Azure AD](http://go.microsoft.com/fwlink/?LinkId=394414).

> This sample been updated to ASP.NET Core 2.0.  Looking for previous versions of this code sample? Check out the tags on the [ASP.NET Core 1.0 version](./tree/aspnet10) branch.

### User experience with this sample
The Web API (TodoListService) maintains an in-memory collection of to-do items per authenticated user. Several applications signed-in under the same identity share the to-do list. These can be
several instances of the Web App proposed in this sample, but also native clients like the [dotnet native (WPF) client](https://github.com/Azure-Samples/active-directory-dotnet-native-aspnetcore) as they share the same service.

The ASP.NET Core 2.0 Web application (TodoListWebApp) enables a user to:
- Sign in. The first time a user signs, a consent screen is presented letting him consent for the application accessing the TodoList Service and the Azure Active Directory (so that the Web App can read the user's profile). Because this is a Web App, hosted in a browser, it can be that the user gets immediately signed-in benefiting from Single Sign On with other web applications. 
- Click on the *Todo List* part of the navigation bar of the application. At that point s/he:
    - sees the list of to-do items exposed by Web API for the signed-in identity (this list would be empty if the service was just started)
    - can add more to-do items (buy clicking on Add item).
- Sign-out

Next time the user navigates to the Web application, s/he is signed-in with the same identity as this identity is persisted in a cookie.
![TodoList WebApp](./Readme/todolist-webApp.png)



## How To Run This Sample
>[!Note] If you want to run this sample on **Azure Government**, navigate to the "Azure Government Deviations" section at the bottom of this page.
>

### Pre-requisites
- Install .NET Core for Windows by following the instructions at [dot.net/core](https://dot.net/core), which will include Visual Studio 2017.
- An Internet connection
- An Azure Active Directory (Azure AD) tenant. For more information on how to get an Azure AD tenant, please see [How to get an Azure AD tenant](https://azure.microsoft.com/en-us/documentation/articles/active-directory-howto-tenant/) 
- A user account in your Azure AD tenant. This sample will not work with a Microsoft account (MSA, live account), so if you signed in to the Azure portal with a Microsoft personal account and have never created a user account in your directory before, you need to do that now (See [Quickstart: Add new users to Azure Active Directory](https://docs.microsoft.com/en-us/azure/active-directory/add-users-azure-active-directory))

### Step 1:  Clone or download this repository

From your shell or command line:

`git clone https://github.com/Azure-Samples/active-directory-dotnet-webapp-webapi-openidconnect-aspnetcore.git`

### Step 2:  Register the sample with your Azure Active Directory tenant

There are two projects in this sample.  Each needs to be separately registered in your Azure AD tenant.

#### Register the TodoListService web API

1. Sign in to the [Azure portal](https://portal.azure.com).
1. On the top bar, click on your account and under the **Directory** list, choose the Active Directory tenant where you wish to register your application.
1. Click on **More Services** in the left hand nav, and choose **Azure Active Directory**.
1. Click on **App registrations** and choose **New application registration**.
1. Enter a friendly **Name** for the application, for example 'TodoListService' and select 'Web Application and/or Web API' as the **Application Type**. For the sign-on URL, enter the base URL for the sample, which is by default `https://localhost:44351`. Click on **Create** to create the application.
1. While still in the Azure portal, choose your application, click on **Settings** and choose **Properties**.
1. From the Azure portal, note the following information, for instance by copying it to notepad:
    - The Tenant domain: See the App ID URI base URL. For example: contoso.onmicrosoft.com 
    - The Tenant ID: See the Endpoints blade (button next to *New application registration*). Record the GUID from any of the endpoint URLs. For example: da41245a5-11b3-996c-00a8-4d99re19f292. Alternatively you can also find the Tenant ID in the Properties of the Azure Active Directory object (this is the value of the **Directory ID** property)
    - The Application ID (Client ID): See the Properties blade. For example: ba74781c2-53c2-442a-97c2-3d60re42f403

#### Register the TodoListWebApp web application

1. If not already done, sign in to the [Azure portal](https://portal.azure.com).
1. On the top bar, click on your account and under the **Directory** list, choose the same Active Directory tenant as for the service
1. Click on **More Services** in the left hand nav, and choose **Azure Active Directory**.
1. Click on **App registrations** and choose **New application registration**.
1. Enter a friendly **Name** for the application, for example 'TodoListWebApp' and select 'Web Application and/or Web API' as the Application Type. For the sign-on URL, enter the base URL for the sample, which is by default `https://localhost:17945/signin-oidc`. 
1. While still in the Azure portal, choose your application, click on **Settings** and choose **Properties**.
1. Find the Application ID value and copy it to the clipboard.
1. On the same page, change the `Logout Url` property to `https://localhost:44371/Account/EndSession`.  This is the default single sign out URL for this sample. 
1. From the Settings menu, choose **Keys** and add a key - select a key duration of either 1 year or 2 years. When you save this page, the key value will be displayed, copy and save the value in a safe location - you will need this key later to configure the project in Visual Studio - this key value will not be displayed again, nor retrievable by any other means, so please record it as soon as it is visible from the Azure Portal.
1. Configure Permissions for your application - in the Settings menu, choose the 'Required permissions' section, click on **Add**, then **Select an API**, and type 'TodoListService' in the textbox. Then, click on  **Select Permissions** and select 'Access TodoListService'.

### Step 3:  Configure the sample to use your Azure AD tenant

#### Configure the TodoListService ASP.NET Core 2.0 C# project

1. Open the solution in Visual Studio.
1. In the TodoListService project, open the `appsettings.json` file.
1. Find the `Domain` property and replace the value with your AAD tenant domain, e.g. contoso.onmicrosoft.com.
1. Find the `TenantId` property and replace the value with the Tenant ID you registered earlier (a GUID), 
1. Find the `ClientId` property and replace the value with the Application ID (Client ID) property of the TodoListService application, that you registered earlier (also a GUID)

#### Configure the TodoListWebApp ASP.NET Core 2.0 C# project

1. Open the solution in Visual Studio.
1. Open the `appsettings.json` file.
1. In the TodoListWebApp project, open the `appsettings.json` file.
1. Find the `Domain` property and replace the value with your AAD tenant domain, e.g. contoso.onmicrosoft.com.
1. Find the `TenantId` property and replace the value with the Tenant ID you registered earlier (a GUID), 
1. Find the `ClientId` property and replace the value with the Application ID (Client ID) property of the *TodoListWebApp* application, that you registered earlier (also a GUID)
1. Find the `ClientSecret` and replace the value with the key for the TodoListWebApp that you noted from the Azure portal.
1. If you changed the base URL of the TodoListWebApp sample, add a `PostLogoutRedirectUri` property and replace the value with the new base URL of the sample.
1. Find the `TodoListResourceId` property and replace the value with ClientId (Application ID) registered for the *TodoListService*, (again this is a GUID)

### Step 4:  Run the sample

Clean the solution, rebuild the solution, and run it.  You might want to go into the solution properties and set both projects as startup projects, with the service project starting first.

When you start the Web API, you will get an empty web page. This is expected.

Explore the sample by signing in into the TodoList Web App, clicking on "Todo List", signing again if needed, adding items to the To Do list, signing-out, and starting again.  As explained, if you close the browser tab (or the browser) without signing-out, the next time you run the application you won't be prompted to sign-in again.

NOTE: Remember, the To Do list is stored in memory in this TodoListService sample. Each time you stop the TodoListService API, your To Do list will get emptied.

## How the code was created?
### Code for the service
The code for the service is exactly the same as in the [active-directory-dotnet-native-aspnetcore](https://github.com/Azure-Samples/active-directory-dotnet-native-aspnetcore) same. Please refer to the readme of that sample to understand how to it was built

### Code for ASP.NET Web App
The code for the ASP.NET Web App is based on the code of the [active-directory-dotnet-webapp-openidconnect-aspnetcore](https://github.com/Azure-Samples/active-directory-dotnet-webapp-openidconnect-aspnetcore) sample. Please read the "About The code" section of that sample first.

Then, based on that code, the following modifications were applied. If you are interested in the details, the following [commit](https://github.com/Azure-Samples/active-directory-dotnet-webapp-webapi-openidconnect-aspnetcore/pull/24/commits/2ce2750dfd172f9297c2d1885cccdd6b66cc7529) details the incremental changes described below:
- Update of the AzureAdOptions class to add a property to compute the `Authority` from the `instance` and the `tenantID`, and adding two other configuration options for `ClientSecret`,  the `resourceId` of TodoListService (its clientId) and the base address for this service.
- Added a `TodoListItem` in models to deserialize the Json sent by the TodoListService
- Added a `NaiveSessionCache` class in a new Utils folder which serves as a token cache which livetime is the duration of the session. Updated the `Startup.cs` file accordingly to add sessions.
- Added a `TodoListController` and a `Todo` view, as well as a "Todo List" entry in the toolbar of the Web API. This is where most of the interesting code is
- Updated the `SignOut()` method of the `AccountController` to clear the cache for the user when s/he signs-out.
- Updated `AzureAdAuthenticationBuilderExtensions.cs` to request an authorization code, and redeem it, getting an access token to the Azure AD graph (https://graph.windows.com), so that the token cache contains a token for the user. This token will be used by the `TodoController` to request another token for the TodoListService

In case you were familiar with this scenario in ASP.NET, you'll want to notice the following line in `AzureAdAuthenticationBuilderExtensions.cs` 

``
options.ResponseType = "id_token code";
``

indeed, contrary to ASP.NET, ASP.NET Core 2.0 seems to use by default an implicit flow. Without overriding the response type (which by default is *id_token*), `OnTokenValidated` event is called instead of  `OnAuthorizationCodeReceived`. In the line above, we request both *id_token* and *code*, so that `OnTokenValidated` is called first which ensures that `context.Principal` has a non-null value represeting the signed-in user when `OnAuthorizeationCodeReceived` is called

### How to change the App URL
If you are using Visual Studio 2017
1. Edit the TodoListService's properties (right click on `TodoListService.csproj`, and choose **Properties**)
1. In the Debug tab:
    1. Check the **Launch browser** field to `https://localhost:44351/api/todolist`
    1. Change the **App URL** field to be `https://localhost:44351` as this is the URL registered in the Azure AD application representing our Web API.
    1. Check the **Enable SSL** field

The same kind of modifications can be made on the `TodoListWebApp.csproj` project.

### What to change when you deploy the sample to Azure
When you deploy this sample to Azure, you will need to:
- update the various URLs (reply URLs, Base URL) in the `appsettings.json` files
- Add Reply URLs pointing to the deployed location, for  both applications in the Azure portal.

### Azure Government Deviations

In order to run this sample on Azure Government you can follow through the steps above with a few variations:

- Step 2: 
   - You must register this sample for your AAD Tenant in Azure Government by following Step 2 above in the [Azure Government portal](https://portal.azure.us). 
- Step 3: 
    - Before configuring the sample, you must make sure your [Visual Studio is connected to Azure Government](https://docs.microsoft.com/azure/azure-government/documentation-government-get-started-connect-with-vs).     
    - Navigate to the appsettings.json files for both the TodoListService web API and TodoListWebApp web application. Replace the "Instance" property in the Azure AD section with `https://login.microsoftonline.us/`. 
    
Once those changes have been accounted for, you should be able to run this sample on Azure Government.


## Related content
### Other documentation / samples
The scenarios involving Azure Active directory with ASP.NET Core are described in ASP.Net Core | Security | Authentication | [Azure Active Directory](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/azure-active-directory/). From this page, you can access the related samples

# Headstart
Welcome! The purpose of this project is to give you and your business a "headstart" to building an ecommerce solution on OrderCloud. This is a complete and opinionated solution including three main parts:

1. [Middleware](./src/Middleware) - The backend written in ASP.NET Core. Extends and enhances the capabilities of OrderCloud by integrating with best of breed services.
2. [Buyer](./src/UI/Buyer) - The frontend buyer application written in Angular. This includes the entire shopping experience from the perspective of a buyer user.
3. [Seller](./src/UI/Seller) - The frontend admin application written in Angular. This includes everything needed to manage the data in your buyer application.

## Initial Setup
There are some tasks that must be completed before you can get an instance of Headstart running. This section will walk you through each of them.

### Accounts
This solution relies on various third party services and credentials for those services. You should have a set of test credentials as well as production credentials. Start by creating an account for all of the services listed.

1. [Avalara](https://www.avalara.com/us/en/get-started/get-started-b.html?adobe_mc_ref=https%3A%2F%2Fwww.avalara.com%2Fus%2Fen%2Findex.html) - Tax calculation
2. [CardConnect](https://cardconnect.com/signup) - Credit card payment processor
3. [Sendgrid](https://signup.sendgrid.com/) - Transactional emails
4. [EasyPost](https://www.easypost.com/signup) - Shipping estimates
5. [SmartyStreets](https://smartystreets.com/pricing) - Address validation

### Seeding OrderCloud Data
This solution depends on a lot of OrderCloud data to be initialized in a particular way. To make it easy when starting a new project we've created an endpoint that does this for you. Just call it with some information, wait a few seconds and presto: You'll have an organization that is seeded with all the right data to get you started immediately.

Detailed Steps:
1. Sign in to the [ordercloud portal](https://portal.ordercloud.io/)
2. Create a new organization in the portal if you dont already have one.
3. Find your organization and save the unique identifier this is your SellerID in step 6.
4. Follow the instructions [here](./src/Middleware/README.md) to start your server locally
5. Download and open [Postman](https://www.postman.com/downloads/) so that you can make API calls to your local server
6. Make a POST to `/seed` endpoint with the body as defined [here]('./src/Middleware/src/Marketplace.Common/Models/Misc/EnvironmentSeed.cs)

## Applications

The major pieces of this solution. Click the links for details, including build steps.

- [Buyer UI](./src/UI/Buyer/README.md)
- [Admin UI (Sellers and Suppliers)](./src/UI/Seller/README.md)
- Marketplace API
  - [Docs](https://marketplace-middleware-test.azurewebsites.net/index.html)
- [Integrations](./src/Middleware/integrations)
- [Orchestration](./src/Middleware/src/Orchestration.Functions/Marketplace.Orchestration/README.md)

## Issue Tracking

We keep track of what needs to be done on a [Jira Board](https://four51.atlassian.net/secure/RapidBoard.jspa?rapidView=167&projectKey=SEB&view=planning&issueLimit=100)

## Git Flow

1.  **Fork** the repo on GitHub
2.  **Clone** the project to your own machine
3.  **Commit** changes to your own branch
4.  **Push** your work back up to your fork
5.  Submit a **Pull request** so that we can review your changes

## Environments

In general we have three environments cooresponding to our three branchs, _dev_, _staging_ and _master_.

- **Test**, where QA happens. Builds automatically on PRs to _dev_.
  - https://marketplace-buyer-ui-test.azurewebsites.net
  - https://marketplace-admin-ui-test.azurewebsites.net
  - https://marketplace-middleware-test.azurewebsites.net
- **Staging**, for demos. Builds automatically on PRs to _staging_.
  - https://marketplace-buyer-ui-staging.azurewebsites.net
  - https://marketplace-admin-ui-staging.azurewebsites.net
  - https://marketplace-middleware-staging.azurewebsites.net
- **Production**, the real deal. Manual builds from staging. Code gets merged to _master_ after the release has been validated.
  - https://marketplace-buyer-ui.azurewebsites.net
  - https://marketplace-admin-ui.azurewebsites.net
  - https://marketplace-middleware.azurewebsites.net

## Seeding data

In order to fully use this app, it will need some data to run against. There are also some configurations that need to be filled out.

- Azure

  - Three app services need to be created. One for Buyer, Seller, and Middleware. From the Azure portal, click 'Create a resource'. Follow the creation flow to create an App Service for each app you'll be hosting.
  - If Application Insights are desired, create a resource for each Application Insight also.

- App Configuration

  -App configs can be imported from JSON. To start with a template, see [Middleware Api Config Template](./src/Middleware/src/Marketplace.Common/AppSettingConfigTemplate.json)

  - If more information is needed on each configuration, see [Middleware Api Config Readme](./src/Middleware/src/Marketplace.Common/AppSettingsReadme.md)

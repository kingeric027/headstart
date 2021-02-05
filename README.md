![Travis (.org) branch](https://img.shields.io/travis/ordercloud-api/ngx-shopper/master.svg?style=flat-square)
![GitHub](https://img.shields.io/github/license/mashape/apistatus.svg?style=flat-square)

# OC-Marketplace

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

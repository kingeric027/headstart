## Building the Buyer App

1.  If you have not before, install the [Angular CLI](https://github.com/angular/angular-cli/wiki) globally on your machine with `npm install -g @angular/cli`

2.  Navigate to the `buyer` Directory with `cd src/UI/Buyer`

3.  Install dependencies with `npm i`

4.  Run `ng serve --configuration=dev` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

### Considerations

- If you change anything in `/marketplace` or somebody else's upstream changes touch that folder, you will need to run `npm i` in `/default-components`.
- In order to change buyers ('Brands'), you must change the value of the `includePaths` key on `angularJson.projects.default-components..architect.build.configurations.dev` in [angular.json](default-components/angular.json), as well as the `appName` constant in the [environment.ts](default-components/src/environments/environment.ts) file. This is also where you can change the url of your local middleware.

## Buyer Theming

| default-components                                                                                                           | marketplace                                                                                                                                                                     |
| ---------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| A single-page web app.                                                                                                       | An npm library locally linked. Published someday.                                                                                                                               |
| Intended to be overridable by different customers                                                                            | Intended to be constant across different customers                                                                                                                              |
| Responsible for presentation. Includes Html, Styling, and thin JS logic.                                                     | Responsible for authentication, app state, api interaction, url route definitions                                                                                               |
| Composed mainly of Web Components generated using Angular Elements.                                                          | Composed mainly of angular services. Would be nice to remove angular dependcy and make framework agnostic.                                                                      |
| One day it would be cool if customers could have unique checkout experiences by injecting different Checkout web components. | One day it would be cool if a customer could install this package and interact with OC using more UI-relatable interface with primitives like CurrentUser, Cart, Checkout, ect. |

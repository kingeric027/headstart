### Zoho ERP Integration

Zoho Authentication is by AuthToken and Organization ID. 
For Marketplace integration there is 1 Organization, so the value is stored in AppSettings.

An AuthToken is acquired and usable without expiration. Therefore, it will also be stored in AppSettings

#### Acquiring AuthToken

* Login to your Zoho Account and open this link: [Link](https://accounts.zoho.com/apiauthtoken/create?SCOPE=ZohoBooks/booksapi)
* Click on the “Generate Authtoken” button

#### Managing AuthToken
You can view all generated AuthTokens at [Link](https://accounts.zoho.com/home#sessions/userauthtoken)

There you can delete tokens no longer active or authorized

#### Account Interface
Zoho Books account login is at [Link](https://accounts.zoho.com/signin?servicename=ZohoBooks&signupurl=https://www.zoho.com/us/books/signup/)

If needed, contact Steve Davis for account registration
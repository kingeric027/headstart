import * as dotenv from 'dotenv'

dotenv.config()

interface testConfig {
	buyerAppUrl: string
	adminAppUrl: string
	automationClientID: string
	automationClientSecret: string
	adminSellerUsername: string
	adminSellerPassword: string
	adminAppClientID: string
	buyerAppClientID: string
}

const testConfig: testConfig = {
	buyerAppUrl: 'https://marketplace-buyer-ui-test.azurewebsites.net/',
	adminAppUrl: 'https://marketplace-admin-ui-test.azurewebsites.net/',
	automationClientID: process.env.AUTOMATION_CLIENT_ID,
	automationClientSecret: process.env.AUTOMATION_CLIENT_SECRET,
	adminSellerUsername: process.env.ADMIN_SELLER_USER_USERNAME,
	adminSellerPassword: process.env.ADMIN_SELLER_USER_PASSWORD,
	adminAppClientID: process.env.ADMIN_APP_CLIENT_ID,
	buyerAppClientID: process.env.BUYER_APP_CLIENT_ID,
}

export default testConfig

import * as dotenv from 'dotenv'

dotenv.config()

interface testConfig {
    appUrl: string
    adminClientID: string
    adminClientSecret: string
}

const testConfig: testConfig = {
    appUrl: 'https://marketplace-buyer-ui-test.azurewebsites.net/',
    adminClientID: process.env.ADMIN_CLIENT_ID,
    adminClientSecret: process.env.ADMIN_CLIENT_SECRET
}

export default testConfig

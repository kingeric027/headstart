import { Configuration, SdkConfiguration } from '@ordercloud/headstart-sdk'

export function setHeadstartSDKUrl() {
	const config: SdkConfiguration = {
		baseApiUrl: 'https://marketplace-middleware-test.azurewebsites.net',
	}

	Configuration.Set(config)
}

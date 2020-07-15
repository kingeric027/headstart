import OrderCloudSDK = require('ordercloud-javascript-sdk')
import faker = require('faker')

export async function createUser(clientAuth: string, buyerID: string) {
	const firstName = faker.name.firstName()
	const lastName = faker.name.lastName()
	const firstNameReplaced = firstName.replace(/'/g, '')
	const lastNameReplaced = lastName.replace(/'/g, '')
	const email = `${firstNameReplaced}${lastNameReplaced}.hpmqx9la@mailosaur.io`

	const testUser: OrderCloudSDK.User = {
		Active: true,
		Email: email,
		FirstName: firstNameReplaced,
		LastName: firstNameReplaced,
		Username: email,
		ID: `${buyerID}-{${buyerID}-UserIncrementor}`,
		xp: {
			AutomationUser: true,
		},
		Password: 'Test123!',
	}

	const createdTestUser = await OrderCloudSDK.Users.Create(buyerID, testUser, {
		accessToken: clientAuth,
	})

	testUser.ID = createdTestUser.ID

	return testUser
}

export async function deleteUser(
	userID: string,
	buyerID: string,
	clientAuth: string
) {
	await OrderCloudSDK.Users.Delete(buyerID, userID, {
		accessToken: clientAuth,
	})
}
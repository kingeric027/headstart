import OrderCloudSDK = require('ordercloud-javascript-sdk')
import faker = require('faker')
import { saveUserAssignment } from './usergroups-helper'

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
		LastName: lastNameReplaced,
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

	//make user assignments so user is able to see products
	//0005-0002 is Denver CO
	//Dxd1cuubXU2BJkdnLf3v1A is All Location Products
	//iJhQ4uM-1UaFruXemXNZaw is Canada Only Products
	await saveUserAssignment(testUser.ID, '0005-0002', '0005', clientAuth)
	await saveUserAssignment(
		testUser.ID,
		'Dxd1cuubXU2BJkdnLf3v1A',
		'0005',
		clientAuth
	)
	await saveUserAssignment(
		testUser.ID,
		'iJhQ4uM-1UaFruXemXNZaw',
		'0005',
		clientAuth
	)

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

export async function getUserID(
	username: string,
	buyerID: string,
	clientAuth: string
) {
	const searchResponse = await OrderCloudSDK.Users.List(
		buyerID,
		{
			search: username,
			searchOn: 'Username',
		},
		{ accessToken: clientAuth }
	)

	const user = searchResponse.Items.find(x => x.Username === username)

	if (user.Username.includes('.hpmqx9la@mailosaur.io')) return user.ID
}

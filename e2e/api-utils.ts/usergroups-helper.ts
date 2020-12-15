import OrderCloudSDK = require('ordercloud-javascript-sdk')

export async function saveUserAssignment(
	userID: string,
	userGroupID: string,
	buyerID: string,
	authToken: string
) {
	const assignment: OrderCloudSDK.UserGroupAssignment = {
		UserID: userID,
		UserGroupID: userGroupID,
	}

	await OrderCloudSDK.UserGroups.SaveUserAssignment(buyerID, assignment, {
		accessToken: authToken,
	})
}

export function getLocationID(country?: string) {
	switch (country) {
		case 'CA':
			return '0005-0003' //Mississauga, ON (Canada)
	}
	return '0005-0002' //Denver, CO (United States)
}

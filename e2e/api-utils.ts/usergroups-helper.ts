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

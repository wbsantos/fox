CREATE OR REPLACE PROCEDURE  fox_group_delete_V1(_id uuid)
LANGUAGE SQL
BEGIN ATOMIC
	DELETE FROM DocumentPermission WHERE holderId = _id
	DELETE FROM SystemPermission WHERE holderId = _id
	DELETE FROM UserGroup WHERE groupId = _id

	DELETE FROM Group WHERE id = _id
	DELETE FROM Holder WHERE id = _id
END
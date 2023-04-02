CREATE OR REPLACE PROCEDURE  fox_group_delete_v1(_id uuid)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM DocumentPermission WHERE holderId = _id;
	DELETE FROM SystemPermission WHERE holderId = _id;
	DELETE FROM UserGroup WHERE groupId = _id;

	DELETE FROM GroupAccount WHERE id = _id;
	DELETE FROM Holder WHERE id = _id;
END
$$;
CREATE OR REPLACE PROCEDURE  fox_user_delete_v1(_id uuid)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM DocumentPermission WHERE holderId = _id;
	DELETE FROM SystemPermission WHERE holderId = _id;
	DELETE FROM UserGroup WHERE userId = _id;

	DELETE FROM Stamp WHERE userId = _id;
	DELETE FROM UserAccount WHERE id = _id;
	DELETE FROM Holder WHERE id = _id;
END
$$;
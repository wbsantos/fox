CREATE OR REPLACE FUNCTION fox_user_read_permission_v1(_userId uuid)
RETURNS TABLE (permission varchar(255))
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			S1.permission
		FROM SystemPermission S1
		WHERE 
			S1.holderId = _userId
		UNION
		SELECT
			S2.permission
		FROM UserGroup U
		INNER JOIN SystemPermission S2  ON S2.holderId = U.groupId
		WHERE
			U.userId = _userId;
END
$$;
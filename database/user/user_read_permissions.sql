CREATE OR REPLACE FUNCTION fox_user_read_permission_v1(_user_id uuid)
RETURNS TABLE (permission varchar(255))
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			S1.permission
		FROM system_permission S1
		WHERE 
			S1.holder_id = _user_id
		UNION
		SELECT
			S2.permission
		FROM user_group U
		INNER JOIN system_permission S2  ON S2.holder_id = U.group_id
		WHERE
			U.user_id = _user_id;
END
$$;
CREATE OR REPLACE FUNCTION fox_group_read_users_v1(_group_id uuid)
RETURNS TABLE (
	id uuid,
	email varchar(255),
	login varchar(63),
	name varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			U.id,
			U.email,
			U.login,
			U.name
		FROM user_account U
		INNER JOIN user_group UG ON
			UG.group_id = _group_id
			AND UG.user_id = U.id;
END
$$;
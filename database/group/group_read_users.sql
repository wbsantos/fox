CREATE OR REPLACE FUNCTION  fox_group_read_users_v1(_groupId uuid)
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
		FROM UserAccount U
		INNER JOIN UserGroup UG ON
			UG.groupId = _groupId
			AND UG.userId = U.id;
END
$$;
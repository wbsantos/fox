CREATE OR REPLACE FUNCTION  fox_user_read_groups_v1(_userId uuid)
RETURNS TABLE (
	name varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			G.name
		FROM GroupAccount G
		INNER JOIN UserGroup UG ON
			UG.groupId = G.id
			AND UG.userId = _userId;
END
$$;
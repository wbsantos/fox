CREATE OR REPLACE FUNCTION fox_user_read_groups_v1(_user_id uuid)
RETURNS TABLE (
	id uuid,
	name varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			G.id, 
			G.name
		FROM group_account G
		INNER JOIN user_group UG ON
			UG.group_id = G.id
			AND UG.user_id = _user_id;
END
$$;
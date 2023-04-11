CREATE OR REPLACE PROCEDURE fox_group_deluser_v1 (
			_group_id uuid,
			_user_ids uuid[]
)
LANGUAGE plpgsql AS
$$
BEGIN
	
	DELETE FROM Stamp
	USING user_group, UNNEST(_user_ids) AS _user_id
	WHERE 
		user_group.stamp_id = Stamp.Id
		AND user_group.user_id = _user_id
		AND user_group.group_id = _group_id;

	DELETE FROM user_group G
	USING UNNEST(_user_ids) AS _user_id 
	WHERE
		G.user_id = _user_id
		AND G.group_id = _group_id;

END
$$;
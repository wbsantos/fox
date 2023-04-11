CREATE OR REPLACE PROCEDURE fox_group_adduser_v1 (
			_stamp_id int,
			_group_id uuid,
			_user_idsToAdd uuid[]
)
LANGUAGE plpgsql AS
$$
BEGIN
	INSERT INTO user_group (group_id, user_id, stamp_id)
	SELECT _group_id, _user_id, _stamp_id
	FROM UNNEST(_user_idsToAdd) AS _user_id
	WHERE
		NOT EXISTS(SELECT 1 FROM user_group U
				   WHERE
				   		U.group_id = _group_id
				   		AND U.user_id = _user_id);
END
$$;
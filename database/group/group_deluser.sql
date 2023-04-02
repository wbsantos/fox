CREATE OR REPLACE PROCEDURE  fox_group_deluser_v1 (
			_groupId uuid,
			_userIds uuid[]
)
LANGUAGE plpgsql AS
$$
BEGIN
	
	DELETE FROM Stamp
	USING UserGroup, UNNEST(_userIds) AS _userId
	WHERE 
		UserGroup.stampId = Stamp.Id
		AND UserGroup.userId = _userId
		AND UserGroup.groupId = _groupId;

	DELETE FROM UserGroup G
	USING UNNEST(_userIds) AS _userId 
	WHERE
		G.userId = _userId
		AND G.groupId = _groupId;

END
$$;
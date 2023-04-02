CREATE OR REPLACE PROCEDURE  fox_group_adduser_v1 (
			_stampId int,
			_groupId uuid,
			_userIdsToAdd uuid[]
)
LANGUAGE plpgsql AS
$$
BEGIN
	INSERT INTO UserGroup (groupId, userId, stampId)
	SELECT _groupId, _userId, _stampId
	FROM UNNEST(_userIdsToAdd) AS _userId
	WHERE
		NOT EXISTS(SELECT 1 FROM UserGroup U
				   WHERE
				   		U.groupId = _groupId
				   		AND U.userId = _userId);
END
$$;
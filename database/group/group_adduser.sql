CREATE OR REPLACE PROCEDURE  fox_group_adduser_V1 (
			_stampId int,
			_groupId uuid,
			_userIdsToAdd uuid[]
)
LANGUAGE SQL
BEGIN ATOMIC
	INSERT INTO UserGroup (groupId, userId, stampId)
	SELECT _groupId, _userId, _stampId
	FROM UNNEST(_userIdsToAdd) AS _userId

END
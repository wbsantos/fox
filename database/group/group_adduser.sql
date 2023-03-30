CREATE OR REPLACE PROCEDURE  fox_group_adduser_V1 (
			_groupId uuid,
			_userIds uuid[]
)
LANGUAGE SQL
BEGIN ATOMIC
	
	INSERT INTO UserGroup (groupId, userId)
	SELECT _groupId, _userId 
	FROM UNNEST(_userIds) AS _userId

END
CREATE OR REPLACE PROCEDURE  fox_group_deluser_V1 (
			_groupId uuid,
			_userIds uuid[]
)
LANGUAGE SQL
BEGIN ATOMIC
	
	DELETE S FROM Stamp 
	JOIN UserGroup ON UserGroup.stampId = Stamp.Id 
	JOIN UNNEST(_userIds) AS _userId ON UserGroup.userId = _userId
	WHERE UserGroup.groupId = _groupId

	DELETE G FROM UserGroup G
	INNER JOIN UNNEST(_userIds) AS _userId ON
		G.userId = _userId
	WHERE
		G.groupId = _groupId

END
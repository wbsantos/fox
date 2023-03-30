CREATE OR REPLACE PROCEDURE  fox_group_deluser_V1 (
			_groupId uuid,
			_userIds uuid[]
)
LANGUAGE SQL
BEGIN ATOMIC
	
	DELETE G FROM UserGroup G
	INNER JOIN UNNEST(_userIds) AS _userId ON
		G.userId = _userId
	WHERE
		G.groupId = _groupId

END
CREATE OR REPLACE PROCEDURE  fox_system_addpermission_V1 (
			_stampId int,
			_holderId uuid,
			_permission varchar(255)
)
LANGUAGE SQL
BEGIN ATOMIC
	INSERT INTO SystemPermission (stampId, holderId, permission)
	VALUES (_stampId, _holderId, _permission)
END
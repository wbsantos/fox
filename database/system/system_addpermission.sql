CREATE OR REPLACE PROCEDURE  fox_system_addpermission_v1 (
			_stampId int,
			_holderId uuid,
			_permission varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	INSERT INTO SystemPermission (stampId, holderId, permission)
	VALUES (_stampId, _holderId, _permission);
END
$$;
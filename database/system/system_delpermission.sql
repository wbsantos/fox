CREATE OR REPLACE PROCEDURE  fox_system_delpermission_v1 (
			_holderId uuid,
			_permission varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM SystemPermission
	WHERE
		holderId = _holderId
		AND permission = _permission;
END
$$;
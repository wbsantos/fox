CREATE OR REPLACE PROCEDURE fox_system_delpermission_v1 (
			_holder_id uuid,
			_permission varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM system_permission
	WHERE
		holder_id = _holder_id
		AND permission = _permission;
END
$$;
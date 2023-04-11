CREATE OR REPLACE PROCEDURE fox_system_addpermission_v1 (
			_stamp_id int,
			_holder_id uuid,
			_permission varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	INSERT INTO system_permission (stamp_id, holder_id, permission)
	VALUES (_stamp_id, _holder_id, _permission);
END
$$;
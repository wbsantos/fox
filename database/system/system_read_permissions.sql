CREATE OR REPLACE FUNCTION fox_system_read_permission_v1(_holder_id uuid)
RETURNS TABLE (permission varchar(255))
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			S1.permission
		FROM system_permission S1
		WHERE 
			S1.holder_id = _holder_id;
END
$$;
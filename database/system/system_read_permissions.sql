CREATE OR REPLACE FUNCTION fox_system_read_permission_v1(_holderId uuid)
RETURNS TABLE (permission varchar(255))
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			S1.permission
		FROM SystemPermission S1
		WHERE 
			S1.holderId = _holderId;
END
$$;
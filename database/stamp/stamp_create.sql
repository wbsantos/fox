CREATE OR REPLACE FUNCTION fox_stamp_create_v1 (
			_user_id uuid,
			_system_version varchar(255)
) RETURNS INTEGER 
LANGUAGE plpgsql AS
$$
DECLARE _stamp_id int;
BEGIN
	
	INSERT INTO Stamp (user_id, system_version, created_at)
	VALUES (_user_id, _system_version, now()) 
	RETURNING id into _stamp_id;

	RETURN _stamp_id;
END
$$;
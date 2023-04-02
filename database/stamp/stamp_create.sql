CREATE OR REPLACE FUNCTION  fox_stamp_create_v1 (
			_userId uuid,
			_systemVersion varchar(255)
) RETURNS INTEGER 
LANGUAGE plpgsql AS
$$
DECLARE _stampId int;
BEGIN
	
	INSERT INTO Stamp (userId, systemVersion, createdAt)
	VALUES (_userId, _systemVersion, now()) 
	RETURNING id into _stampId;
ß
	RETURN _stampId;
END
$$;
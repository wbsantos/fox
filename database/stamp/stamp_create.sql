CREATE OR REPLACE FUNCTION  fox_stamp_create_V1 (
			_userId uuid,
			_systemVersion varchar(255)
) RETURNS INTEGER AS
LANGUAGE SQL
BEGIN ATOMIC
	DECLARE _stampId int
	
	INSERT INTO Stamp (userId, systemVersion, createdAt)
	VALUES (_user, _systemVersion, now()) 
	RETURNING id into _stampId

	RETURN _stampId
END
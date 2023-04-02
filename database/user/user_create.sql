CREATE OR REPLACE FUNCTION fox_user_create_v1 (
			_email varchar(255),
			_login varchar(63),
			_password bytea,
			_salt bytea,
			_hashMethod smallint,
			_name varchar(255))
RETURNS UUID
LANGUAGE plpgsql AS
$$
DECLARE _holderid uuid;
BEGIN
	
	INSERT INTO Holder DEFAULT VALUES RETURNING id into _holderId;

    INSERT INTO UserAccount
	(
		id,
		email,
		login,
		password,
		salt,
		hashMethod,
		name
    )
	VALUES
	(
		_holderId,
		_email,
		_login,
		_password,
		_salt,
		_hashMethod,
		_name
    );
   
   RETURN _holderId;
END
$$;
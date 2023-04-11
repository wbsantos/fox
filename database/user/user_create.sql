CREATE OR REPLACE FUNCTION fox_user_create_v1 (
			_email varchar(255),
			_login varchar(63),
			_password bytea,
			_salt bytea,
			_hash_method int,
			_name varchar(255))
RETURNS UUID
LANGUAGE plpgsql AS
$$
DECLARE _holder_id uuid;
BEGIN
	
	INSERT INTO Holder DEFAULT VALUES RETURNING id into _holder_id;

    INSERT INTO user_account
	(
		id,
		email,
		login,
		password,
		salt,
		hash_method,
		name
    )
	VALUES
	(
		_holder_id,
		_email,
		_login,
		_password,
		_salt,
		_hash_method,
		_name
    );
   
   RETURN _holder_id;
END
$$;
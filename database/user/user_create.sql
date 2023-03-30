CREATE OR REPLACE PROCEDURE  fox_user_create_V1 (
			_email varchar(255),
			_login varchar(63),
			_password bytea,
			_salt bytea,
			_hashMethod smallint,
			_name varchar(255))
LANGUAGE SQL
BEGIN ATOMIC
	DECLARE _holderId uuid
	INSERT INTO Holder DEFAULT VALUES RETURNING id into _holderId

    INSERT INTO User
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
    )

END
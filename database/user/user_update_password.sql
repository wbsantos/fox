CREATE OR REPLACE PROCEDURE  fox_user_update_V1(
			_id uuid,
			_password bytea,
			_salt bytea,
			_hashMethod smallint)
LANGUAGE SQL
BEGIN ATOMIC
	UPDATE User
	SET
		password = _password,
		salt = _salt,
		hashMethod = _hashMethod
	WHERE
		id = _id
END
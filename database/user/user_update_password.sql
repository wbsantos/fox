CREATE OR REPLACE PROCEDURE  fox_user_update_password_v1(
			_id uuid,
			_password bytea,
			_salt bytea,
			_hashMethod int)
LANGUAGE plpgsql AS
$$
BEGIN
	UPDATE UserAccount
	SET
		password = _password,
		salt = _salt,
		hashMethod = _hashMethod
	WHERE
		id = _id;
END
$$;
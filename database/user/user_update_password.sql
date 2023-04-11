CREATE OR REPLACE PROCEDURE fox_user_update_password_v1(
			_id uuid,
			_password bytea,
			_salt bytea,
			_hash_method int)
LANGUAGE plpgsql AS
$$
BEGIN
	UPDATE user_account
	SET
		password = _password,
		salt = _salt,
		hash_method = _hash_method
	WHERE
		id = _id;
END
$$;
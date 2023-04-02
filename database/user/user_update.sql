CREATE OR REPLACE PROCEDURE  fox_user_update_v1(
			_id uuid,
			_email varchar(255),
			_login varchar(63),
			_name varchar(255))
LANGUAGE plpgsql AS
$$
BEGIN
	UPDATE UserAccount
	SET
		email = _email,
		login = _login,
		name = _name
	WHERE
		id = _id;
END
$$;
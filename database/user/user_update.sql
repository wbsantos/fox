CREATE OR REPLACE PROCEDURE  fox_user_update_V1(
			_id uuid,
			_email varchar(255),
			_login varchar(63),
			_name varchar(255))
LANGUAGE SQL
BEGIN ATOMIC
	UPDATE User
	SET
		email = _email,
		login = _login,
		name = _name
	WHERE
		id = _id
END
CREATE OR REPLACE PROCEDURE  fox_user_read_byid_V1(_id uuid)
LANGUAGE SQL
BEGIN ATOMIC
	SELECT
		id,
		email,
		login,
		password,
		salt,
		hash_method,
		name
	FROM User
	WHERE 
		id = _id
END
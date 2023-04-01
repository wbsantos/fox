CREATE OR REPLACE FUNCTION  fox_user_read_byid_V1(_id uuid)
RETURNS setof UserAccount
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			id,
			email,
			login,
			password,
			salt,
			hashMethod,
			name
		FROM UserAccount
		WHERE 
			id = _id;
END
$$;

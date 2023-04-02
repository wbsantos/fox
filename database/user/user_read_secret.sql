CREATE OR REPLACE FUNCTION  fox_user_read_secret_v1(_userId uuid)
RETURNS TABLE (
	id uuid,
	password bytea,
	salt bytea,
	hashMethod smallint
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			id,
			password,
			salt,
			hashMethod
		FROM UserAccount
		WHERE 
			id = _id;
END
$$;

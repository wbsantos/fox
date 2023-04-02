CREATE OR REPLACE FUNCTION  fox_user_read_secret_v1(_userLogin varchar(63))
RETURNS TABLE (
	id uuid,
	password bytea,
	salt bytea,
	hashMethod int
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			U.id,
			U.password,
			U.salt,
			U.hashMethod
		FROM UserAccount U
		WHERE 
			U.login = _userLogin;
END
$$;

--drop function fox_user_read_secret_v1
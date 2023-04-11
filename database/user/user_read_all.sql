CREATE OR REPLACE FUNCTION fox_user_read_all_v1()
RETURNS TABLE (
	id uuid,
	email varchar(255),
	login varchar(63),
	name varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			U.id,
			U.email,
			U.login,
			U.name
		FROM user_account U;
END
$$;
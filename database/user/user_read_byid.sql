CREATE OR REPLACE FUNCTION  fox_user_read_byid_v1(_id uuid)
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
		FROM UserAccount U
		WHERE 
			U.id = _id;
END
$$;
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
			id,
			email,
			login,
			name
		FROM UserAccount
		WHERE 
			id = _id;
END
$$;
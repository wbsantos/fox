CREATE OR REPLACE FUNCTION  fox_group_read_byid_v1(_id uuid)
RETURNS TABLE (
	id uuid,
	name varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query
		SELECT id, name FROM GroupAccount WHERE id = _id;
END
$$;
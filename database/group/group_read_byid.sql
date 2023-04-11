CREATE OR REPLACE FUNCTION fox_group_read_byid_v1(_id uuid)
RETURNS TABLE (
	id uuid,
	name varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query
		SELECT 
			G.id, 
			G.name
		FROM group_account G 
		WHERE 
			g.id = _id;
END
$$;
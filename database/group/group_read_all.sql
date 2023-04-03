CREATE OR REPLACE FUNCTION fox_group_read_all_v1()
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
		FROM GroupAccount G;
END
$$;
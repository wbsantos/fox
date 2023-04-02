CREATE OR REPLACE PROCEDURE  fox_group_read_byid_v1(_id uuid)
LANGUAGE plpgsql AS
$$
BEGIN
	SELECT name FROM GroupAccount WHERE id = _id;
END
$$;
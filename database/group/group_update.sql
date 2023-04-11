CREATE OR REPLACE PROCEDURE fox_group_update_v1(
			_id uuid,
			_name varchar(255))
LANGUAGE plpgsql AS
$$
BEGIN
	UPDATE group_account SET name = _name WHERE id = _id;
END
$$;
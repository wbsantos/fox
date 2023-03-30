CREATE OR REPLACE PROCEDURE  fox_group_update_V1(
			_id uuid,
			_name varchar(255))
LANGUAGE SQL
BEGIN ATOMIC
	UPDATE Group SET name = _name WHERE id = _id
END
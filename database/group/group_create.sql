CREATE OR REPLACE PROCEDURE  fox_group_create_V1 (_name varchar(255))
LANGUAGE SQL
BEGIN ATOMIC
	DECLARE _holderId uuid
	INSERT INTO Holder DEFAULT VALUES RETURNING id into _holderId

    INSERT INTO Group (name) VALUES (_name)
END
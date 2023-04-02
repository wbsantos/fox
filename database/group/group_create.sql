CREATE OR REPLACE PROCEDURE  fox_group_create_v1 (_name varchar(255))
LANGUAGE plpgsql AS
$$
DECLARE _holderId uuid;
BEGIN
	INSERT INTO Holder DEFAULT VALUES RETURNING id into _holderId;
    INSERT INTO GroupAccount (name) VALUES (_name);
END
$$;
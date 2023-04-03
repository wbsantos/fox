CREATE OR REPLACE FUNCTION  fox_group_create_v1 (_name varchar(255))
RETURNS UUID
LANGUAGE plpgsql AS
$$
DECLARE _holderId uuid;
BEGIN
	INSERT INTO Holder DEFAULT VALUES RETURNING id into _holderId;
    INSERT INTO GroupAccount (id, name) VALUES (_holderId, _name);
   
   RETURN _holderId;
END
$$;
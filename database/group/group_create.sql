CREATE OR REPLACE FUNCTION fox_group_create_v1 (_name varchar(255))
RETURNS UUID
LANGUAGE plpgsql AS
$$
DECLARE _holder_id uuid;
BEGIN
	INSERT INTO Holder DEFAULT VALUES RETURNING id into _holder_id;
    INSERT INTO group_account (id, name) VALUES (_holder_id, _name);
   
   RETURN _holder_id;
END
$$;
CREATE OR REPLACE FUNCTION  fox_document_read_binary_v1(_id uuid)
RETURNS bytea
LANGUAGE plpgsql AS
$$
DECLARE _fileBinary bytea;
BEGIN
	
	SELECT 
		fileBinary
	INTO _fileBinary
	FROM Document 
	WHERE id = _id ;
	
	RETURN _fileBinary;
END
$$;
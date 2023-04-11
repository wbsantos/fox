CREATE OR REPLACE FUNCTION fox_document_read_binary_v1(_id uuid)
RETURNS bytea
LANGUAGE plpgsql AS
$$
DECLARE _file_binary bytea;
BEGIN
	
	SELECT 
		file_binary
	INTO _file_binary
	FROM Document 
	WHERE id = _id ;
	
	RETURN _file_binary;
END
$$;
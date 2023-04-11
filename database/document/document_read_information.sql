CREATE OR REPLACE FUNCTION fox_document_read_information_v1(_document_id uuid)
RETURNS TABLE (
	id uuid,
	name varchar(255),
	file_size_bytes int
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query
		SELECT
			D.id,
			D.name,
			D.file_size_bytes 
		FROM Document D 
		WHERE D.id = _document_id;
END
$$;
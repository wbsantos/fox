CREATE OR REPLACE FUNCTION  fox_document_read_information_v1(_documentId uuid)
RETURNS TABLE (
	id uuid,
	name varchar(255),
	fileSizeBytes int
)
LANGUAGE plpgsql AS
$$
BEGIN
	SELECT
		D.id,
		D.name,
		D.filesizebytes 
	FROM Document D 
	WHERE D.id = _documentId;
END
$$;
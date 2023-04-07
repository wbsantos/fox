CREATE OR REPLACE FUNCTION  fox_document_read_metadata_v1(_documentId uuid)
RETURNS TABLE (
	key varchar(255),
	value varchar(1023)
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query
		SELECT
			M.key,
			M.value
		FROM DocumentMetadata M 
		WHERE M.documentId = _documentId;
END
$$;
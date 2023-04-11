CREATE OR REPLACE FUNCTION fox_document_read_metadata_v1(_document_id uuid)
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
		FROM document_metadata M 
		WHERE M.document_id = _document_id;
END
$$;
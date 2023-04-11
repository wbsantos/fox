CREATE OR REPLACE PROCEDURE fox_document_delmetadata_v1 (
			_document_id uuid,
			_keys varchar(255)[]
)
LANGUAGE plpgsql AS
$$
BEGIN

	DELETE FROM document_metadata M
	USING UNNEST(_keys) AS _key
	WHERE
		M.key = _key
		AND M.document_id = _document_id;

END
$$;
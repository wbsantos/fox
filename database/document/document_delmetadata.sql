CREATE OR REPLACE PROCEDURE  fox_document_delmetadata_v1 (
			_documentId uuid,
			_keys varchar(255)[]
)
LANGUAGE plpgsql AS
$$
BEGIN

	DELETE FROM DocumentMetadata M
	USING UNNEST(_keys) AS _key
	WHERE
		M.key = _key
		AND M.documentId = _documentId;

END
$$;
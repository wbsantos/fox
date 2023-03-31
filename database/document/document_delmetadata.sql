CREATE OR REPLACE PROCEDURE  fox_document_delmetadata_V1 (
			_documentId uuid,
			_keys varchar(255)[]
)
LANGUAGE SQL
BEGIN ATOMIC

	DELETE M FROM DocumentMetadata M
	INNER JOIN UNNEST(_keys) AS _key ON
		M.key = _key
	WHERE
		M.documentId = _documentId

END